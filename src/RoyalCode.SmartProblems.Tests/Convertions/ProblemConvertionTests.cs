
using Microsoft.AspNetCore.Mvc;
using RoyalCode.SmartProblems.Descriptions;
using RoyalCode.SmartProblems.Convertions;

namespace RoyalCode.SmartProblems.Tests.Convertions;

public class ProblemConvertionTests
{
    [Fact]
    public void Convert_NotFount()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        var problem = Problems.NotFound("Not Found");

        // Act
        ProblemDetails problemDetails = problem.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, problemDetails.Status);
        Assert.Equal("Not Found", problemDetails.Detail);
        Assert.Equal(ProblemDetailsExtended.Titles.NotFoundTitle, problemDetails.Title);
        Assert.Equal("about:blank", problemDetails.Type);
    }

    [Fact]
    public void Convert_InvalidParameter()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        var problem = Problems.InvalidParameter("Invalid Parameter", "MyProperty");

        // Act
        ProblemDetails problemDetails = problem.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal("Invalid Parameter", problemDetails.Detail);
        Assert.Equal(ProblemDetailsExtended.Titles.InvalidParametersTitle, problemDetails.Title);
        Assert.Equal("about:blank", problemDetails.Type);
        Assert.Equal("MyProperty", problemDetails.Extensions["property"]);
    }

    [Fact]
    public void Convert_ValidationFailed()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        var problem = Problems.ValidationFailed("Validation Failed", "MyProperty");

        // Act
        ProblemDetails problemDetails = problem.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, problemDetails.Status);
        Assert.Equal("Validation Failed", problemDetails.Detail);
        Assert.Equal(ProblemDetailsExtended.Titles.ValidationFailedTitle, problemDetails.Title);
        Assert.Equal("about:blank", problemDetails.Type);
        Assert.Equal("MyProperty", problemDetails.Extensions["property"]);
    }

    [Fact]
    public void Convert_InvalidState()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        var problem = Problems.InvalidState("Invalid State");

        // Act
        ProblemDetails problemDetails = problem.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status409Conflict, problemDetails.Status);
        Assert.Equal("Invalid State", problemDetails.Detail);
        Assert.Equal(ProblemDetailsExtended.Titles.InvalidStateTitle, problemDetails.Title);
        Assert.Equal("about:blank", problemDetails.Type);
    }

    [Fact]
    public void Convert_NotAllowed()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        var problem = Problems.NotAllowed("Not Allowed");

        // Act
        ProblemDetails problemDetails = problem.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden, problemDetails.Status);
        Assert.Equal("Not Allowed", problemDetails.Detail);
        Assert.Equal(ProblemDetailsExtended.Titles.NotAllowedTitle, problemDetails.Title);
        Assert.Equal("about:blank", problemDetails.Type);
    }

    [Fact]
    public void Convert_InternalServerError()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        var exception = new Exception("Internal Server Error");
        var problem = Problems.InternalError(exception);

        // Act
        ProblemDetails problemDetails = problem.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal("Internal Server Error", problemDetails.Detail);
        Assert.Equal(ProblemDetailsExtended.Titles.InternalServerErrorTitle, problemDetails.Title);
        Assert.Equal("about:blank", problemDetails.Type);
    }

    [Fact]
    public void Convert_CustomProblem_NotDescribed()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        var problem = Problems.Custom("Custom Problem", "MyType");

        // Act
        ProblemDetails problemDetails = problem.ToProblemDetails(options);

        // Assert
        Assert.Equal("MyType", problemDetails.Type);
        Assert.Equal("Custom Problem", problemDetails.Detail);
    }
}
