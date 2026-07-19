using RoyalCode.SmartProblems.Entities;
using System.ComponentModel;

namespace RoyalCode.SmartProblems.Tests.Basics;

/// <summary>
/// Tests for <see cref="FindResult{TEntity}.ProjectedFrom{TSource}"/> and
/// <see cref="FindResult{TEntity, TId}.ProjectedFrom{TSource}"/>: a projected result carries a DTO,
/// but the problems must name the original entity, with the correct category in every path.
/// </summary>
public class ProjectedFindResultTests
{
    [Fact]
    public void ProjectedFrom_Found_ReturnsValue_WithoutProblems()
    {
        // Arrange
        var dto = new ProductDetails { Id = 1, Name = "Alpha" };

        // Act
        var result = FindResult<ProductDetails>.ProjectedFrom<Product>(dto, [new FindCriterion("Sku", "SKU-A")]);

        // Assert
        Assert.True(result.Found);
        Assert.Same(dto, result.Entity);
        Assert.False(result.NotFound(out _));
        Assert.False(result.HasInvalidParameter(out _));
    }

    [Fact]
    public void ProjectedFrom_NotFound_NamesEntity_NotDto()
    {
        // Act
        var result = FindResult<ProductDetails>.ProjectedFrom<Product>(null, [new FindCriterion("Sku", "SKU-X")]);

        // Assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal(ProblemCategory.NotFound, problem!.Category);
        Assert.Equal("The record of 'Product' with Sku 'SKU-X' was not found", problem.Detail);
        Assert.Equal("Product", problem.Extensions!["entity"]);
        Assert.Equal("SKU-X", problem.Extensions!["Sku"]);
    }

    [Fact]
    public void ProjectedFrom_HasInvalidParameter_UsesInvalidParameterCategory_AndEntityName()
    {
        // Act
        var result = FindResult<ProductDetails>.ProjectedFrom<Product>(null, [new FindCriterion("Sku", "SKU-X")]);

        // Assert
        Assert.True(result.HasInvalidParameter(out var problem, "sku"));
        Assert.Equal(ProblemCategory.InvalidParameter, problem!.Category);
        Assert.Equal("The record of 'Product' with Sku 'SKU-X' was not found", problem.Detail);
        Assert.Equal("sku", problem.Property);
        Assert.Equal("Product", problem.Extensions!["entity"]);
    }

    [Fact]
    public void ProjectedFrom_MultipleCriteria_ListsAll_InDeclarationOrder()
    {
        // Act
        var result = FindResult<ProductDetails>.ProjectedFrom<Product>(
            null,
            [new FindCriterion("Sku", "SKU-X"), new FindCriterion("Code", 7)]);

        // Assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal("The record of 'Product' with Sku 'SKU-X', Code '7' was not found", problem!.Detail);
        Assert.Equal("SKU-X", problem.Extensions!["Sku"]);
        Assert.Equal(7, problem.Extensions!["Code"]);
    }

    [Fact]
    public void ProjectedFrom_WithoutCriteria_UsesGenericMessage_NamingEntity()
    {
        // Act
        var result = FindResult<ProductDetails>.ProjectedFrom<Product>(null, []);

        // Assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal("The record for 'Product' was not found", problem!.Detail);
        Assert.Equal("Product", problem.Extensions!["entity"]);
    }

    [Fact]
    public void ProjectedFrom_CriterionWithoutByName_ResolvesDisplayNameFromEntity()
    {
        // Arrange: DisplayNameAttribute on the entity property, absent on the DTO
        var result = FindResult<ProductDetails>.ProjectedFrom<Product>(null, [new FindCriterion("Ean", "789")]);

        // Act & Assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal("The record of 'Product' with European Article Number '789' was not found", problem!.Detail);
    }

    [Fact]
    public void ProjectedFrom_CriterionWithByName_IsPreserved()
    {
        // Act
        var result = FindResult<ProductDetails>.ProjectedFrom<Product>(null, [new FindCriterion("Sku", "SKU-X", "Product Code")]);

        // Assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal("The record of 'Product' with Product Code 'SKU-X' was not found", problem!.Detail);
    }

