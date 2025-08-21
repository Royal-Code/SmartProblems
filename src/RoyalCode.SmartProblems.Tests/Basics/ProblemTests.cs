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
