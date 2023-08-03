using System;
using System.Collections.Generic;
using System.Linq;
using ConfigurationExtensions.Tests;
using FluentAssertions;
using FluentAssertions.Collections;
using Xunit;

namespace ConfigurationExtensions.Collection.Nested.Tests;

public static class EquivalenceExtension
{
    public static bool IsEquivalentTo<T>(this IEnumerable<T> self, IEnumerable<T> expected)
    {
        try
        {
            self.Should().BeEquivalentTo(expected);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class ListOfListTests : BindingTestBase<ReadOnlyListOfLists>
{
    protected override void AssertEqualsExpected(ReadOnlyListOfLists actual)
    {
        var expected = new List<List<string>>()
        {
            new() { "hello" },
            new() { "embedded", "list" }
        };

        actual.Lists.Should().Equal(expected, equalityComparison: (a, e) => a.IsEquivalentTo(e));
    }

    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { KeyFromNestedProperties("Lists", "0", "0"), "hello" },
            { KeyFromNestedProperties("Lists", "1", "0"), "embedded" },
            { KeyFromNestedProperties("Lists", "1", "1"), "list" }
        };
}

public class ListOfSetTests : BindingTestBase<ReadOnlyListOfSets>
{
    protected override void AssertEqualsExpected(ReadOnlyListOfSets actual)
    {
        var expected = new List<HashSet<int>>()
        {
            new() { 123, 456 },
            new() { 456, 789 }
        };

        actual.Sets.Should().Equal(expected, equalityComparison: (a, e) => a.SetEquals(e));
    }

    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { KeyFromNestedProperties("Sets", "0", "0"), "456" },
            { KeyFromNestedProperties("Sets", "0", "1"), "123" },
            { KeyFromNestedProperties("Sets", "1", "0"), "456" },
            { KeyFromNestedProperties("Sets", "1", "1"), "789" }
        };
}

public class MutableListOfMultipleTypesTests : BindingTestBase<ListOfMultipleTypes>
{
    protected override void AssertEqualsExpected(ListOfMultipleTypes actual)
    {
        var expected = new List<MultipleTypes>()
        {
            new("s1", 42, true, 3.14m, DateTime.Parse("1999-12-31"), new Uri("https://example.com"))
        };

        actual.MultipleTypes.Should().BeEquivalentTo(expected);
    }

    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { KeyFromNestedProperties("MultipleTypes","0", "Foo"), "s1" },
            { KeyFromNestedProperties("MultipleTypes","0", "Bar"), "42" },
            { KeyFromNestedProperties("MultipleTypes","0", "Other"), bool.TrueString },
            { KeyFromNestedProperties("MultipleTypes","0", "Misc"), "3.14" },
            { KeyFromNestedProperties("MultipleTypes","0", "Blah"), "1999-12-31" },
            { KeyFromNestedProperties("MultipleTypes","0", "Url"), "https://example.com" },
        };
}

public class ListOfMultipleTypesTests : BindingTestBase<ReadOnlyListOfMultipleTypes>
{
    protected override void AssertEqualsExpected(ReadOnlyListOfMultipleTypes actual)
    {
        var expected = new List<MultipleTypes>()
        {
            new("s1", 42, true, 3.14m, DateTime.Parse("1999-12-31"), new Uri("https://example.com"))
        };

        actual.MultipleTypes.Should().BeEquivalentTo(expected);
    }

    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { KeyFromNestedProperties("MultipleTypes","0", "Foo"), "s1" },
            { KeyFromNestedProperties("MultipleTypes","0", "Bar"), "42" },
            { KeyFromNestedProperties("MultipleTypes","0", "Other"), bool.TrueString },
            { KeyFromNestedProperties("MultipleTypes","0", "Misc"), "3.14" },
            { KeyFromNestedProperties("MultipleTypes","0", "Blah"), "1999-12-31" },
            { KeyFromNestedProperties("MultipleTypes","0", "Url"), "https://example.com" },
        };
}

public class MutableListOfComplexTypeTests : BindingTestBase<ListOfComplexTypes>
{
    protected override void AssertEqualsExpected(ListOfComplexTypes actual)
    {
        List<ComplexType> expected = new()
        {
            new(
                String: "s1",
                StringSet: new HashSet<string>() {"ss_0", "ss_1", "ss_2"},
                MultipleTypes: new HashSet<MultipleTypes>()
                {
                    new("first.foo", 1, true, 1.23m, new DateTime(2001,01,01), new Uri("https://example.com")),
                    new("second.foo", 2, false, 456m, new DateTime(2022, 02, 02), new Uri("https://example.org"))
                })
        };

        actual.ComplexTypes.IsEquivalentTo(expected);
    }

