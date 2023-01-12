using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DevTrends.ConfigurationExtensions.Tests;

public class IConfigurationExtensionsTests
{
    [Fact]
    public void NoPublicConstructor_Throws()
    {
        var configurationManager = new ConfigurationManager();

        Assert.Throws<ArgumentException>(() => configurationManager.Bind<NoPublicConstructor>());
    }

    [Fact]
    public void RequiredString_Throws_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<RequiredString>());
    }

    [Fact]
    public void RequiredString_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Foo:Foo", "text string")
        });

        var result = configurationManager.Bind<RequiredString>("Foo");

        Assert.Equal("text string", result.Foo);
    }

    [Fact]
    public void RequiredString_Binds_Using_Inferred_Section_Name()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("RequiredString:Foo", "text string")
        });

        var result = configurationManager.Bind<RequiredString>();

        Assert.Equal("text string", result.Foo);
    }

    [Fact]
    public void OptionalString_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("OptionalString:Foo", "optional string")
        });

        var result = configurationManager.Bind<OptionalString>();

        Assert.Equal("optional string", result.Foo);
    }

    [Fact]
    public void OptionalString_Binds_Null_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        var result = configurationManager.Bind<OptionalString>();

        Assert.Null(result.Foo);
    }

    [Fact]
    public void MultipleTypes_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleTypes:Misc", "3.14"),
            new KeyValuePair<string, string>("MultipleTypes:Blah", "1999-12-31"),
            new KeyValuePair<string, string>("MultipleTypes:Url", "https://www.foo.com"),
        });

        var result = configurationManager.Bind<MultipleTypes>();

        Assert.Equal("s1", result.Foo);
        Assert.Equal(42, result.Bar);
        Assert.True(result.Other);
        Assert.Equal(3.14m, result.Misc);
        Assert.Equal(DateTime.Parse("1999-12-31"), result.Blah);
        Assert.Equal(new Uri("https://www.foo.com"), result.Url);
    }

    [Fact]
    public void MultipleTypes_Throws_When_Missing_Int()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleTypes:Misc", "3.14"),
            new KeyValuePair<string, string>("MultipleTypes:Blah", "1999-12-31"),
            new KeyValuePair<string, string>("MultipleTypes:Url", "https://www.foo.com"),
        });

        var ex = Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<MultipleTypes>());
        Assert.Contains("Unable to set Bar", ex.Message);
    }

    [Fact]
    public void MultipleTypes_Throws_When_Missing_Bool()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleTypes:Misc", "3.14"),
            new KeyValuePair<string, string>("MultipleTypes:Blah", "1999-12-31"),
            new KeyValuePair<string, string>("MultipleTypes:Url", "https://www.foo.com"),
        });

        var ex = Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<MultipleTypes>());
        Assert.Contains("Unable to set Other", ex.Message);
    }

    [Fact]
    public void MultipleTypes_Throws_When_Missing_Decimal()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleTypes:Blah", "1999-12-31"),
            new KeyValuePair<string, string>("MultipleTypes:Url", "https://www.foo.com"),
        });

        var ex = Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<MultipleTypes>());
        Assert.Contains("Unable to set Misc", ex.Message);
    }

    [Fact]
    public void MultipleTypes_Throws_When_Missing_DateTime()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleTypes:Misc", "3.14"),
            new KeyValuePair<string, string>("MultipleTypes:Url", "https://www.foo.com"),
        });

        var ex = Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<MultipleTypes>());
        Assert.Contains("Unable to set Blah", ex.Message);
    }

    [Fact]
    public void MultipleTypes_Throws_When_Missing_Uri()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleTypes:Misc", "3.14"),
            new KeyValuePair<string, string>("MultipleTypes:Blah", "1999-12-31")
        });

        var ex = Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<MultipleTypes>());
        Assert.Contains("Unable to set Url", ex.Message);
    }

    [Fact]
    public void MultipleOptionalTypes_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MultipleOptionalTypes:Foo", "s1"),
            new KeyValuePair<string, string>("MultipleOptionalTypes:Bar", "42"),
            new KeyValuePair<string, string>("MultipleOptionalTypes:Other", bool.TrueString),
            new KeyValuePair<string, string>("MultipleOptionalTypes:Misc", "3.14"),
            new KeyValuePair<string, string>("MultipleOptionalTypes:Blah", "1999-12-31"),
            new KeyValuePair<string, string>("MultipleOptionalTypes:Url", "https://www.foo.com"),
        });

        var result = configurationManager.Bind<MultipleOptionalTypes>();

        Assert.Equal("s1", result.Foo);
        Assert.Equal(42, result.Bar);
        Assert.True(result.Other);
        Assert.Equal(3.14m, result.Misc);
        Assert.Equal(DateTime.Parse("1999-12-31"), result.Blah);
        Assert.Equal(new Uri("https://www.foo.com"), result.Url);
    }

    [Fact]
    public void MultipleOptionalTypes_Binds_Null_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        var result = configurationManager.Bind<MultipleOptionalTypes>();

        Assert.Null(result.Foo);
        Assert.Null(result.Bar);
        Assert.Null(result.Other);
        Assert.Null(result.Misc);
        Assert.Null(result.Blah);
        Assert.Null(result.Url);
    }

    [Fact]
    public void Nested_NoPublicConstructor_Throws()
    {
        var configurationManager = new ConfigurationManager();

        Assert.Throws<ArgumentException>(() => configurationManager.Bind<NestedNoPublicConstructor>());
    }

    [Fact]
    public void NestedRequired_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedRequired:Something", "hi"),
            new KeyValuePair<string, string>("NestedRequired:Other:Foo", "nested")
        });

        var result = configurationManager.Bind<NestedRequired>();

        Assert.Equal("hi", result.Something);
        Assert.NotNull(result.Other);
        Assert.Equal("nested", result.Other.Foo);
    }

    [Fact]
    public void NestedRequired_Throws_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedRequired:Something", "hi")
        });

        Assert.Throws<ConfigurationBindException>(() => configurationManager.Bind<NestedRequired>());
    }

    [Fact]
    public void NestedOptional_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedOptional:Something", "hi"),
            new KeyValuePair<string, string>("NestedOptional:Other:Foo", "nested")
        });

        var result = configurationManager.Bind<NestedOptional>();

        Assert.Equal("hi", result.Something);
        Assert.NotNull(result.Other);
        Assert.Equal("nested", result.Other.Foo);
    }

    [Fact]
    public void NestedOptional_Binds_Null_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedOptional:Something", "hi")
        });

        var result = configurationManager.Bind<NestedOptional>();

        Assert.Equal("hi", result.Something);
        Assert.NotNull(result.Other);
        Assert.Null(result.Other.Foo);
    }

    [Fact]
    public void NestedOptionalType_Binds()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedOptionalType:Something", "hi"),
            new KeyValuePair<string, string>("NestedOptionalType:Other:Foo", "nested")
        });

        var result = configurationManager.Bind<NestedOptionalType>();

        Assert.Equal("hi", result.Something);
        Assert.NotNull(result.Other);
        Assert.Equal("nested", result.Other!.Foo);
    }

    [Fact]
    public void NestedOptionalType_Binds_Null_When_Config_Missing()
    {
        var configurationManager = new ConfigurationManager();

        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("NestedOptionalType:Something", "hi")
        });

        var result = configurationManager.Bind<NestedOptionalType>();

        Assert.Equal("hi", result.Something);
        Assert.Null(result.Other);
    }

    [Fact]
    public void ListOfIntType_Binds()
    {
        string typeName = nameof(ListOfIntType);
        string propertyName = nameof(ListOfIntType.Ints);

        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0", "123"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:1", "456"),
        });

        var result = configurationManager.Bind<ListOfIntType>();

        var expected = new List<int> { 123, 456 };
        Assert.True(expected.SequenceEqual(result.Ints));
    }

    [Fact]
    public void HashSetOfIntType_Binds()
    {
        string typeName = nameof(HashSetOfIntType);
        string propertyName = nameof(HashSetOfIntType.Ints);

        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0", "123"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:1", "456"),
        });

        var result = configurationManager.Bind<HashSetOfIntType>();

        var expected = new HashSet<int>() { 123, 456 };
        Assert.True(expected.SetEquals(result.Ints));
    }

    [Fact]
    public void IEnumerableOfIntType_Binds()
    {
        string typeName = nameof(IEnumerableOfIntType);
        string propertyName = nameof(IEnumerableOfIntType.Ints);

        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0", "123"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:1", "456"),
        });

        var result = configurationManager.Bind<IEnumerableOfIntType>();

        IEnumerable<int> expected = new List<int> { 123, 456 };
        Assert.True(expected.SequenceEqual(result.Ints));
    }

    [Fact]
    public void ListOfStringType_Binds()
    {
        string typeName = nameof(ListOfStringType);
        string propertyName = nameof(ListOfStringType.Strings);

        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0", "123"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:1", "456"),
        });

        var result = configurationManager.Bind<ListOfStringType>();

        var expected = new List<string> { "123", "456" };
        Assert.True(expected.SequenceEqual(result.Strings));
    }

    [Fact]
    public void HashSetOfStringType_Binds()
    {
        string typeName = nameof(HashSetOfStringType);
        string propertyName = nameof(HashSetOfStringType.Strings);

        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0", "123"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:1", "456"),
        });

        var result = configurationManager.Bind<HashSetOfStringType>();

        var expected = new HashSet<string> { "123", "456" };
        Assert.True(expected.SetEquals(result.Strings));
    }

    [Fact]
    public void IEnumerableOfStringType_Binds()
    {
        string typeName = nameof(IEnumerableOfStringType);
        string propertyName = nameof(IEnumerableOfStringType.Strings);

        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0", "123"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:1", "456"),
        });

        var result = configurationManager.Bind<IEnumerableOfStringType>();
        IEnumerable<string> expected = new List<string> { "123", "456" };
        Assert.True(expected.SequenceEqual(result.Strings));
    }

    [Fact]
    public void ListOfListType_Binds()
    {
        string typeName = nameof(ListOfListsType);
        string propertyName = nameof(ListOfListsType.Lists);

        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:0", "123"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:1", "456"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:1:0", "789"),
        });

        var result = configurationManager.Bind<ListOfListsType>();

        Assert.Equal(2, result.Lists.Count);
        Assert.Equal(2, result.Lists[0].Count);
        Assert.Single(result.Lists[1]);
        Assert.Equal("123", result.Lists[0][0]);
        Assert.Equal("456", result.Lists[0][1]);
        Assert.Equal("789", result.Lists[1][0]);
    }

    [Fact]
    public void ListOfMultipleTypes_Binds()
    {
        string typeName = nameof(ListOfMultipleTypes);
        string propertyName = nameof(ListOfMultipleTypes.MultipleTypes);

        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:Foo", "s1"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:Bar", "42"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:Other", bool.TrueString),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:Misc", "3.14"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:Blah", "1999-12-31"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:Url", "https://www.example.com"),
        });

        var listType = configurationManager.Bind<ListOfMultipleTypes>();

        Assert.Single(listType.MultipleTypes);
        var result = listType.MultipleTypes[0];
        Assert.Equal("s1", result.Foo);
        Assert.Equal(42, result.Bar);
        Assert.True(result.Other);
        Assert.Equal(3.14m, result.Misc);
        Assert.Equal(DateTime.Parse("1999-12-31"), result.Blah);
        Assert.Equal(new Uri("https://www.example.com"), result.Url);
    }

    [Fact]
    public void ListOfComplexTypes_Binds()
    {
        string typeName = nameof(ListOfComplexTypes);
        string propertyName = nameof(ListOfComplexTypes.List);

        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:String", "s1"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:StringSet:0", "ss_0"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:StringSet:1", "ss_1"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:StringSet:2", "ss_2"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:OtherTypes:0:First", "first.first"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:OtherTypes:0:Second", "first.second"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:OtherTypes:0:Third", "123"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:OtherTypes:1:First", "second.first"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:OtherTypes:1:Second", "second.second"),
            new KeyValuePair<string, string>($"{typeName}:{propertyName}:0:OtherTypes:1:Third", "456"),
        });

        var result = configurationManager.Bind<ListOfComplexTypes>();
        Assert.Single(result.List);
        var complexType = result.List[0];

        Assert.Equal("s1", complexType.String);
        Assert.True((new HashSet<string>() { "ss_0", "ss_1", "ss_2" }).SetEquals(complexType.StringSet));
        Assert.True((new HashSet<EmbeddedType>() {
            new ("first.first", "first.second", 123),
            new ("second.first", "second.second", 456),
        }).SetEquals(complexType.OtherTypes));
    }

}

