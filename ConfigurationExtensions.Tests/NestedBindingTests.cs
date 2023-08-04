using System;
using System.Collections.Generic;
using ConfigurationExtensions.Tests;
using DevTrends.ConfigurationExtensions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConfigurationExtensions.Nested.Tests;

public class NestedRequiredTests : BindingTestBase<NestedRequired>
{
    protected override void AssertEqualsExpected(NestedRequired actual)
    {
        Assert.Equal("hi", actual.Something);
        Assert.NotNull(actual.Other);
        Assert.Equal("nested", actual.Other.Foo);
    }
    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { KeyFromPropertyName("Something"), "hi" },
            { KeyFromNestedProperties("Other", "Foo"), "nested" }
        };

    // Additional Tests
    [Fact]
    public void Throws_When_Config_Missing()
    {
        ConfigManager.AddInMemoryCollection(new Dictionary<string, string>()
        {
            { KeyFromPropertyName("Something"), "hi" }
        });

        Assert.Throws<ConfigurationBindException>(() => TypedBind());
    }
}

// Tests for record { record { string? } }
public class NestedRequiredOptionalTests : BindingTestBase<NestedRequiredOptional>
{
    protected override void AssertEqualsExpected(NestedRequiredOptional actual)
    {
        Assert.Equal("hi", actual.Something);
        Assert.NotNull(actual.Other);
        Assert.Equal("nested", actual.Other.Foo);
    }

    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { KeyFromPropertyName("Something"), "hi" },
            { KeyFromNestedProperties("Other", "Foo"), "nested" }
        };

    // Additional Tests
    [Fact]
    public void Binds_Null_When_Config_Missing()
    {
        ConfigManager.AddInMemoryCollection(new Dictionary<string, string>()
        {
            { KeyFromPropertyName("Something"), "hi" }
        });
        var actual = TypedBind();
        Assert.NotNull(actual.Other);
        Assert.Null(actual.Other.Foo);
    }
}

// Tests for record { record? { string } }
public class NestedOptionalRequiredTests : BindingTestBase<NestedOptionalRequired>
{
    protected override void AssertEqualsExpected(NestedOptionalRequired actual)
    {
        Assert.Equal("hi", actual.Something);
        Assert.NotNull(actual.Other);
        Assert.Equal("nested", actual.Other!.Foo);
    }

    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { KeyFromPropertyName("Something"), "hi" },
            { KeyFromNestedProperties("Other", "Foo"), "nested" }
        };

    // Additional tests
    [Fact]
    public void Binds_Null_When_Config_Missing()
    {
        ConfigManager.AddInMemoryCollection(new Dictionary<string, string>()
        {
            { KeyFromPropertyName("Something"), "hi" }
        });

        var actual = TypedBind();

        Assert.Equal("hi", actual.Something);
        Assert.Null(actual.Other);
    }
}


public class NestedNoPublicConstructorTests
{
    [Fact]
    public void Throws()
    {
        ConfigurationManager configurationManager = new();
        Assert.Throws<ArgumentException>(() => configurationManager.Bind<NestedNoPublicConstructor>());
    }
}


// Nested Records - Base Types are defined in SimpleBindingTests.cs
public record NestedRequired(string Something, RequiredString Other);
public record NestedRequiredOptional(string Something, OptionalString Other);
public record NestedOptionalRequired(string Something, RequiredString? Other);
public record NestedNoPublicConstructor(NoPublicConstructor Other);
