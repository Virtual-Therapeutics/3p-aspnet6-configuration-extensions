#if false


[Fact]
public void MutableListOfComplexTypes_Binds()
{
    const string typeName = nameof(ListOfComplexTypes);
    const string propertyName = nameof(ListOfComplexTypes.ComplexTypes);
    const string firstItem = $"{typeName}:{propertyName}:0";

    var configurationManager = new ConfigurationManager();
    configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.String)}", "s1"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.StringSet)}:0", "ss_0"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.StringSet)}:1", "ss_1"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.StringSet)}:2", "ss_2"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Foo", "first.foo"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Bar", "1"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Other", "true"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Misc", "1.23"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Blah", "2001-01-01"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Url", "http://example.com"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Foo", "second.foo"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Bar", "2"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Other", "false"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Misc", "456"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Blah", "2022-02-02"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Url", "http://example.com"),
        });

    var result = configurationManager.Bind<ListOfComplexTypes>();
    Assert.Single(result.ComplexTypes);
    var complexType = result.ComplexTypes.First();

    Assert.Equal("s1", complexType.String);
    Assert.True((new HashSet<string>() { "ss_0", "ss_1", "ss_2" }).SetEquals(complexType.StringSet));
    Assert.True((new HashSet<MultipleTypes>() {
            new ("first.foo", 1, true, 1.23m, new DateTime(2001, 01, 01), new Uri("http://example.com")),
            new ("second.foo", 2, false, 456m, new DateTime(2022, 02, 02), new Uri("http://example.org")),
        }).SetEquals(complexType.MultipleTypes));
}

[Fact]
public void ListOfComplexTypes_Binds()
{
    const string typeName = nameof(ReadOnlyListOfComplexTypes);
    const string propertyName = nameof(ReadOnlyListOfComplexTypes.ComplexTypes);
    const string firstItem = $"{typeName}:{propertyName}:0";

    var configurationManager = new ConfigurationManager();
    configurationManager.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.String)}", "s1"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.StringSet)}:0", "ss_0"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.StringSet)}:1", "ss_1"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.StringSet)}:2", "ss_2"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Foo", "first.foo"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Bar", "1"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Other", "true"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Misc", "1.23"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Blah", "2001-01-01"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:0:Url", "http://example.com"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Foo", "second.foo"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Bar", "2"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Other", "false"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Misc", "456"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Blah", "2022-02-02"),
            new KeyValuePair<string, string>($"{firstItem}:{nameof(ComplexType.MultipleTypes)}:1:Url", "http://example.com"),
        });

    var result = configurationManager.Bind<ReadOnlyListOfComplexTypes>();
    Assert.Single(result.ComplexTypes);
    var complexType = result.ComplexTypes.First();

    Assert.Equal("s1", complexType.String);
    Assert.True((new HashSet<string>() { "ss_0", "ss_1", "ss_2" }).SetEquals(complexType.StringSet));
    Assert.True((new HashSet<MultipleTypes>() {
            new ("first.foo", 1, true, 1.23m, new DateTime(2001, 01, 01), new Uri("http://example.com")),
            new ("second.foo", 2, false, 456m, new DateTime(2022, 02, 02), new Uri("http://example.org")),
        }).SetEquals(complexType.MultipleTypes));
}

}

public class NoPublicConstructor
{
    private NoPublicConstructor()
    {
    }
}
#endif
