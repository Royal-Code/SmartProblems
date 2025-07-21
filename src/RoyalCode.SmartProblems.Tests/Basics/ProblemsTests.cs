
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.SmartProblems.Tests.Basics;

public class ProblemsTests
{
    [Fact]
    public void Problems_InternalError_Exception_Must_UseMessageAsDetail()
    {
        // Arrange
        var message = "This is a message";
        var exception = new Exception(message);

        // Act
        var problem = Problems.InternalError(exception);

        // Assert
        Assert.Equal(message, problem.Detail);
    }

    [Fact]
    public void Problems_InternalError_ArgumentException_Must_UseParameterAsProperty()
    {
        // Arrange
        var paramName = "paramName";
        var exception = new ArgumentException("message", paramName);

        // Act
        var problem = Problems.InternalError(exception);

        // Assert
        Assert.Equal(paramName, problem.Property);
    }

    [Fact]
    public void Problems_InternalError_ArgumentNullException_Must_UseParameterAsProperty()
    {
        // Arrange
        var paramName = "paramName";
        var exception = new ArgumentNullException(paramName);

        // Act
        var problem = Problems.InternalError(exception);

        // Assert
        Assert.Equal(paramName, problem.Property);
    }

    [Fact]
    public void Problems_InternalError_CustomException_Must_BeHandled()
    {
        // Arrange
        Problems.ExceptionHandler = new TestsExceptionHandler();
        var exception = new CustomException("my message", "my_code");

        // Act
        var problem = Problems.InternalError(exception);

        // Assert
        Assert.Equal(exception.Message, problem.Detail);
        Assert.Equal(exception.Code, problem.TypeId);
    }

    [Fact]
    public void Problems_InternalError_DisableExceptionMessage()
    {
        // Arrange
        var options = new ExceptionOptions 
        { 
            UseExceptionMessageAsDetail = false
        };

        // Act
        var problem = Problems.InternalError(new Exception("This is a message"), options);

        // Assert
        Assert.Equal(options.DefaultExceptionMessage, problem.Detail);
    }

    [Fact]
    public void Problems_InternalError_EnableExceptionTypeAndStackTrace()
    {
        // Arrange
        var options = new ExceptionOptions
        {
            IncludeExceptionTypeName = true,
            IncludeStackTrace = true
        };

        // Act
        var problem = Problems.InternalError(new Exception("This is a message"), options);

        // Assert
        object? exceptionType = null;
        problem.Extensions?.TryGetValue("exception", out exceptionType);
        var exceptionName = exceptionType as string;
        Assert.Contains("System.Exception", exceptionName);
        
        var containsStackTrace = problem.Extensions?.ContainsKey("stack_trace") ?? false;
        Assert.True(containsStackTrace);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("my-type")]
    public void Problems_Custom_Must_HaveTypeId(string? typeId)
    {
        // Arrange
        var mustThrow = string.IsNullOrWhiteSpace(typeId);

        // Act
        var act = new Action(() => Problems.Custom("Message", typeId!));

        // Assert
        if (mustThrow)
        {
            Assert.Throws<ArgumentException>(act);
        }
        else
        {
            act();
        }
    }

    [Fact]
    public void Problems_Custom_Must_HaveUriValidTypeId()
    {
        // Arrange
        var typeId = "my type";

        // Act
        var act = new Action(() => Problems.Custom("Message", typeId));

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Problems_Implict_Add_Problem()
    {
        // Arrange
        Problems problems = [];

        // Act
        problems += Problems.InvalidParameter("Invalid parameter");

        // Assert
        Assert.Single(problems);
    }

    [Fact]
    public void Problems_Implict_Add_Problems()
    {
        // Arrange
        Problems problems = [];
        Problems other = [Problems.InvalidParameter("Invalid parameter"), Problems.InvalidParameter("Invalid parameter")];

        // Act
        problems += other;

        // Assert
        Assert.Equal(2, problems.Count);
    }

    [Fact]
    public void Problems_GetByIndex()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("Invalid parameter 1"), Problems.InvalidParameter("Invalid parameter 2")];

        // Act
        var problem = problems[1];