    private static string ComplexKeyFrom(params string[] nested)
    {
        var prefix = new List<string>() { "ComplexTypes", "0" };
        prefix.AddRange(nested);
        return KeyFromNestedProperties(prefix.ToArray());
    }

    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { ComplexKeyFrom("String"), "s1" },
            { ComplexKeyFrom("StringSet", "0"), "ss_0" },
            { ComplexKeyFrom("StringSet", "1"), "ss_1" },
            { ComplexKeyFrom("StringSet", "2"), "ss_2" },
            { ComplexKeyFrom("MultipleTypes", "0", "Foo"), "first.foo" },
            { ComplexKeyFrom("MultipleTypes", "0", "Bar"), "1" },
            { ComplexKeyFrom("MultipleTypes", "0", "Other"), "true" },
            { ComplexKeyFrom("MultipleTypes", "0", "Misc"), "1.23" },
            { ComplexKeyFrom("MultipleTypes", "0", "Blah"), "2001-01-01" },
            { ComplexKeyFrom("MultipleTypes", "0", "Url"), "http://example.com" },
            { ComplexKeyFrom("MultipleTypes", "1", "Foo"), "second.foo" },
            { ComplexKeyFrom("MultipleTypes", "1", "Bar"), "2" },
            { ComplexKeyFrom("MultipleTypes", "1", "Other"), "false" },
            { ComplexKeyFrom("MultipleTypes", "1", "Misc"), "456" },
            { ComplexKeyFrom("MultipleTypes", "1", "Blah"), "2022-02-02" },
            { ComplexKeyFrom("MultipleTypes", "1", "Url"), "http://example.com" },
        };
}

public class ListOfComplexTypeTests : BindingTestBase<ReadOnlyListOfComplexTypes>
{
    protected override void AssertEqualsExpected(ReadOnlyListOfComplexTypes actual)
    {
        List<ComplexType> expected = new()
        {
            new(
                String: "s1",
                StringSet: new HashSet<string>() {"ss_0", "ss_1", "ss_2"},
                MultipleTypes: new HashSet<MultipleTypes>()
                {
                    new("first.foo", 1, true, 1.23m, new DateTime(2001,01,01), new Uri("https://example.com")),
                    new("second.foo", 2, false, 456m, new DateTime(2022, 02, 02), new Uri("https://example.org"))
                })
        };

        actual.ComplexTypes.IsEquivalentTo(expected);
    }

    private static string ComplexKeyFrom(params string[] nested)
    {
        var prefix = new List<string>() { "ComplexTypes", "0" };
        prefix.AddRange(nested);
        return KeyFromNestedProperties(prefix.ToArray());
    }

    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { ComplexKeyFrom("String"), "s1" },
            { ComplexKeyFrom("StringSet", "0"), "ss_0" },
            { ComplexKeyFrom("StringSet", "1"), "ss_1" },
            { ComplexKeyFrom("StringSet", "2"), "ss_2" },
            { ComplexKeyFrom("MultipleTypes", "0", "Foo"), "first.foo" },
            { ComplexKeyFrom("MultipleTypes", "0", "Bar"), "1" },
            { ComplexKeyFrom("MultipleTypes", "0", "Other"), "true" },
            { ComplexKeyFrom("MultipleTypes", "0", "Misc"), "1.23" },
            { ComplexKeyFrom("MultipleTypes", "0", "Blah"), "2001-01-01" },
            { ComplexKeyFrom("MultipleTypes", "0", "Url"), "http://example.com" },
            { ComplexKeyFrom("MultipleTypes", "1", "Foo"), "second.foo" },
            { ComplexKeyFrom("MultipleTypes", "1", "Bar"), "2" },
            { ComplexKeyFrom("MultipleTypes", "1", "Other"), "false" },
            { ComplexKeyFrom("MultipleTypes", "1", "Misc"), "456" },
            { ComplexKeyFrom("MultipleTypes", "1", "Blah"), "2022-02-02" },
            { ComplexKeyFrom("MultipleTypes", "1", "Url"), "http://example.com" },
        };
}

public record ReadOnlyListOfLists(IReadOnlyList<IReadOnlyList<string>> Lists);
public record ReadOnlyListOfSets(IReadOnlyList<IReadOnlySet<int>> Sets);

public record ListOfMultipleTypes(List<MultipleTypes> MultipleTypes);
public record ReadOnlyListOfMultipleTypes(IReadOnlyList<MultipleTypes> MultipleTypes); // this one's broken.

public record ComplexType(string String, IReadOnlySet<string> StringSet, IReadOnlySet<MultipleTypes> MultipleTypes);

public record ListOfComplexTypes(List<ComplexType> ComplexTypes);
public record ReadOnlyListOfComplexTypes(IReadOnlyList<ComplexType> ComplexTypes); // this one's broken.
