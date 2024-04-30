using RoyalCode.SmartProblems.Convertions.Internals;

namespace RoyalCode.SmartProblems.Tests.Convertions;

public class PointerParserTests
{

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("#", "")]
    [InlineData("/", "")]
    [InlineData("#/", "")]
    [InlineData("#/a", "a")]
    [InlineData("#/a/b", "a.b")]
    [InlineData("#/a/0", "a[0]")]
    [InlineData("#/a/0/", "a[0]")]
    [InlineData("#/a/0/b", "a[0].b")]
    [InlineData("#/a/0/b/1", "a[0].b[1]")]
    [InlineData("#/a/0/b/1/", "a[0].b[1]")]
    [InlineData("#/a/0/b/1/c", "a[0].b[1].c")]
    [InlineData("#/0", "[0]")]
    [InlineData("#/0/", "[0]")]
    [InlineData("#/0/a", "[0].a")]
    [InlineData("#/0/a/", "[0].a")]
    [InlineData("#/0/a/b", "[0].a.b")]
    [InlineData("/a", "a")]
    [InlineData("/a/b", "a.b")]
    [InlineData("/a/b/0", "a.b[0]")]
    [InlineData("a", "a")]
    [InlineData("a/b", "a.b")]
    [InlineData("a/0", "a[0]")]
    [InlineData("a/10", "a[10]")]
    [InlineData("a/10/b", "a[10].b")]
    [InlineData("a/10/b/", "a[10].b")]
    public void PointerToProperty(string pointer, string expected)
    {
        var property = pointer.PointerToProperty();

        Assert.Equal(expected, property);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "#/")]
    [InlineData("a", "#/a")]
    [InlineData("a.b", "#/a/b")]
    [InlineData("a[0]", "#/a/0")]
    [InlineData("a[0].b", "#/a/0/b")]
    [InlineData("a[0].b[1]", "#/a/0/b/1")]
    [InlineData("a[0].b[1].c", "#/a/0/b/1/c")]
    [InlineData("[0]", "#/0")]
    [InlineData("[0].a", "#/0/a")]
    [InlineData("[0].a.b", "#/0/a/b")]
    public void PropertyToPointer(string property, string expected)
    {
        var pointer = property.PropertyToPointer();

        Assert.Equal(expected, pointer);
    }   
}
