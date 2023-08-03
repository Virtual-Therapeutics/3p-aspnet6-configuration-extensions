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

        if (type == typeof(int))
        {
            if (int.TryParse(value, out var intValue))
            {
                return intValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to an int. Source: '{key}'");
        }

        if (type == typeof(bool))
        {
            if (bool.TryParse(value, out var boolValue))
            {
                return boolValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to a bool. Source: '{key}'");
        }

        if (type == typeof(decimal))
        {
            if (decimal.TryParse(value, out var decimalValue))
            {
                return decimalValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to a decimal. Source: '{key}'");
        }

        if (type == typeof(DateTime))
        {
            if (DateTime.TryParse(value, out var dateTimeValue))
            {
                return dateTimeValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to a datetime. Source: '{key}'");
        }

        if (type == typeof(Uri))
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out var uriValue))
            {
                return uriValue;
            }

            throw new ConfigurationBindException($"Error converting value '{value}' to a uri. Source: '{key}'");
        }

        throw new ConfigurationBindException($"Unhandled type '{type.FullName}'");
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
