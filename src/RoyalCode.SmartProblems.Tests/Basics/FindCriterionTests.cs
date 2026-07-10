using RoyalCode.SmartProblems.Entities;
using RoyalCode.SmartProblems;
using RoyalCode.SmartProblems.Tests.UseCases;
using System.ComponentModel;

namespace RoyalCode.SmartProblems.Tests.Basics;

public class FindCriterionTests
{
    [Fact]
    public void Problem_ZeroCriteria_UsesGenericMessage()
    {
        // Act
        var findResult = FindResult<Foo>.Problem(ReadOnlySpan<FindCriterion>.Empty);
        var notFound = findResult.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record for 'Foo' was not found", problem.Detail);

        Assert.NotNull(problem.Extensions);
        problem.Extensions.TryGetValue("entity", out var entityValue);
        Assert.Equal("Foo", (string)entityValue!);
    }

    [Fact]
    public void Problem_SingleCriterion_MatchesLegacyFormat()
    {
        // Arrange
        FindCriterion[] criteria = [new FindCriterion("Value", 1, "Value")];

        // Act
        var findResult = FindResult<Foo>.Problem(criteria);
        var notFound = findResult.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'Foo' with Value '1' was not found", problem.Detail);
    }

    [Fact]
    public void Problem_MultipleCriteria_ListsAllInDeclarationOrder()
    {
        // Arrange
        FindCriterion[] criteria =
        [
            new FindCriterion("Name", "Blumenau", "Name"),
            new FindCriterion("Value", 5, "Value"),
        ];

        // Act
        var findResult = FindResult<Foo>.Problem(criteria);
        var notFound = findResult.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'Foo' with Name 'Blumenau', Value '5' was not found", problem.Detail);

        Assert.NotNull(problem.Extensions);
        problem.Extensions.TryGetValue("Name", out var nameValue);
        Assert.Equal("Blumenau", (string)nameValue!);
        problem.Extensions.TryGetValue("Value", out var valueValue);
        Assert.Equal(5, (int)valueValue!);
    }

    [Fact]
    public void Problem_ByNameNull_ResolvesViaDisplayNames()
    {
        // Arrange: Foo.Value has no [DisplayName], so DisplayNames falls back to the raw property name.
        FindCriterion[] criteria = [new FindCriterion("Value", 42)];

        // Act
        var findResult = FindResult<Foo>.Problem(criteria);
        var notFound = findResult.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'Foo' with Value '42' was not found", problem.Detail);
    }

    [Fact]
    public void Problem_ByNameWhitespace_TreatedAsUnspecified()
    {
        // Arrange
        FindCriterion[] criteria = [new FindCriterion("Value", 42, byName: "   ")];

        // Act
        var findResult = FindResult<Foo>.Problem(criteria);
        var notFound = findResult.NotFound(out var problem);

        // Assert: falls back to DisplayNames resolution instead of a blank name.
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'Foo' with Value '42' was not found", problem.Detail);
    }

    [Fact]
    public void Problem_DisplayNameAttribute_UsedInDetail()
    {
        // Arrange
        FindCriterion[] criteria = [new FindCriterion(nameof(SampleEntity.Name), "Blumenau")];

        // Act
        var findResult = FindResult<SampleEntity>.Problem(criteria);
        var notFound = findResult.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'The Sample Entity' with Sample Name 'Blumenau' was not found", problem.Detail);
    }

    [Fact]
    public void Problem_DuplicatePropertyName_DetailListsBoth_ExtensionsLastWins()
    {
        // Arrange
        FindCriterion[] criteria =
        [
            new FindCriterion("Value", 1, "Value"),
            new FindCriterion("Value", 2, "Value"),
        ];

        // Act
        var findResult = FindResult<Foo>.Problem(criteria);
        var notFound = findResult.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'Foo' with Value '1', Value '2' was not found", problem.Detail);

        Assert.NotNull(problem.Extensions);
        problem.Extensions.TryGetValue("Value", out var value);
        Assert.Equal(2, (int)value!);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_RejectsInvalidPropertyName(string? propertyName)
    {
        // ArgumentException.ThrowIfNullOrWhiteSpace throws ArgumentNullException for null,
        // ArgumentException for empty/whitespace — ThrowsAny accepts either.
        Assert.ThrowsAny<ArgumentException>(() => new FindCriterion(propertyName!, 1));
    }

    [Fact]
    public void Problem_DefaultCriterion_IsIgnored()
    {
        // Arrange
        FindCriterion[] criteria = [default, new FindCriterion("Value", 1, "Value")];

        // Act
        var findResult = FindResult<Foo>.Problem(criteria);
        var notFound = findResult.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'Foo' with Value '1' was not found", problem.Detail);
    }

    [Fact]
    public void Problem_AllDefaultCriteria_FallsBackToZeroCriteriaCase()
    {
        // Arrange
        FindCriterion[] criteria = [default, default];

        // Act
        var findResult = FindResult<Foo>.Problem(criteria);
        var notFound = findResult.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record for 'Foo' was not found", problem.Detail);
    }
}

[DisplayName("The Sample Entity")]
internal class SampleEntity
{
    public int Id { get; set; }

    [DisplayName("Sample Name")]
    public string? Name { get; set; }
}
