using Microsoft.AspNetCore.Mvc;
using RoyalCode.SmartProblems.Conversions;
using RoyalCode.SmartProblems.Descriptions;

namespace RoyalCode.SmartProblems.Tests.Conversions;

/// <summary>
/// <para>
///     Tests for the convertion of the problems to ProblemDetails.
/// </para>
/// <para>
///     These tests cover a single problem in a ProblemDetails.
/// </para>
/// </summary>
public class ProblemConversionTests
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
        Assert.Equal("#/MyProperty", problemDetails.Extensions[ProblemDetailsExtended.Fields.Pointer]);
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
        Assert.Equal("#/MyProperty", problemDetails.Extensions[ProblemDetailsExtended.Fields.Pointer]);
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
        Assert.Equal($"tag:problemdetails/.problems#problem-occurred", problemDetails.Type);
        Assert.Equal(ProblemDetailsExtended.Titles.DefaultTitle, problemDetails.Title);
        Assert.Equal("Custom Problem", problemDetails.Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, problemDetails.Status);
    }

    [Fact]
    public void Convert_CustomProblem_Described()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription(
            "MyType", 
            "My Custom Problem", 
            "Used for tests only",
            System.Net.HttpStatusCode.UnprocessableEntity));

        var problem = Problems.Custom("Custom Problem", "MyType");

        // Act
        ProblemDetails problemDetails = problem.ToProblemDetails(options);

        // Assert
        Assert.Equal("tag:problemdetails/.problems#MyType", problemDetails.Type);
        Assert.Equal("My Custom Problem", problemDetails.Title);
        Assert.Equal("Custom Problem", problemDetails.Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, problemDetails.Status);
    }
}
