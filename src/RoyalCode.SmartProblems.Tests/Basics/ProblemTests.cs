namespace RoyalCode.SmartProblems.Tests.Basics;

public class ProblemTests
{
    [Fact]
    public void Problem_ChainProperty_MustConcatProperties()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter", "my-property");

        // Act
        problem.ChainProperty("my-other-property");

        // Assert
        Assert.Equal("my-other-property.my-property", problem.Property);
    }

    [Fact]
    public void Problem_ReplaceProperty_MustReplace()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter", "my-property");

        // Act
        problem.ReplaceProperty("new-property");

        // Assert
        Assert.Equal("new-property", problem.Property);
    }

    [Fact]
    public void Problem_ReplaceProperty_WithNull_MustSetNull()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter", "my-property");

        // Act
        problem.ReplaceProperty(null);

        // Assert
        Assert.Null(problem.Property);
    }

    [Fact]
    public void Problem_ReplaceProperty_WithEmpty_MustSetEmpty()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter", "my-property");

        // Act
        problem.ReplaceProperty(string.Empty);

        // Assert
        Assert.Equal(string.Empty, problem.Property);
    }

    [Fact]
    public void Problem_OperatorPlus_MustAggregateInProblems()
    {
        // Arrange
        var p1 = Problems.InvalidParameter("p1");
        var p2 = Problems.NotFound("p2");

        // Act
        var aggregated = p1 + p2;

        // Assert
        Assert.NotNull(aggregated);
        Assert.Equal(2, aggregated.Count);
        Assert.Same(p1, aggregated[0]);
        Assert.Same(p2, aggregated[1]);
    }

    [Fact]
    public void Problem_ToString_Default_OnlyDetailAndCategory()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter");

        // Act
        var text = problem.ToString();

        // Assert
        Assert.Contains("Category: InvalidParameter", text);
        Assert.Contains("Details: Invalid parameter", text);
        Assert.DoesNotContain("Extensions:", text);
    }

    [Fact]
    public void Problem_ToString_Default_WithPropertyAndTypeId()
    {
        // Arrange
        var problem = new Problem
        {
            Detail = "Invalid parameter",
            Category = ProblemCategory.InvalidParameter,
            TypeId = "MyType",
            Property = "my-property"
        };

        // Act
        var text = problem.ToString();

        // Assert
        Assert.Contains("Property: my-property", text);
        Assert.Contains("TypeId: MyType", text);
    }

    [Fact]
    public void Problem_ToString_Default_WithExtensions()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter");
        problem.With("key1", "value1").With("key2", 2);

        // Act
        var text = problem.ToString();

        // Assert
        Assert.Contains("Extensions: {", text);
        Assert.Contains("key1: value1", text);
        Assert.Contains("key2: 2", text);
    }

    [Fact]
    public void Problem_ToStringFactory_Custom_MustUseFactory()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter");
        problem.ToStringFactory = p => $"{p.Category}:{p.Detail}";

        // Act
        var text = problem.ToString();

        // Assert
        Assert.Equal("InvalidParameter:Invalid parameter", text);
    }

    [Fact]
    public void Problem_With_ShouldInitializeExtensions_AndOverwriteKey()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter");

        // Act
        problem.With("k", "v1");
        problem.With("k", "v2");

        // Assert
        Assert.NotNull(problem.Extensions);
        Assert.Single(problem.Extensions);
        Assert.Equal("v2", problem.Extensions["k"]);
    }

    [Fact]
    public void Problem_With_MultipleKeys_MustStoreAll()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter");

        // Act
        problem.With("k1", "v1");
        problem.With("k2", 123);

        // Assert
        Assert.NotNull(problem.Extensions);
        Assert.Equal(2, problem.Extensions.Count);
        Assert.Equal("v1", problem.Extensions["k1"]);
        Assert.Equal(123, problem.Extensions["k2"]);
    }

    [Fact]
    public void Problem_Extensions_StartsNull_BecomesNotNullAfterWith()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter");

        // Act
        var wasNull = problem.Extensions is null;
        problem.With("k", "v");

        // Assert
        Assert.True(wasNull);
        Assert.NotNull(problem.Extensions);
    }

    [Fact]
    public void Problem_ChainProperty_WithNullParent_MustDoNothing()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter", "child");

        // Act
        problem.ChainProperty(null);

        // Assert
        Assert.Equal("child", problem.Property);
    }

    [Fact]
    public void Problem_ChainProperty_WithEmptyParent_MustDoNothing()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter", "child");

        // Act
        problem.ChainProperty(string.Empty);

        // Assert
        Assert.Equal("child", problem.Property);
    }

    [Fact]
    public void Problem_ChainProperty_Index_Negative_MustConcatTextually()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter", "item");

        // Act
        problem.ChainProperty("list", -1);

        // Assert
        Assert.Equal("list[-1].item", problem.Property);
    }

    [Fact]
    public void Problem_ChainProperty_WithNullProperty_MustDoNothing()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter");

        // Act
        problem.ChainProperty("my-property");

        // Assert
        Assert.Null(problem.Property);
    }

    [Fact]
    public void Problem_ChainProperty_WithEmptyProperty_MustDoNothing()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter", string.Empty);

        // Act
        problem.ChainProperty("my-property");

        // Assert
        Assert.Equal(string.Empty, problem.Property);
    }

    [Fact]
    public void Problem_ChainProperty_WithIndex_MustConcatProperties()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter", "my-property");

        // Act
        problem.ChainProperty("my-other-property", 1);

        // Assert
        Assert.Equal("my-other-property[1].my-property", problem.Property);
    }

    [Fact]
    public void Problem_ChainProperty_WithIndexAndNullProperty_MustDoNothing()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter");

        // Act
        problem.ChainProperty("my-property", 1);

        // Assert
        Assert.Null(problem.Property);
    }

    [Fact]
    public void Problem_ChainProperty_WithIndexAndEmptyProperty_MustDoNothing()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter", string.Empty);

        // Act
        problem.ChainProperty("my-property", 1);

        // Assert
        Assert.Equal(string.Empty, problem.Property);
    }

    [Fact]
    public void Problem_With_MustAddExtension()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter");

        // Act
        problem.With("my-key", "my-value");

        // Assert
        Assert.NotNull(problem.Extensions);
        Assert.Single(problem.Extensions);
        Assert.Equal("my-value", problem.Extensions["my-key"]);
    }

    [Fact]
    public void Problem_WithEnum_MustAddExtension()
    {
        // Arrange
        var problem = Problems.InvalidParameter("Invalid parameter");

        // Act
        problem.With("my-key", MyEnum.Value1);

        // Assert
        Assert.NotNull(problem.Extensions);
        Assert.Single(problem.Extensions);
        Assert.Equal("Value1", problem.Extensions["my-key"]);
    }

    [Fact]
    public void Problem_Implicit_Task()
    {
        // Arrange
        Problem problem = Problems.InvalidParameter("Invalid parameter");
        Task<Result> task;

        // Act
        task = problem;

        // Assert
        Assert.NotNull(task);
        var result = task.Result;
        Assert.True(result.HasProblems(out var resultProblems));
        Assert.NotNull(resultProblems);
        Assert.Single(resultProblems);
    }

    [Fact]
    public void Problem_AsResult()
    {
        // Arrange
        Problem problem = Problems.InvalidParameter("Invalid parameter");
        Result result;
        Task<Result> task;

        // Act
        task = problem.AsResult();

        // Assert
        Assert.NotNull(task);
        result = task.Result;
        Assert.True(result.HasProblems(out var resultProblems));
        Assert.NotNull(resultProblems);
        Assert.Single(resultProblems);
    }

    [Fact]
    public void Problem_AsResultValue()
    {
        // Arrange
        Problem problem = Problems.InvalidParameter("Invalid parameter");
        Result<int> result;
        Task<Result<int>> task;

        // Act
        task = problem.AsResult<int>();

        // Assert
        Assert.NotNull(task);
        result = task.Result;
        Assert.True(result.HasProblems(out var resultProblems));
        Assert.NotNull(resultProblems);
        Assert.Single(resultProblems);
    }
}

file enum MyEnum
{
    Value1,
    Value2
}
