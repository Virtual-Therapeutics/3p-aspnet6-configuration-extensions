using System.Collections;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace DevTrends.ConfigurationExtensions;

public static class IConfigurationExtensions
{
    public static T Bind<T>(this IConfiguration configuration, string? sectionKey = null)
    {
        return (T)Bind(configuration, typeof(T), sectionKey)!;
    }

    private static object? Bind(this IConfiguration configuration, Type type,
        string? sectionKey, bool isNullable = false)
    {
        var keyToUse = sectionKey ?? type.Name;

        var constructors = type.GetConstructors().ToList();

        if (constructors.Count < 1)
        {
            throw new ArgumentException($"Type '{type.Name}' does not have a public constructor", nameof(type));
        }

        var query = constructors.Select(x => new
        {
            Constructor = x,
            Parameters = x.GetParameters().ToList()
        }).OrderByDescending(x => x.Parameters.Count).First();

        if (isNullable && query.Parameters.Count > 0 && !configuration.GetSection(keyToUse).Exists())
        {
            return null;
        }

        var parameters = new List<object?>();

        foreach (var parameter in query.Parameters)
        {
            var key = $"{keyToUse}:{parameter.Name}";

            parameters.Add(GetValue(configuration, key, parameter));
        }

        return query.Constructor.Invoke(parameters.ToArray());
    }

    private static object? GetValue(IConfiguration configuration, string key, ParameterInfo parameter)
    {
        var nullableType = GetNullableType(parameter);

        var type = nullableType ?? parameter.ParameterType;

        if (type != typeof(string) && type.IsAssignableTo(typeof(IEnumerable)))
        {
            var section = configuration.GetSection(key);
            IEnumerable? result = default;
            try
            {
                result = section.Get(type) as IEnumerable;
                // Get() with a set-like type returns a List.
                // Lists don't implicitly convert to sets.
                // So we need special code here to check if `type` is set-like,
                // and if so, construct a different class ourselves.
                result = HandleSetTypes(type, result);
            }
            catch (InvalidOperationException)
            {
                // This shows up, for example, when trying to Get() something from System.Collections.Immutable
            }

            if (result is not null && result.GetEnumerator().MoveNext())
            {
                // MoveNext() returning true tells us the result is not empty
                return result;
            }
            else
            {
                // Construct the class implementing IEnumerable<T>
                var containedType = type.GenericTypeArguments[0];
                var children = new ReflectedGenericList(containedType);

                foreach (var child in section.GetChildren())
                {
                    object? item = section.GetValue(type.GenericTypeArguments[0], child.Key);
                    item ??= Bind(configuration, type.GenericTypeArguments[0], $"{key}:{child.Key}");
                    children.Add(item);
                }
                return children.AsContainerOfT(type);
            }
        }

        if (type.IsClass && type != typeof(string) && type != typeof(Uri))
        {
            return Bind(configuration, type, key, nullableType != null);
        }

        var value = configuration[key];

        if (nullableType != null)
        {
            if (value == null) return null;
        }
        else
        {
            if (value == null)
            {
                throw new ConfigurationBindException($"Missing configuration key '{key}'. Unable to set {parameter.Name}.");
            }
        }

        if (type == typeof(string)) return value;

        if (type.IsValueType)
        {
            try
            {
                return Convert.ChangeType(value, type);
            }
            catch (Exception e) when (e is FormatException ||
                                      e is InvalidCastException ||
                                      e is OverflowException)
            {
                throw new ConfigurationBindException($"Error converting value '{value}' to {type.Name}. Source: '{key}'", e);
            }
        }

        if (type == typeof(DateTime))
        {
            if (DateTime.TryParse(value, out var dateTimeValue))
            {
                return dateTimeValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to a DateTime. Source: '{key}'");
        }

        if (type == typeof(Uri))
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out var uriValue))
            {
                return uriValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to a Uri. Source: '{key}'");
        }

        throw new ConfigurationBindException($"Unhandled type '{type.FullName}'");
    }

    private static IEnumerable? HandleSetTypes(Type type, IEnumerable? result)
    {
        if (result is null) return null;
        if (type.IsConstructedGenericType)
        {
            var containedType = type.GetGenericArguments().First();
            var constructedSetType = typeof(HashSet<>).MakeGenericType(containedType);

            if (type.IsAssignableFrom(constructedSetType))
            {
                // Activator.CreateInstance() only returns null when constructing a Nullable<T> with no value.
                // That's not going to be the case here.
                return (IEnumerable)Activator.CreateInstance(constructedSetType, result)!;
            }
        }
        return result;
    }

    private static Type? GetNullableType(ParameterInfo parameter)
    {
        if (parameter.ParameterType.IsClass
            && new NullabilityInfoContext().Create(parameter).WriteState != NullabilityState.NotNull)
        {
            return parameter.ParameterType;
        }

        return Nullable.GetUnderlyingType(parameter.ParameterType);
    }
}
