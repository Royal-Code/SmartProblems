
namespace RoyalCode.SmartProblems.Tests.Basics;

public class ResultTests
{
    [Fact]
    public void Result_Ok_Must_Be_Successful()
    {
        // Arrange
        var result = Result.Ok();

        // Act
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.False(hasProblems);
        Assert.Null(problems);
    }

    [Fact]
    public void Result_Implicit_Problem()
    {
        // Arrange
        Result result;
        Problem problem = Problems.InvalidParameter("invalid");

        // Act
        result = problem;

        // Assert
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Same(problem, problems[0]);
    }

    [Fact]
    public void Result_Implict_Problems()
    {
        // Arrange
        Result result;
        Problems problems = [Problems.InvalidParameter("invalid")];

        // Act
        result = problems;

        // Assert
        Assert.True(result.HasProblems(out var resultProblems));
        Assert.NotNull(resultProblems);
        Assert.Single(resultProblems);
        Assert.Same(problems[0], resultProblems[0]);
    }

    [Fact]
    public void Result_Implict_Problems_Must_Throw_When_Null()
    {
        // Arrange
        Result result;
        Problems? problems = null;

        // Act
        Action act = () => result = problems!;

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Result_Implict_Exception()
    {
        // Arrange
        Result result;
        Exception exception = new("Error");

        // Act
        result = exception;

        // Assert
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Equal("Error", problems[0].Detail);
    }

    [Fact]
    public void Result_Add_Problem_Must_HasProblems()
    {
        // Arrange
        var result = Result.Ok();

        // Act
        result += Problems.InvalidParameter("invalid");

        // Assert
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Equal("invalid", problems[0].Detail);
    }

    [Fact]
    public void Result_Add_Problems_Must_HasProblems()
    {
        // Arrange
        var result = Result.Ok();
        Problems problems = [Problems.InvalidParameter("invalid"), Problems.NotFound("not found")];

        // Act
        result += problems;

        // Assert
        Assert.True(result.HasProblems(out var resultProblems));
        Assert.NotNull(resultProblems);
        Assert.Equal(2, resultProblems.Count);
        Assert.Equal("invalid", resultProblems[0].Detail);
        Assert.Equal("not found", resultProblems[1].Detail);
    }

    [Fact]
    public void Result_Add_ResultWitProblems_Must_HasProblems()
    {
        // Arrange
        Result result = Result.Ok();
        Result other = Problems.InvalidParameter("invalid");

        // Act
        result += other;

        // Assert
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Equal("invalid", problems[0].Detail);
    }

    [Fact]
    public void Result_Add_ResultValuedWitProblems_Must_HasProblems()
    {
        // Arrange
        Result result = Result.Ok();
        Result<string> other = Problems.InvalidParameter("invalid");

        // Act
        result += other;

        // Assert
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Equal("invalid", problems[0].Detail);
    }

    [Fact]
    public void Result_GetByIndex_Must_Throw_When_Successful()
    {
        // Arrange
        var result = Result.Ok();

        // Act
        Action act = () => _ = result[0];

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
}
