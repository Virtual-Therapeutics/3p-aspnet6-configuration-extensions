using System;
using System.Collections.Generic;
using ConfigurationExtensions.Tests;
using FluentAssertions;
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
    [SkippableFact]
    public override void Binds()
        => Ignore.IfThrows(() => base.Binds(), "Parsing a record of IReadOnlyList<>s of IReadOnlySet<>s throws");

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

// We're just going to use the same data for all of these, since the error is in the initial parsing
// and the boilerplate was a bit much
public abstract class EnumerableOfMultipleBaseTest<T> : BindingTestBase<T>
{
    protected List<MultipleTypes> Expected { get; private init; } =
        new()
        {
            new("first.foo", 1, true, 1.23m, new DateTime(2001,01,01), new Uri("https://example.com")),
            new("second.foo", 2, false, 456m, new DateTime(2022, 02, 02), new Uri("https://example.org"))
        };

    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { KeyFromNestedProperties("MultipleTypes","0", "Foo"), "first.foo" },
            { KeyFromNestedProperties("MultipleTypes","0", "Bar"), "1" },
            { KeyFromNestedProperties("MultipleTypes","0", "Other"), bool.TrueString },
            { KeyFromNestedProperties("MultipleTypes","0", "Misc"), "1.23" },
            { KeyFromNestedProperties("MultipleTypes","0", "Blah"), "2001-01-01" },
            { KeyFromNestedProperties("MultipleTypes","0", "Url"), "https://example.com" },
            { KeyFromNestedProperties("MultipleTypes","1", "Foo"), "second.foo" },
            { KeyFromNestedProperties("MultipleTypes","1", "Bar"), "2" },
            { KeyFromNestedProperties("MultipleTypes","1", "Other"), bool.FalseString },
            { KeyFromNestedProperties("MultipleTypes","1", "Misc"), "456" },
            { KeyFromNestedProperties("MultipleTypes","1", "Blah"), "2022-02-02" },
            { KeyFromNestedProperties("MultipleTypes","1", "Url"), "https://example.org" },
        };
}

public class MutableListOfMultipleTypesTests : EnumerableOfMultipleBaseTest<ListOfMultipleTypes>
{
    protected override void AssertEqualsExpected(ListOfMultipleTypes actual)
    {
        actual.MultipleTypes.Should().BeEquivalentTo(Expected);
    }
}

public class ListOfMultipleTypesTests : EnumerableOfMultipleBaseTest<ReadOnlyListOfMultipleTypes>
{
    [SkippableFact]
    public override void Binds()
    {
        Ignore.IfThrows(() => base.Binds(), "Parsing a record of an IReadOnlyList<> of records throws");
    }

    protected override void AssertEqualsExpected(ReadOnlyListOfMultipleTypes actual)
    {
        actual.MultipleTypes.Should().BeEquivalentTo(Expected);
    }
}




// We're just going to use the same data for all of these, since the error is in the initial parsing.
// and the boilerplate was overwhelming.
public abstract class EnumerableOfComplexBaseTest<T> : BindingTestBase<T>
{
    protected (string String, HashSet<string> StringSet, HashSet<MultipleTypes> MultipleTypes) Expected { get; private init; } =
        (
            String: "s1",
            StringSet: new HashSet<string>() { "ss_0", "ss_1", "ss_2" },
            MultipleTypes: new HashSet<MultipleTypes>()
                {
                    new("first.foo", 1, true, 1.23m, new DateTime(2001,01,01), new Uri("https://example.com")),
                    new("second.foo", 2, false, 456m, new DateTime(2022, 02, 02), new Uri("https://example.org"))
                }
        );

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

public class ListOfComplexTypeTests : EnumerableOfComplexBaseTest<ListOfComplexTypes>
{
    [SkippableFact]
    public override void Binds()
        => Ignore.IfThrows(() => base.Binds(), "Parsing a record of an IReadOnlyList<> of records with an IReadOnlySet<> throws");

    protected override void AssertEqualsExpected(ListOfComplexTypes actual)
    {
        (string s, HashSet<string> ss, HashSet<MultipleTypes> m) = Expected;
        var expected = new List<ComplexType>() { new(s, ss, m) };

        actual.ComplexTypes.IsEquivalentTo(expected);
    }
}

public class ListOfMutableComplexTypeTests : EnumerableOfComplexBaseTest<ListOfMutableComplexTypes>
{
    [SkippableFact]
    public override void Binds()
        => Ignore.IfThrows(() => base.Binds(), "Parsing a record of a List<> of records with a HashSet<> throws");

    protected override void AssertEqualsExpected(ListOfMutableComplexTypes actual)
    {
        (string s, HashSet<string> ss, HashSet<MultipleTypes> m) = Expected;
        var expected = new List<MutableComplexType>() { new(s, ss, m) };

        actual.ComplexTypes.IsEquivalentTo(expected);
    }
}

public class MutableListOfComplexTypeTests : EnumerableOfComplexBaseTest<MutableListOfComplexTypes>
{
    [SkippableFact]
    public override void Binds()
        => Ignore.IfThrows(() => base.Binds(), "Parsing a record of a List<> of records with an IReadOnlySet<> throws");
    protected override void AssertEqualsExpected(MutableListOfComplexTypes actual)
    {
        (string s, HashSet<string> ss, HashSet<MultipleTypes> m) = Expected;
        var expected = new List<ComplexType>() { new(s, ss, m) };

        actual.ComplexTypes.IsEquivalentTo(expected);
    }
}

public class MutableListOfMutableComplexTypeTests : EnumerableOfComplexBaseTest<MutableListOfMutableComplexTypes>
{
    protected override void AssertEqualsExpected(MutableListOfMutableComplexTypes actual)
    {
        (string s, HashSet<string> ss, HashSet<MultipleTypes> m) = Expected;
        var expected = new List<MutableComplexType>() { new(s, ss, m) };

        actual.ComplexTypes.IsEquivalentTo(expected);
    }
}


public record ReadOnlyListOfLists(IReadOnlyList<IReadOnlyList<string>> Lists);
public record ReadOnlyListOfSets(IReadOnlyList<IReadOnlySet<int>> Sets);

public record ListOfMultipleTypes(List<MultipleTypes> MultipleTypes);
public record ReadOnlyListOfMultipleTypes(IReadOnlyList<MultipleTypes> MultipleTypes); // this one's broken.



public record ComplexType(string String, IReadOnlySet<string> StringSet, IReadOnlySet<MultipleTypes> MultipleTypes);
public record MutableComplexType(string String, HashSet<string> StringSet, HashSet<MultipleTypes> MultipleTypes);

// These all fail:
public record ListOfComplexTypes(IReadOnlyList<ComplexType> ComplexTypes);
public record ListOfMutableComplexTypes(IReadOnlyList<MutableComplexType> ComplexTypes);
public record MutableListOfComplexTypes(List<ComplexType> ComplexTypes);

// This one works, though:
public record MutableListOfMutableComplexTypes(List<MutableComplexType> ComplexTypes);