        // Assert
        Assert.Equal("Invalid parameter 2", problem.Detail);
    }

    [Fact]
    public void Problems_GetByIndex_Must_ThrowIndexOutOfRangeException()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("Invalid parameter 1"), Problems.InvalidParameter("Invalid parameter 2")];

        // Act
        var act = new Action(() => _ = problems[2]);

        // Assert
        Assert.Throws<IndexOutOfRangeException>(act);
    }

    [Fact]
    public void Problems_ForEach()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("Invalid parameter 1"), Problems.InvalidParameter("Invalid parameter 2")];

        // Act
        var count = 0;
        foreach (var problem in problems)
        {
            count++;
        }

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void Problems_Count()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("Invalid parameter 1"), Problems.InvalidParameter("Invalid parameter 2")];

        // Act
        var count = problems.Count;

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void Problems_Contains()
    {
        var problem3 = Problems.InvalidParameter("Invalid parameter 3");

        // Arrange
        Problems problems = [
            Problems.InvalidParameter("Invalid parameter 1"),
            Problems.InvalidParameter("Invalid parameter 2"),
            problem3,
            Problems.InvalidParameter("Invalid parameter 4"),
            Problems.InvalidParameter("Invalid parameter 5")];

        // Act
        var contains = problems.Contains(problem3);

        // Assert
        Assert.True(contains);
    }

    [Fact]
    public void Problems_Contains_Must_ReturnFalse()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("Invalid parameter 1"), Problems.InvalidParameter("Invalid parameter 2")];

        // Act
        var contains = problems.Contains(Problems.InvalidParameter("Invalid parameter 3"));

        // Assert
        Assert.False(contains);
    }

    [Fact]
    public void Problems_CopyTo()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("Invalid parameter 1"), Problems.InvalidParameter("Invalid parameter 2")];
        var array = new Problem[2];

        // Act
        problems.CopyTo(array, 0);

        // Assert
        Assert.Equal("Invalid parameter 1", array[0].Detail);
        Assert.Equal("Invalid parameter 2", array[1].Detail);
    }

    [Fact]
    public void Problems_CopyTo_Must_ThrowArgumentException_When_LengthIsNotEnough()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("Invalid parameter 1"), Problems.InvalidParameter("Invalid parameter 2")];
        var array = new Problem[2];

        // Act
        var act = new Action(() => problems.CopyTo(array, 1));

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Problems_ToException()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("Invalid parameter 1"), Problems.InvalidParameter("Invalid parameter 2")];
        
        // Act
        var exception = problems.ToException();
        
        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Contains("Invalid parameter 1", exception.Message);
        Assert.Contains("Invalid parameter 2", exception.Message);
    }

    [Fact]
    public void Problems_ToException_WithMessagePattern()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("Invalid parameter 1"), Problems.InvalidParameter("Invalid parameter 2")];

        // Act
        var exception = problems.ToException("Custom message: {0}");

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Contains("Custom message:", exception.Message);
        Assert.Contains("Invalid parameter 1", exception.Message);
        Assert.Contains("Invalid parameter 2", exception.Message);
    }

    [Fact]
    public void Problems_ToException_WithMessagePattern_And_Separator()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("Invalid parameter 1"), Problems.InvalidParameter("Invalid parameter 2")];

        // Act
        var exception = problems.ToException("Custom message:\n - {0}", "\n - ");

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Contains("Custom message:", exception.Message);
        Assert.Contains("\n - Category: InvalidParameter, Details: Invalid parameter 1", exception.Message);
        Assert.Contains("\n - Category: InvalidParameter, Details: Invalid parameter 2", exception.Message);
    }
}

#region classes

#pragma warning disable S3871 // Exceptions must be public

file sealed class CustomException : Exception
{
    public CustomException(string message, string code) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}

file class TestsExceptionHandler : IExceptionHandler
{
    public bool TryHandle(Exception exception, [NotNullWhen(true)] out Problem? problem)
    {
        if (exception is CustomException customException)
        {
            problem = new Problem
            {
                Category = ProblemCategory.CustomProblem,
                Detail = customException.Message,
                TypeId = customException.Code
            };
            return true;
        }

        problem = null;
        return false;
    }
}

#endregion