public class NoPublicConstructor
{
    private NoPublicConstructor()
    {
    }
}

public record RequiredString(string Foo);

public record OptionalString(string? Foo);

public record MultipleTypes(string Foo, int Bar, bool Other, decimal Misc, DateTime Blah, Uri Url);

public record MultipleOptionalTypes(string? Foo, int? Bar, bool? Other, decimal? Misc, DateTime? Blah, Uri? Url);

public record NestedRequired(string Something, RequiredString Other);

public record NestedOptional(string Something, OptionalString Other);

public record NestedNoPublicConstructor(NoPublicConstructor Other);

public record NestedOptionalType(string Something, RequiredString? Other);

public record ListOfIntType(IReadOnlyList<int> Ints);
// For some Reason, IReadOnlyet<> fails...
public record HashSetOfIntType(HashSet<int> Ints);
public record IEnumerableOfIntType(IReadOnlyCollection<int> Ints);

public record ListOfStringType(IReadOnlyList<string> Strings);
// For some Reason, IReadOnlyet<> fails...
public record HashSetOfStringType(HashSet<string> Strings);
public record IEnumerableOfStringType(IReadOnlyCollection<string> Strings);

public record ListOfListsType(IReadOnlyList<IReadOnlyList<string>> Lists);

// Looks like IReadOnlyList fails here, too...
public record ListOfMultipleTypes(List<MultipleTypes> MultipleTypes);

public record EmbeddedType(string First, string Second, int Third);
// For some Reason, IReadOnlyet<> fails...
public record ComplexType(string String, HashSet<string> StringSet, HashSet<EmbeddedType> OtherTypes);
// Looks like IReadOnlyList fails here, too...
public record ListOfComplexTypes(List<ComplexType> List);
