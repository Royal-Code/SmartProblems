using Microsoft.AspNetCore.Mvc;
using RoyalCode.SmartProblems.Conversions;
using RoyalCode.SmartProblems.Descriptions;

// ReSharper disable PossibleMultipleEnumeration

namespace RoyalCode.SmartProblems.Tests.Conversions;

/// <summary>
/// <para>
///     Tests for the conversion of the <see cref="Problems"/> to <see cref="ProblemDetails"/>.
/// </para>
/// <para>
///     These tests cover a multiple problems in a ProblemDetails.
/// </para>
/// </summary>
public class ProblemsConversionTests
{
    [Fact]
    public void Convert_Many_NotFount()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.NotFound("Not Found 1") 
            + Problems.NotFound("Not Found 2") 
            + Problems.NotFound("Not Found 3");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, problemDetails.Status);
        Assert.Equal(ProblemDetailsExtended.Titles.NotFoundTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.NotFoundMessage, problemDetails.Detail);
        Assert.Equal("about:blank", problemDetails.Type);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.NotFound, out var notFound);
        Assert.NotNull(notFound);

        var notFoundArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(notFound);
        Assert.Equal(3, notFoundArray.Count());

        Assert.Equal("Not Found 1", notFoundArray.ElementAt(0).Detail);
        Assert.Equal("Not Found 2", notFoundArray.ElementAt(1).Detail);
        Assert.Equal("Not Found 3", notFoundArray.ElementAt(2).Detail);
    }

    [Fact]
    public void Convert_Many_InvalidParameter()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.InvalidParameter("Invalid Parameter 1", "MyProperty1")
            + Problems.InvalidParameter("Invalid Parameter 2", "MyProperty2")
            + Problems.InvalidParameter("Invalid Parameter 3", "MyProperty3");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal(ProblemDetailsExtended.Titles.InvalidParametersTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.InvalidParametersMessage, problemDetails.Detail);
        Assert.Equal("about:blank", problemDetails.Type);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var invalidParameters);
        Assert.NotNull(invalidParameters);

        var invalidParametersArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(invalidParameters);
        Assert.Equal(3, invalidParametersArray.Count());

        Assert.Equal("Invalid Parameter 1", invalidParametersArray.ElementAt(0).Detail);
        Assert.Equal("#/MyProperty1", invalidParametersArray.ElementAt(0).Pointer);

        Assert.Equal("Invalid Parameter 2", invalidParametersArray.ElementAt(1).Detail);
        Assert.Equal("#/MyProperty2", invalidParametersArray.ElementAt(1).Pointer);

        Assert.Equal("Invalid Parameter 3", invalidParametersArray.ElementAt(2).Detail);
        Assert.Equal("#/MyProperty3", invalidParametersArray.ElementAt(2).Pointer);
    }

    [Fact]
    public void Convert_Many_ValidationFailed()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        var problems = Problems.ValidationFailed("Validation Failed 1", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed 2", "MyProperty2")
            + Problems.ValidationFailed("Validation Failed 3", "MyProperty3");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, problemDetails.Status);
        Assert.Equal(ProblemDetailsExtended.Titles.ValidationFailedTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.ValidationFailedMessage, problemDetails.Detail);
        Assert.Equal("about:blank", problemDetails.Type);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var validationFailed);
        Assert.NotNull(validationFailed);

        var validationFailedArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(validationFailed);
        Assert.Equal(3, validationFailedArray.Count());

        Assert.Equal("Validation Failed 1", validationFailedArray.ElementAt(0).Detail);
        Assert.Equal("#/MyProperty1", validationFailedArray.ElementAt(0).Pointer);

        Assert.Equal("Validation Failed 2", validationFailedArray.ElementAt(1).Detail);
        Assert.Equal("#/MyProperty2", validationFailedArray.ElementAt(1).Pointer);

        Assert.Equal("Validation Failed 3", validationFailedArray.ElementAt(2).Detail);
        Assert.Equal("#/MyProperty3", validationFailedArray.ElementAt(2).Pointer);
    }

    [Fact]
    public void Convert_Many_InvalidState()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.InvalidState("Invalid State 1")
            + Problems.InvalidState("Invalid State 2")
            + Problems.InvalidState("Invalid State 3");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status409Conflict, problemDetails.Status);
        Assert.Equal(ProblemDetailsExtended.Titles.InvalidStateTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.InvalidStateMessage, problemDetails.Detail);
        Assert.Equal("about:blank", problemDetails.Type);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var invalidState);
        Assert.NotNull(invalidState);

        var invalidStateArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(invalidState);
        Assert.Equal(3, invalidStateArray.Count());

        Assert.Equal("Invalid State 1", invalidStateArray.ElementAt(0).Detail);
        Assert.Equal("Invalid State 2", invalidStateArray.ElementAt(1).Detail);
        Assert.Equal("Invalid State 3", invalidStateArray.ElementAt(2).Detail);
    }

    [Fact]
    public void Convert_Many_NotAllowed()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.NotAllowed("Not Allowed 1")
            + Problems.NotAllowed("Not Allowed 2")
            + Problems.NotAllowed("Not Allowed 3");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden, problemDetails.Status);
        Assert.Equal(ProblemDetailsExtended.Titles.NotAllowedTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.NotAllowedMessage, problemDetails.Detail);
        Assert.Equal("about:blank", problemDetails.Type);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var notAllowed);
        Assert.NotNull(notAllowed);

        var notAllowedArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(notAllowed);
        Assert.Equal(3, notAllowedArray.Count());

        Assert.Equal("Not Allowed 1", notAllowedArray.ElementAt(0).Detail);
        Assert.Equal("Not Allowed 2", notAllowedArray.ElementAt(1).Detail);
        Assert.Equal("Not Allowed 3", notAllowedArray.ElementAt(2).Detail);
    }

    [Fact]
    public void Convert_Many_InternalServerError()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.InternalError(new Exception("Internal Server Error 1"))
            + Problems.InternalError(new Exception("Internal Server Error 2"))
            + Problems.InternalError(new Exception("Internal Server Error 3"));

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal(ProblemDetailsExtended.Titles.InternalServerErrorTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.InternalErrorMessage, problemDetails.Detail);
        Assert.Equal("about:blank", problemDetails.Type);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var internalServerError);
        Assert.NotNull(internalServerError);

        var internalServerErrorArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(internalServerError);
        Assert.Equal(3, internalServerErrorArray.Count());

        Assert.Equal("Internal Server Error 1", internalServerErrorArray.ElementAt(0).Detail);
        Assert.Equal("Internal Server Error 2", internalServerErrorArray.ElementAt(1).Detail);
        Assert.Equal("Internal Server Error 3", internalServerErrorArray.ElementAt(2).Detail);
    }

    [Fact]
    public void Convert_Many_CustomProblem_NotDescribed()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problem = Problems.Custom("Custom Problem 1", "MyType")
            + Problems.Custom("Custom Problem 2", "MyType")
            + Problems.Custom("Custom Problem 3", "MyType");

        // Act
        var problemDetails = problem.ToProblemDetails(options);

        // Assert
        Assert.Equal($"tag:problemdetails/.problems#aggregate-problems-details", problemDetails.Type);
        Assert.Equal(ProblemDetailsDescriptor.Titles.AggregateProblemsDetailsTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.AggregateMessage, problemDetails.Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, problemDetails.Status);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Aggregate, out var aggregate);
        Assert.NotNull(aggregate);

        var aggregateArray = Assert.IsAssignableFrom<IEnumerable<ProblemDetails>>(aggregate);
        Assert.Equal(3, aggregateArray.Count());

        Assert.Equal("Custom Problem 1", aggregateArray.ElementAt(0).Detail);
        Assert.Equal("Custom Problem 2", aggregateArray.ElementAt(1).Detail);
        Assert.Equal("Custom Problem 3", aggregateArray.ElementAt(2).Detail);
    }

    [Fact]
    public void Convert_Many_CustomProblem_Described()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription(
            "MyType",
            "My Custom Problem",
            "Used for tests only",
            System.Net.HttpStatusCode.UnprocessableEntity));

        Problems problem = Problems.Custom("Custom Problem 1", "MyType")
            + Problems.Custom("Custom Problem 2", "MyType")
            + Problems.Custom("Custom Problem 3", "MyType");

        // Act
        var problemDetails = problem.ToProblemDetails(options);

        // Assert
        Assert.Equal("tag:problemdetails/.problems#aggregate-problems-details", problemDetails.Type);
        Assert.Equal(ProblemDetailsDescriptor.Titles.AggregateProblemsDetailsTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.AggregateMessage, problemDetails.Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, problemDetails.Status);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Aggregate, out var aggregate);
        Assert.NotNull(aggregate);

        var aggregateArray = Assert.IsAssignableFrom<IEnumerable<ProblemDetails>>(aggregate);
        Assert.Equal(3, aggregateArray.Count());

        Assert.Equal("tag:problemdetails/.problems#MyType", aggregateArray.ElementAt(0).Type);
        Assert.Equal("My Custom Problem", aggregateArray.ElementAt(0).Title);
        Assert.Equal("Custom Problem 1", aggregateArray.ElementAt(0).Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, aggregateArray.ElementAt(0).Status);

        Assert.Equal("tag:problemdetails/.problems#MyType", aggregateArray.ElementAt(1).Type);
        Assert.Equal("My Custom Problem", aggregateArray.ElementAt(1).Title);
        Assert.Equal("Custom Problem 2", aggregateArray.ElementAt(1).Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, aggregateArray.ElementAt(1).Status);

        Assert.Equal("tag:problemdetails/.problems#MyType", aggregateArray.ElementAt(2).Type);
        Assert.Equal("My Custom Problem", aggregateArray.ElementAt(2).Title);
        Assert.Equal("Custom Problem 3", aggregateArray.ElementAt(2).Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, aggregateArray.ElementAt(2).Status);
    }

    [Fact]
    public void Convert_NotFound_InvalidParameter()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal(ProblemDetailsExtended.Titles.InvalidParametersTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.InvalidParametersMessage, problemDetails.Detail);
        Assert.Equal("about:blank", problemDetails.Type);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.NotFound, out var notFound);
        Assert.NotNull(notFound);

        var notFoundArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(notFound);
        Assert.Single(notFoundArray);
        Assert.Equal("Not Found", notFoundArray.ElementAt(0).Detail);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var invalidParameters);
        Assert.NotNull(invalidParameters);

        var invalidParametersArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(invalidParameters);
        Assert.Single(invalidParametersArray);
        Assert.Equal("Invalid Parameter", invalidParametersArray.ElementAt(0).Detail);
        Assert.Equal("#/MyProperty", invalidParametersArray.ElementAt(0).Pointer);
    }

    [Fact]
    public void Convert_NotFound_InvalidParameter_ValidationFailed()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, problemDetails.Status);
        Assert.Equal(ProblemDetailsExtended.Titles.ValidationFailedTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.ValidationFailedMessage, problemDetails.Detail);
        Assert.Equal("about:blank", problemDetails.Type);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.NotFound, out var notFound);
        Assert.NotNull(notFound);

        var notFoundArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(notFound);
        Assert.Single(notFoundArray);
        Assert.Equal("Not Found", notFoundArray.ElementAt(0).Detail);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var errors);
        Assert.NotNull(errors);

        var errorsArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(errors);
        Assert.Equal(2, errorsArray.Count());
        
        Assert.Equal("Invalid Parameter", errorsArray.ElementAt(0).Detail);
        Assert.Equal("#/MyProperty1", errorsArray.ElementAt(0).Pointer);
        
        Assert.Equal("Validation Failed", errorsArray.ElementAt(1).Detail);
        Assert.Equal("#/MyProperty2", errorsArray.ElementAt(1).Pointer);
    }

    [Fact]
    public void Convert_NotFound_InvalidParameter_ValidationFailed_InvalidState()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status409Conflict, problemDetails.Status);
        Assert.Equal(ProblemDetailsExtended.Titles.InvalidStateTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.InvalidStateMessage, problemDetails.Detail);
        Assert.Equal("about:blank", problemDetails.Type);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.NotFound, out var notFound);
        Assert.NotNull(notFound);

        var notFoundArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(notFound);
        Assert.Single(notFoundArray);
        Assert.Equal("Not Found", notFoundArray.ElementAt(0).Detail);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var errors);
        Assert.NotNull(errors);

        var errorsArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(errors);
        Assert.Equal(3, errorsArray.Count());

        Assert.Equal("Invalid Parameter", errorsArray.ElementAt(0).Detail);
        Assert.Equal("#/MyProperty1", errorsArray.ElementAt(0).Pointer);

        Assert.Equal("Validation Failed", errorsArray.ElementAt(1).Detail);
        Assert.Equal("#/MyProperty2", errorsArray.ElementAt(1).Pointer);

        Assert.Equal("Invalid State", errorsArray.ElementAt(2).Detail);
    }

    [Fact]
    public void Convert_NotFound_InvalidParameter_ValidationFailed_InvalidState_InternalError()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State")
            + Problems.InternalError(new Exception("Internal Server Error"));

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal(ProblemDetailsExtended.Titles.InternalServerErrorTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.InternalErrorMessage, problemDetails.Detail);
        Assert.Equal("about:blank", problemDetails.Type);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.NotFound, out var notFound);
        Assert.NotNull(notFound);

        var notFoundArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(notFound);
        Assert.Single(notFoundArray);
        Assert.Equal("Not Found", notFoundArray.ElementAt(0).Detail);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var errors);
        Assert.NotNull(errors);

        var errorsArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(errors);
        Assert.Equal(4, errorsArray.Count());

        Assert.Equal("Invalid Parameter", errorsArray.ElementAt(0).Detail);
        Assert.Equal("#/MyProperty1", errorsArray.ElementAt(0).Pointer);

        Assert.Equal("Validation Failed", errorsArray.ElementAt(1).Detail);
        Assert.Equal("#/MyProperty2", errorsArray.ElementAt(1).Pointer);

        Assert.Equal("Invalid State", errorsArray.ElementAt(2).Detail);
        Assert.Equal("Internal Server Error", errorsArray.ElementAt(3).Detail);
    }

    [Fact]
    public void Convert_NotFound_InvalidParameter_ValidationFailed_InvalidState_InternalError_CustomError_NotDescribed()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State")
            + Problems.InternalError(new Exception("Internal Server Error"))
            + Problems.Custom("Custom Problem", "MyType");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal($"tag:problemdetails/.problems#problem-occurred", problemDetails.Type);
        Assert.Equal(ProblemDetailsExtended.Titles.DefaultTitle, problemDetails.Title);
        Assert.Equal("Custom Problem", problemDetails.Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, problemDetails.Status);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.NotFound, out var notFound);
        Assert.NotNull(notFound);

        var notFoundArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(notFound);
        Assert.Single(notFoundArray);
        Assert.Equal("Not Found", notFoundArray.ElementAt(0).Detail);

                problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var errors);
        Assert.NotNull(errors);

        var errorsArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(errors);
        Assert.Equal(4, errorsArray.Count());

        Assert.Equal("Invalid Parameter", errorsArray.ElementAt(0).Detail);
        Assert.Equal("#/MyProperty1", errorsArray.ElementAt(0).Pointer);

        Assert.Equal("Validation Failed", errorsArray.ElementAt(1).Detail);
        Assert.Equal("#/MyProperty2", errorsArray.ElementAt(1).Pointer);

        Assert.Equal("Invalid State", errorsArray.ElementAt(2).Detail);
        Assert.Equal("Internal Server Error", errorsArray.ElementAt(3).Detail);
    }

    [Fact]
    public void Convert_NotFound_InvalidParameter_ValidationFailed_InvalidState_InternalError_CustomError_Described()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription(
            "MyType",
            "My Custom Problem",
            "Used for tests only",
            System.Net.HttpStatusCode.UnprocessableEntity));

        Problems problems = Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State")
            + Problems.InternalError(new Exception("Internal Server Error"))
            + Problems.Custom("Custom Problem", "MyType");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal("tag:problemdetails/.problems#MyType", problemDetails.Type);
        Assert.Equal("My Custom Problem", problemDetails.Title);
        Assert.Equal("Custom Problem", problemDetails.Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, problemDetails.Status);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.NotFound, out var notFound);
        Assert.NotNull(notFound);

        var notFoundArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(notFound);
        Assert.Single(notFoundArray);
        Assert.Equal("Not Found", notFoundArray.ElementAt(0).Detail);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var errors);
        Assert.NotNull(errors);

        var errorsArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(errors);
        Assert.Equal(4, errorsArray.Count());

        Assert.Equal("Invalid Parameter", errorsArray.ElementAt(0).Detail);
        Assert.Equal("#/MyProperty1", errorsArray.ElementAt(0).Pointer);

        Assert.Equal("Validation Failed", errorsArray.ElementAt(1).Detail);
        Assert.Equal("#/MyProperty2", errorsArray.ElementAt(1).Pointer);

        Assert.Equal("Invalid State", errorsArray.ElementAt(2).Detail);
        Assert.Equal("Internal Server Error", errorsArray.ElementAt(3).Detail);
    }

    [Fact]
    public void Convert_NotFound_InvalidParameter_ValidationFailed_InvalidState_InternalError_ManyCustomError_NotDescribed()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        Problems problems = Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State")
            + Problems.InternalError(new Exception("Internal Server Error"))
            + Problems.Custom("Custom Problem 1", "MyType")
            + Problems.Custom("Custom Problem 2", "MyType")
            + Problems.Custom("Custom Problem 3", "MyType");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal($"tag:problemdetails/.problems#aggregate-problems-details", problemDetails.Type);
        Assert.Equal(ProblemDetailsDescriptor.Titles.AggregateProblemsDetailsTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.AggregateMessage, problemDetails.Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, problemDetails.Status);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Aggregate, out var aggregate);
        Assert.NotNull(aggregate);

        var aggregateArray = Assert.IsAssignableFrom<IEnumerable<ProblemDetails>>(aggregate);
        Assert.Equal(3, aggregateArray.Count());

        Assert.Equal("Custom Problem 1", aggregateArray.ElementAt(0).Detail);
        Assert.Equal("Custom Problem 2", aggregateArray.ElementAt(1).Detail);
        Assert.Equal("Custom Problem 3", aggregateArray.ElementAt(2).Detail);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.NotFound, out var notFound);
        Assert.NotNull(notFound);

        var notFoundArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(notFound);
        Assert.Single(notFoundArray);
        Assert.Equal("Not Found", notFoundArray.ElementAt(0).Detail);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var errors);
        Assert.NotNull(errors);

        var errorsArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(errors);
        Assert.Equal(4, errorsArray.Count());

        Assert.Equal("Invalid Parameter", errorsArray.ElementAt(0).Detail);
        Assert.Equal("#/MyProperty1", errorsArray.ElementAt(0).Pointer);

        Assert.Equal("Validation Failed", errorsArray.ElementAt(1).Detail);
        Assert.Equal("#/MyProperty2", errorsArray.ElementAt(1).Pointer);

        Assert.Equal("Invalid State", errorsArray.ElementAt(2).Detail);
        Assert.Equal("Internal Server Error", errorsArray.ElementAt(3).Detail);
    }

    [Fact]
    public void Convert_NotFound_InvalidParameter_ValidationFailed_InvalidState_InternalError_ManyCustomError_Described()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription(
            "MyType1",
            "My Custom Problem 1",
            "Used for tests only 1",
            System.Net.HttpStatusCode.UnprocessableEntity));
        options.Descriptor.Add(new ProblemDetailsDescription(
            "MyType2",
            "My Custom Problem 2",
            "Used for tests only 2",
            System.Net.HttpStatusCode.Forbidden));
        options.Descriptor.Add(new ProblemDetailsDescription(
            "MyType3",
            "My Custom Problem 3",
            "Used for tests only 3",
            System.Net.HttpStatusCode.ExpectationFailed));

        Problems problems = Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State")
            + Problems.InternalError(new Exception("Internal Server Error"))
            + Problems.Custom("Custom Problem 1", "MyType1")
            + Problems.Custom("Custom Problem 2", "MyType2")
            + Problems.Custom("Custom Problem 3", "MyType3");

        // Act
        var problemDetails = problems.ToProblemDetails(options);

        // Assert
        Assert.Equal("tag:problemdetails/.problems#aggregate-problems-details", problemDetails.Type);
        Assert.Equal(ProblemDetailsDescriptor.Titles.AggregateProblemsDetailsTitle, problemDetails.Title);
        Assert.Equal(ProblemDetailsDescriptor.Messages.AggregateMessage, problemDetails.Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, problemDetails.Status);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Aggregate, out var aggregate);
        Assert.NotNull(aggregate);

        var aggregateArray = Assert.IsAssignableFrom<IEnumerable<ProblemDetails>>(aggregate);
        Assert.Equal(3, aggregateArray.Count());

        Assert.Equal("tag:problemdetails/.problems#MyType1", aggregateArray.ElementAt(0).Type);
        Assert.Equal("My Custom Problem 1", aggregateArray.ElementAt(0).Title);
        Assert.Equal("Custom Problem 1", aggregateArray.ElementAt(0).Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity, aggregateArray.ElementAt(0).Status);

        Assert.Equal("tag:problemdetails/.problems#MyType2", aggregateArray.ElementAt(1).Type);
        Assert.Equal("My Custom Problem 2", aggregateArray.ElementAt(1).Title);
        Assert.Equal("Custom Problem 2", aggregateArray.ElementAt(1).Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden, aggregateArray.ElementAt(1).Status);

        Assert.Equal("tag:problemdetails/.problems#MyType3", aggregateArray.ElementAt(2).Type);
        Assert.Equal("My Custom Problem 3", aggregateArray.ElementAt(2).Title);
        Assert.Equal("Custom Problem 3", aggregateArray.ElementAt(2).Detail);
        Assert.Equal(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, aggregateArray.ElementAt(2).Status);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.NotFound, out var notFound);
        Assert.NotNull(notFound);

        var notFoundArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(notFound);
        Assert.Single(notFoundArray);
        Assert.Equal("Not Found", notFoundArray.ElementAt(0).Detail);

        problemDetails.Extensions.TryGetValue(ProblemDetailsExtended.Fields.Errors, out var errors);
        Assert.NotNull(errors);

        var errorsArray = Assert.IsAssignableFrom<IEnumerable<ErrorDetails>>(errors);
        Assert.Equal(4, errorsArray.Count());

        Assert.Equal("Invalid Parameter", errorsArray.ElementAt(0).Detail);
        Assert.Equal("#/MyProperty1", errorsArray.ElementAt(0).Pointer);

        Assert.Equal("Validation Failed", errorsArray.ElementAt(1).Detail);
        Assert.Equal("#/MyProperty2", errorsArray.ElementAt(1).Pointer);

        Assert.Equal("Invalid State", errorsArray.ElementAt(2).Detail);
        Assert.Equal("Internal Server Error", errorsArray.ElementAt(3).Detail);
    }
}