    [Fact]
    public void ProjectedFrom_EntityWithDisplayName_UsesEntityDisplayName()
    {
        // Act
        var result = FindResult<ProductDetails>.ProjectedFrom<Named>(null, []);

        // Assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal("The record for 'Nice Name' was not found", problem!.Detail);
        Assert.Equal(nameof(Named), problem.Extensions!["entity"]);
    }

    [Fact]
    public void ProjectedFrom_NullCriteria_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            FindResult<ProductDetails>.ProjectedFrom<Product>(null, null!));
    }

    [Fact]
    public void ProjectedFrom_ToResult_UsesInvalidParameter_WithEntityName()
    {
        // Arrange
        var result = FindResult<ProductDetails>.ProjectedFrom<Product>(null, [new FindCriterion("Sku", "SKU-X")]);

        // Act
        Result<ProductDetails> converted = result.ToResult("sku");

        // Assert
        Assert.True(converted.HasProblems(out var problems));
        Assert.Single(problems!);
        Assert.Equal(ProblemCategory.InvalidParameter, problems![0].Category);
        Assert.Contains("'Product'", problems[0].Detail);
    }

    [Fact]
    public void ProjectedFromById_Found_ReturnsValue()
    {
        // Arrange
        var dto = new ProductDetails { Id = 5, Name = "Alpha" };

        // Act
        var result = FindResult<ProductDetails, int>.ProjectedFrom<Product>(dto, 5);

        // Assert
        Assert.True(result.Found);
        Assert.Same(dto, result.Entity);
        Assert.False(result.NotFound(out _));
    }

    [Fact]
    public void ProjectedFromById_NotFound_NamesEntity_WithIdData()
    {
        // Act
        var result = FindResult<ProductDetails, int>.ProjectedFrom<Product>(null, 5);

        // Assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal(ProblemCategory.NotFound, problem!.Category);
        Assert.Equal("The record of 'Product' with id '5' was not found", problem.Detail);
        Assert.Equal("Product", problem.Extensions!["entity"]);
        Assert.Equal(5, problem.Extensions!["id"]);
    }

    [Fact]
    public void ProjectedFromById_HasInvalidParameter_UsesInvalidParameterCategory_AndEntityName()
    {
        // Act
        var result = FindResult<ProductDetails, int>.ProjectedFrom<Product>(null, 5);

        // Assert
        Assert.True(result.HasInvalidParameter(out var problem, "id"));
        Assert.Equal(ProblemCategory.InvalidParameter, problem!.Category);
        Assert.Equal("The record of 'Product' with id '5' was not found", problem.Detail);
        Assert.Equal("id", problem.Property);
        Assert.Equal("Product", problem.Extensions!["entity"]);
    }

    [Fact]
    public void ProjectedFromById_NonProjected_KeepsCurrentBehavior_NamingDtoType()
    {
        // Arrange: the plain constructor keeps naming the generic type (the DTO), unchanged.
        var result = new FindResult<ProductDetails, int>(null, 5);

        // Act & Assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal("The record of 'ProductDetails' with id '5' was not found", problem!.Detail);
        Assert.Equal(nameof(ProductDetails), problem.Extensions!["entity"]);
    }

    [Fact]
    public void FindResult_ConstructedWithProblem_Quirk_HasInvalidParameterReturnsStoredProblem()
    {
        // Arrange: current, documented quirk â€” a FindResult constructed with a pre-built Problem
        // returns that same problem from HasInvalidParameter, keeping its original category.
        // This is why projected results carry a descriptor instead of a pre-built problem.
        var stored = Problems.NotFound("stored not-found problem");
        var result = new FindResult<ProductDetails>(stored);

        // Act & Assert
        Assert.True(result.HasInvalidParameter(out var problem, "sku"));
        Assert.Same(stored, problem);
        Assert.Equal(ProblemCategory.NotFound, problem!.Category);
    }

    #region models

    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;

        [DisplayName("European Article Number")]
        public string? Ean { get; set; }

        public int Code { get; set; }
    }

    [DisplayName("Nice Name")]
    public class Named
    {
        public int Id { get; set; }
    }

    public class ProductDetails
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    #endregion
}
