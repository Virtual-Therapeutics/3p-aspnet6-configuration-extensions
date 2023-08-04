using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DevTrends.ConfigurationExtensions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConfigurationExtensions.Tests;

/// <summary>
/// A base class to help reduce human error from copy-pasting a lot of boilerplate for generics.
/// </summary>
/// <typeparam name="T">The record type to test binding on</typeparam>
public abstract class BindingTestBase<T>
{
    /// <summary>
    /// Populate the configuration manager with the configuration to bind.
    /// </summary>
    protected abstract Dictionary<string, string> CreateConfig();

    /// <summary>Compare the records.</summary>
    /// <remarks>
    /// While records have value equality, for development's sake it's preferable
    /// to assert on each value individually. Also, collection types within the record
    /// may need special consideration.
    /// </remarks>
    protected abstract void AssertEqualsExpected(T actual);

    /// <summary>
    /// Converts field name to make initialization boilerplate less error-prone.
    /// </summary>
    protected static string KeyFromPropertyName(string propertyName)
        => $"{typeof(T).Name}:{typeof(T).GetProperty(propertyName).NotNull().Name}";
    //{
    //    var (result, _) = KeyAndTypeFromPropertyOfType(propertyName, typeof(T));
    //    return result;
    //}

    protected virtual T TypedBind(string? sectionKey = null) => ConfigManager.Bind<T>(sectionKey);

    protected ConfigurationManager ConfigManager { get; private init; } = new();

    private static (string Key, Type Type) KeyAndTypeFromPropertyOfType(string propertyName, Type type)
    {
        PropertyInfo prop = type.GetProperty(propertyName)!;
        Assert.NotNull(prop);

        return (propertyName, prop.PropertyType);
    }

    protected static string KeyFromNestedProperties(params string[] nestedNames)
    {
        var (subKeys, nestedType) = KeyAndTypeFromPropertyOfType(nestedNames[0], typeof(T));
        foreach (var name in nestedNames[1..])
        {
            if (int.TryParse(name, out int index))
            {
                var typeInfo = nestedType.GetTypeInfo();
                subKeys += $":{index}";
                nestedType = nestedType
                    .GetInterface(typeof(IEnumerable<>).FullName!).NotNull()
                    .GetGenericArguments().First(); // todo: be better
            }
            else
            {
                var propInfo = nestedType.GetProperty(name).NotNull();
                subKeys += $":{propInfo.Name}";
                nestedType = propInfo.PropertyType;
            }
        }

        return $"{typeof(T).Name}:{subKeys}";
    }

    protected static string NthKeyFromNestedProperties(int index, params string[] nestedNames)
        => $"{KeyFromNestedProperties(nestedNames)}:{index}";

    [Fact]
    public virtual void Binds()
    {
        ConfigManager.AddInMemoryCollection(CreateConfig());
        T result = ConfigManager.Bind<T>();
        AssertEqualsExpected(result);
    }
}
