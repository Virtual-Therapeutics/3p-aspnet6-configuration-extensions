using System;
using System.Collections.Generic;
using System.Linq;
using ConfigurationExtensions.Tests;
using FluentAssertions;
using Xunit;

namespace ConfigurationExtensions.Collection.Tests;

public abstract class IntCollectionTestBase<T> : BindingTestBase<T>
{
    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { NthKeyFromNestedProperties(0, "Ints"), "123" },
            { NthKeyFromNestedProperties(1, "Ints"), "456" }
        };
}

public class ListOfIntTests : IntCollectionTestBase<ReadOnlyListOfInt>
{
    protected override void AssertEqualsExpected(ReadOnlyListOfInt actual)
    {
        var expected = new List<int>() { 123, 456 };
        Assert.True(expected.SequenceEqual(actual.Ints));
    }
}

public class SetOfIntTests : IntCollectionTestBase<ReadOnlySetOfInt>
{
    protected override void AssertEqualsExpected(ReadOnlySetOfInt actual)
    {
        var expected = new HashSet<int>() { 123, 456 };
        Assert.True(expected.SetEquals(actual.Ints));
    }
}

public class CollectionOfIntTests : IntCollectionTestBase<ReadOnlyCollectionOfInt>
{
    protected override void AssertEqualsExpected(ReadOnlyCollectionOfInt actual)
    {
        var expected = new List<int>() { 456, 123 };

        // IEnumerable makes no explicit promises about sequence
        actual.Ints.Should()
            .HaveCount(expected.Count).And
            .OnlyContain(i => expected.Contains(i));
    }
}

public class EnumerableOfIntTests : IntCollectionTestBase<IEnumerableOfInt>
{
    protected override void AssertEqualsExpected(IEnumerableOfInt actual)
    {
        var expected = new List<int>() { 456, 123 };

        // IEnumerable makes no explicit promises about sequence
        actual.Ints.Should()
            .HaveCount(expected.Count).And
            .OnlyContain(i => expected.Contains(i));
    }
}

public abstract class StringCollectionTestBase<T> : BindingTestBase<T>
{
    protected override Dictionary<string, string> CreateConfig() =>
        new()
        {
            { NthKeyFromNestedProperties(0, "Strings"), "hello" },
            { NthKeyFromNestedProperties(1, "Strings"), "world" }
        };
}

public class ListOfStringTests : StringCollectionTestBase<ReadOnlyListOfString>
{
    protected override void AssertEqualsExpected(ReadOnlyListOfString actual)
    {
        var expected = new List<string>() { "hello", "world" };
        Assert.True(expected.SequenceEqual(actual.Strings));
    }
}

public class SetOfStringTests : StringCollectionTestBase<ReadOnlySetOfString>
{
    protected override void AssertEqualsExpected(ReadOnlySetOfString actual)
    {
        var expected = new HashSet<string>() { "world", "hello" };
        Assert.True(expected.SetEquals(actual.Strings));
    }
}

public class CollectionOfStringTests : StringCollectionTestBase<ReadOnlyCollectionOfString>
{
    protected override void AssertEqualsExpected(ReadOnlyCollectionOfString actual)
    {
        var expected = new List<string>() { "world", "hello" };

        // IEnumerable makes no explicit promises about sequence,
        // and EquivalentTo respects that.
        actual.Strings.Should().BeEquivalentTo(expected);
    }
}

public class EnumerableOfStringTests : StringCollectionTestBase<IEnumerableOfString>
{
    protected override void AssertEqualsExpected(IEnumerableOfString actual)
    {
        var expected = new List<string>() { "world", "hello" };

        // IEnumerable makes no explicit promises about sequence,
        // and EquivalentTo respects that.
        actual.Strings.Should().BeEquivalentTo(expected);
    }
}

// Collection Records
public record ReadOnlyListOfInt(IReadOnlyList<int> Ints);
public record ReadOnlySetOfInt(IReadOnlySet<int> Ints);
public record ReadOnlyCollectionOfInt(IReadOnlyCollection<int> Ints);
public record IEnumerableOfInt(IEnumerable<int> Ints);

public record ReadOnlyListOfString(IReadOnlyList<string> Strings);
public record ReadOnlySetOfString(IReadOnlySet<string> Strings);
public record ReadOnlyCollectionOfString(IReadOnlyCollection<string> Strings);
public record IEnumerableOfString(IEnumerable<string> Strings);
