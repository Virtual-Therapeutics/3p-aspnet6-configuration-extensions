using System;
using System.Collections.Generic;
using DevTrends.ConfigurationExtensions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConfigurationExtensions.Tests;

public class RequiredStringTests : BindingTestBase<RequiredString>
{
    const string EXPECTED = "text string";

    protected override void AssertEqualsExpected(RequiredString actual)
        => Assert.Equal(EXPECTED, actual.Foo);
    protected override Dictionary<string, string> CreateConfig()
    {
        return new()
        {
            { KeyFromPropertyName("Foo"), EXPECTED }
        };
    }

    // Additional Tests
    [Fact]
    public void Binds_Using_Explicit_Section_Name()
    {
        ConfigManager.AddInMemoryCollection(new Dictionary<string, string>()
        {
            { $"Foo:Foo", EXPECTED }
        });

        var result = TypedBind("Foo");

        AssertEqualsExpected(result);
    }

    [Fact]
    public void Throws_When_Config_Missing()
    {
        Assert.Throws<ConfigurationBindException>(() => TypedBind());
    }
}

public class OptionalStringTests : BindingTestBase<OptionalString>
{
    const string EXPECTED = "text string";

    protected override void AssertEqualsExpected(OptionalString actual)
        => Assert.Equal(EXPECTED, actual.Foo);

    protected override Dictionary<string, string> CreateConfig()
        => new()
        {
            { KeyFromPropertyName("Foo"), EXPECTED }
        };


    // Additional Tests
    [Fact]
    public void OptionalString_Binds_Null_When_Config_Missing()
    {
        var result = TypedBind();

        Assert.Null(result.Foo);
    }
}

public class MultipleTypesTests : BindingTestBase<MultipleTypes>
{
    const string EXPECTED_FOO = "s1";
    const int EXPECTED_BAR = 42;
    const bool EXPECTED_OTHER = true;
    const decimal EXPECTED_MISC = 3.14m;
    const string DATE_STRING = "1999-12-31";
    const string URL_STRING = "https://example.com";
    protected override void AssertEqualsExpected(MultipleTypes actual)
    {
        Assert.Equal(EXPECTED_FOO, actual.Foo);
        Assert.Equal(EXPECTED_BAR, actual.Bar);
        Assert.Equal(EXPECTED_OTHER, actual.Other);
        Assert.Equal(EXPECTED_MISC, actual.Misc);
        Assert.Equal(DateTime.Parse(DATE_STRING), actual.Blah);
        Assert.Equal(new Uri(URL_STRING), actual.Url);
    }

    protected override Dictionary<string, string> CreateConfig()
    {
        return new()
        {
            { KeyFromPropertyName("Foo"), EXPECTED_FOO },
            { KeyFromPropertyName("Bar"), EXPECTED_BAR.ToString() },
            { KeyFromPropertyName("Other"), EXPECTED_OTHER.ToString() },
            { KeyFromPropertyName("Misc"), EXPECTED_MISC.ToString("0.##") },
            { KeyFromPropertyName("Blah"), DATE_STRING },
            { KeyFromPropertyName("Url"), URL_STRING }
        };
    }

    // Additional Tests
    [Theory]
    [InlineData("Bar")]
    [InlineData("Other")]
    [InlineData("Misc")]
    [InlineData("Blah")]
    [InlineData("Url")]
    public void Throws_When_Missing_NotNullableField(string name)
    {
        var config = CreateConfig();
        Assert.True(config.Remove(KeyFromPropertyName(name)));
        ConfigManager.AddInMemoryCollection(config);

        var ex = Assert.Throws<ConfigurationBindException>(() => TypedBind());
        Assert.Contains($"Unable to set {name}", ex.Message);
    }
}

public class MultipleOptionalTypesTests : BindingTestBase<MultipleOptionalTypes>
{
    protected override void AssertEqualsExpected(MultipleOptionalTypes actual)
    {
        Assert.Null(actual.Foo);
        Assert.Null(actual.Bar); ;
        Assert.Null(actual.Other);
        Assert.Null(actual.Misc);
        Assert.Null(actual.Blah);
        Assert.Null(actual.Url);
    }
    
    protected override Dictionary<string, string> CreateConfig() => new();
}

public class NoPublicConstructorTest
{
    [Fact]
    public void Throws()
    {
        ConfigurationManager configurationManager = new();
        Assert.Throws<ArgumentException>(() => configurationManager.Bind<NoPublicConstructor>());
    }
}

// Simple Records
public record RequiredString(string Foo);
public record OptionalString(string? Foo);

public record MultipleTypes(string Foo, int Bar, bool Other, decimal Misc, DateTime Blah, Uri Url);
public record MultipleOptionalTypes(string? Foo, int? Bar, bool? Other, decimal? Misc, DateTime? Blah, Uri? Url);

public class NoPublicConstructor { private NoPublicConstructor() { } }
