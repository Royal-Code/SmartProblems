using System.Net;
using RoyalCode.SmartProblems.Descriptions;
using RoyalCode.SmartProblems.Descriptions.Documentation;

namespace RoyalCode.SmartProblems.Tests.Descriptions;

public class ProblemDetailsDescriptionPageModelTests
{
    [Fact]
    public void Create_IncludesGenericDefaultAndCustomDescriptions()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription(
            "my-custom-type",
            "My custom problem type",
            "A custom description.",
            HttpStatusCode.Conflict));

        // Act
        var model = ProblemDetailsDescriptionPageModel.Create(options, "https://example.test/.problems");

        // Assert
        Assert.Contains(model.Items, item => item.TypeId == "not-found");
        Assert.Contains(model.Items, item => item.TypeId == "invalid-parameter");
        Assert.Contains(model.Items, item => item.TypeId == "my-custom-type");
    }

    [Fact]
    public void Create_UsesPageUriAsBaseWhenBaseAddressIsDefault()
    {
        // Arrange
        var options = new ProblemDetailsOptions();

        // Act
        var model = ProblemDetailsDescriptionPageModel.Create(options, "https://example.test/.problems");
        var item = Assert.Single(model.Items, item => item.TypeId == "aggregate-problems-details");

        // Assert
        Assert.True(item.UsesGeneratedTypeUri);
        Assert.Equal("https://example.test/.problems#aggregate-problems-details", item.ResolvedTypeUri);
    }

    [Fact]
    public void Create_UsesPageUriAsBaseWhenBaseAddressWasGeneratedFromDefaultRoute()
    {
        // Arrange
        var options = new ProblemDetailsOptions
        {
            BaseAddress = "https://example.test/.problems"
        };

        // Act
        var model = ProblemDetailsDescriptionPageModel.Create(options, "http://example.test/problems/catalog");
        var item = Assert.Single(model.Items, item => item.TypeId == "aggregate-problems-details");

        // Assert
        Assert.True(item.UsesGeneratedTypeUri);
        Assert.Equal("http://example.test/problems/catalog#aggregate-problems-details", item.ResolvedTypeUri);
    }

    [Fact]
    public void Create_UsesConfiguredBaseAddressWhenItIsCustom()
    {
        // Arrange
        var options = new ProblemDetailsOptions
        {
            BaseAddress = "https://docs.example.test/problems/",
            TypeComplement = string.Empty
        };

        // Act
        var model = ProblemDetailsDescriptionPageModel.Create(options, "https://example.test/.problems");
        var item = Assert.Single(model.Items, item => item.TypeId == "aggregate-problems-details");

        // Assert
        Assert.True(item.UsesGeneratedTypeUri);
        Assert.Equal("https://docs.example.test/problems/aggregate-problems-details", item.ResolvedTypeUri);
    }

    [Fact]
    public void Create_PreservesExplicitTypeUri()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription(
            "size-out-of-bounds",
            "https://example.com/problems/size-out-of-bounds",
            "Size out of bounds",
            "The size is outside the accepted range.",
            HttpStatusCode.BadRequest));

        // Act
        var model = ProblemDetailsDescriptionPageModel.Create(options, "https://example.test/.problems");
        var item = Assert.Single(model.Items, item => item.TypeId == "size-out-of-bounds");

        // Assert
        Assert.False(item.UsesGeneratedTypeUri);
        Assert.Equal("https://example.com/problems/size-out-of-bounds", item.ResolvedTypeUri);
    }

    [Fact]
    public void Create_CustomDescriptionOverridesGenericTypeId()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription(
            "not-found",
            "Overridden",
            "The not found description was overridden.",
            (HttpStatusCode)418));

        // Act
        var model = ProblemDetailsDescriptionPageModel.Create(options, "https://example.test/.problems");
        var item = Assert.Single(model.Items, item => item.TypeId == "not-found");

        // Assert
        Assert.Equal("Overridden", item.Title);
        Assert.Equal(418, item.StatusCode);
    }

    [Fact]
    public void Create_NormalizesSectionIdsAndHandlesCollisions()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        options.Descriptor.AddRange([
            new ProblemDetailsDescription(
                "bad id/<x>",
                "Bad id",
                "The id contains characters that need normalization.",
                HttpStatusCode.BadRequest),
            new ProblemDetailsDescription(
                "bad-id-x",
                "Bad id collision",
                "The id collides after normalization.",
                HttpStatusCode.BadRequest)
        ]);

        // Act
        var model = ProblemDetailsDescriptionPageModel.Create(options, "https://example.test/.problems");
        var first = Assert.Single(model.Items, item => item.TypeId == "bad id/<x>");
        var second = Assert.Single(model.Items, item => item.TypeId == "bad-id-x");

        // Assert
        Assert.Equal("problem-bad-id-x", first.SectionId);
        Assert.Equal("problem-bad-id-x-2", second.SectionId);
        Assert.Equal("#problem-bad-id-x", first.NavigationHref);
        Assert.Equal("#problem-bad-id-x-2", second.NavigationHref);
    }
}
