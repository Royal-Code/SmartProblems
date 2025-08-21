
using RoyalCode.SmartProblems.Entities;

namespace RoyalCode.SmartProblems.Tests.Basics;

public class ResultValuedTests
{
    [Fact]
    public void ResultValued_Implicit_Value()
    {
        // Arrange
        Result<string> result;
        string value = "Good day!";

        // Act
        result = value;

        // Assert
        Assert.True(result.HasValue(out var resultValue));
        Assert.Equal(value, resultValue);
    }

    [Fact]
    public void ResultValued_Implicit_NullValue_Must_Throw()
    {
        // Arrange
        Result<string> result;
        string? value = null;

        // Act
        Action act = () => result = value!;

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void ResultValued_Implicit_Problem()
    {
        // Arrange
        Result<string> result;
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
    public void ResultValued_Implicit_Problems()
    {
        // Arrange
        Result<string> result;
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
    public void Result_Implicit_Task()
    {
        // Arrange
        Result<int> result = 42;
        Task<Result<int>> task;

        // Act
        task = result;

        // Assert
        Assert.NotNull(task);
        Assert.True(task.IsCompleted);
        var resultValue = task.Result;
        Assert.Equal(result, resultValue);
        Assert.True(resultValue.HasValue(out var value));
        Assert.Equal(42, value);
    }

    [Fact]
    public void ResultValued_Implicit_Problems_Must_Throw_When_Null()
    {
        // Arrange
        Result<string> result;
        Problems? problems = null;

        // Act
        Action act = () => result = problems!;

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void ResultValued_Implicit_Exception()
    {
        // Arrange
        Result<string> result;
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
    public void ResultValued_Implicit_FindResult_Found()
    {
        // Arrange
        FindResult<string> findResult = new FindResult<string>("Good day!");

        // Act
        Result<string> result = findResult;
        
        // Assert
        Assert.True(result.HasValue(out var resultValue));
        Assert.Equal("Good day!", resultValue);
    }

    [Fact]
    public void ResultValued_Implicit_FindResult_NotFound()
    {
        // Arrange
        FindResult<string> findResult = FindResult<string>.Problem("name", "name", "x");

        // Act
        Result<string> result = findResult;

        // Assert
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
    }

    [Fact]
    public void ResultValued_Add_Problem_Must_HasProblems()
    {
        // Arrange
        Result<string> result = Problems.InvalidParameter("invalid 1");

        // Act
        result += Problems.InvalidParameter("invalid 2");

        // Assert
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Equal(2, problems.Count);
        Assert.Equal("invalid 1", problems[0].Detail);
        Assert.Equal("invalid 2", problems[1].Detail);
    }

    [Fact]
    public void ResultValued_Add_Problem_Must_Throw_When_Successfully()
    {
        // Arrange
        Result<string> result = "Good day!";

        // Act
        Action act = () => result += Problems.InvalidParameter("invalid 2");

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void ResultValued_Add_Problems_Must_HasProblems()
    {
        // Arrange
        Result<string> result = Problems.InvalidParameter("invalid 1");
        Problems problems = [Problems.InvalidParameter("invalid 2"), Problems.NotFound("not found")];

        // Act
        result += problems;

        // Assert
        Assert.True(result.HasProblems(out var resultProblems));
        Assert.NotNull(resultProblems);
        Assert.Equal(3, resultProblems.Count);
        Assert.Equal("invalid 1", resultProblems[0].Detail);
        Assert.Equal("invalid 2", resultProblems[1].Detail);
        Assert.Equal("not found", resultProblems[2].Detail);
    }

    [Fact]
    public void ResultValued_Add_Problems_Must_Throw_When_Successfully()
    {
        // Arrange
        Result<string> result = "Good day!";
        Problems problems = [Problems.InvalidParameter("invalid 2"), Problems.NotFound("not found")];

        // Act
        Action act = () => result += problems;

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void ResultValued_Add_Resul_Must_HasProblems()
    {
        // Arrange
        Result<string> result = Problems.InvalidParameter("invalid 1");
        Result other = Problems.InvalidParameter("invalid 2");

        // Act
        result += other;

        // Assert
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Equal(2, problems.Count);
        Assert.Equal("invalid 1", problems[0].Detail);
        Assert.Equal("invalid 2", problems[1].Detail);
    }

    [Fact]
    public void ResultValued_Add_Result_Must_Throw_When_Successfully()
    {
        // Arrange
        Result<string> result = "Good day!";
        Result other = Problems.InvalidParameter("invalid 2");

        // Act
        Action act = () => result += other;

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void ResultValued_Add_Result_Must_Success_When_BothSuccessful()
    {
        // Arrange
        Result<string> result = "Good day!";
        Result other = Result.Ok();

        // Act
        result += other;

        // Assert
        Assert.True(result.HasValue(out var resultValue));
        Assert.Equal("Good day!", resultValue);
    }

    [Fact]
    public void ResultValued_Add_ResulValued_Must_HasProblems()
    {
        // Arrange
        Result<string> result = Problems.InvalidParameter("invalid 1");
        Result<string> other = Problems.InvalidParameter("invalid 2");

        // Act
        result += other;

        // Assert
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Equal(2, problems.Count);
        Assert.Equal("invalid 1", problems[0].Detail);
        Assert.Equal("invalid 2", problems[1].Detail);
    }

    [Fact]
    public void ResultValued_Add_ResultValued_Must_Throw_When_Successfully()
    {
        // Arrange
        Result<string> result = "Good day!";
        Result<string> other = Problems.InvalidParameter("invalid 2");

        // Act
        Action act = () => result += other;

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void ResultValued_Add_ResultValued_Must_Success_When_BothSuccessful()
    {
        // Arrange
        Result<string> result = "Good day!";
        Result<string> other = "Good night!";

        // Act
        result += other;

        // Assert
        Assert.True(result.HasValue(out var resultValue));
        Assert.Equal("Good day!", resultValue);
    }

    [Fact]
    public void ResultValued_GetByIndex_Must_Throw_When_Successful()
    {
        // Arrange
        Result<string> result = "Good day!";

        // Act
        Action act = () => _ = result[0];

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
    
    [Fact]
    public void ResultValued_EnsureSuccess_WithoutProblems_MustNotThrow()
    {
        // Arrange
        Result<string> result = "Success";

        // Act
        result.EnsureSuccess();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }
    
    [Fact]
    public void ResultValued_EnsureSuccess_WithProblems_MustThrow()
    {
        // Arrange
        Result<string> result = Problems.InvalidParameter("invalid");

        // Act
        Action act = () => result.EnsureSuccess();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
    
    [Fact]
    public void ResultValued_EnsureHasValue_WithoutProblems_MustHasValue()
    {
        // Arrange
        Result<string> result = "Success";

        // Act
        result.EnsureHasValue(out var value);

        // Assert
        Assert.Equal("Success", value);
    }

    [Fact]
    public void ResultValued_EnsureHasValue_WithProblems_MustThrow()
    {
        // Arrange
        Result<string> result = Problems.InvalidParameter("invalid");

        // Act
        Action act = () => result.EnsureHasValue(out _);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void ResultValued_HasProblemsOrGetValue()
    {
        // Arrange
        Result<string> result = "Success";

        // Act
        var hasProblems = result.HasProblemsOrGetValue(out var problems, out var value);

        // Assert
        Assert.False(hasProblems);
        Assert.Null(problems);
        Assert.NotNull(value);
        Assert.Equal("Success", value);
    }

    [Fact]
    public void ResultValued_HasProblemsOrGetValue_WithProblem()
    {
        // Arrange
        Result<string> result = Problems.InvalidParameter("invalid");

        // Act
        var hasProblems = result.HasProblemsOrGetValue(out var problems, out var value);

        // Assert
        Assert.True(hasProblems);
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Equal("invalid", problems[0].Detail);
        Assert.Null(value);
    }

    [Fact]
    public void ResultValued_HasValueOrGetProblems()
    {
        // Arrange
        Result<string> result = "Success";

        // Act
        var hasValue = result.HasValueOrGetProblems(out var value, out var problems);

        // Assert
        Assert.True(hasValue);
        Assert.Null(problems);
        Assert.NotNull(value);
        Assert.Equal("Success", value);
    }

    [Fact]
    public void ResultValued_HasValueOrGetProblems_WithProblem()
    {
        // Arrange
        Result<string> result = Problems.InvalidParameter("invalid");

        // Act
        var hasValue = result.HasValueOrGetProblems(out var value, out var problems);

        // Assert
        Assert.False(hasValue);
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Equal("invalid", problems[0].Detail);
        Assert.Null(value);
    }
}
