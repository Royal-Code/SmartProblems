
using System.Net;

namespace RoyalCode.SmartProblems.Tests.Http;

public class ApiResultsTests : IClassFixture<AppFixture>
{
    private readonly AppFixture fixture;

    public ApiResultsTests(AppFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task Api_NotFound()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/not-found");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Single(problems);
        Assert.Equal("Not Found", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[0].Category);
    }

    [Fact]
    public async Task Api_InvalidParameter()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/invalid-parameter");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Single(problems);
        Assert.Equal("Invalid Parameter", problems[0].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[0].Category);
        Assert.Equal("MyProperty", problems[0].Property);
    }

    [Fact]
    public async Task Api_ValidationFailed()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/validation-failed");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Single(problems);
        Assert.Equal("Validation Failed", problems[0].Detail);
        Assert.Equal(ProblemCategory.ValidationFailed, problems[0].Category);
        Assert.Equal("MyProperty", problems[0].Property);
    }

    [Fact]
    public async Task Api_InvalidState()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/invalid-state");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Single(problems);
        Assert.Equal("Invalid State", problems[0].Detail);
        Assert.Equal(ProblemCategory.InvalidState, problems[0].Category);
    }

    [Fact]
    public async Task Api_NotAllowed()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/not-allowed");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Single(problems);
        Assert.Equal("Not Allowed", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotAllowed, problems[0].Category);
    }

    [Fact]
    public async Task Api_InternalServerError()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/internal-server-error");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Single(problems);
        Assert.Equal("Internal Server Error", problems[0].Detail);
        Assert.Equal(ProblemCategory.InternalServerError, problems[0].Category);
    }

    [Fact]
    public async Task Api_CustomProblem()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/custom-problem");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.ExpectationFailed, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Single(problems);
        Assert.Equal("Custom Problem", problems[0].Detail);
        Assert.Equal(ProblemCategory.CustomProblem, problems[0].Category);
    }

    [Fact]
    public async Task Api_Many_NotFound()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/many-not-found");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Equal(3, problems.Count);
        Assert.Equal("Not Found 1", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[0].Category);
        Assert.Equal("Not Found 2", problems[1].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[1].Category);
        Assert.Equal("Not Found 3", problems[2].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[2].Category);
    }

    [Fact]
    public async Task Api_Many_InvalidParameter()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/many-invalid-parameter");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Equal(3, problems.Count);
        Assert.Equal("Invalid Parameter 1", problems[0].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[0].Category);
        Assert.Equal("MyProperty1", problems[0].Property);
        Assert.Equal("Invalid Parameter 2", problems[1].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[1].Category);
        Assert.Equal("MyProperty2", problems[1].Property);
        Assert.Equal("Invalid Parameter 3", problems[2].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[2].Category);
        Assert.Equal("MyProperty3", problems[2].Property);
    }

    [Fact]
    public async Task Api_Many_ValidationFailed()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/many-validation-failed");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Equal(3, problems.Count);
        Assert.Equal("Validation Failed 1", problems[0].Detail);
        Assert.Equal(ProblemCategory.ValidationFailed, problems[0].Category);
        Assert.Equal("MyProperty1", problems[0].Property);
        Assert.Equal("Validation Failed 2", problems[1].Detail);
        Assert.Equal(ProblemCategory.ValidationFailed, problems[1].Category);
        Assert.Equal("MyProperty2", problems[1].Property);
        Assert.Equal("Validation Failed 3", problems[2].Detail);
        Assert.Equal(ProblemCategory.ValidationFailed, problems[2].Category);
        Assert.Equal("MyProperty3", problems[2].Property);
    }

    [Fact]
    public async Task Api_Many_InvalidState()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/many-invalid-state");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Equal(3, problems.Count);
        Assert.Equal("Invalid State 1", problems[0].Detail);
        Assert.Equal(ProblemCategory.InvalidState, problems[0].Category);
        Assert.Equal("Invalid State 2", problems[1].Detail);
        Assert.Equal(ProblemCategory.InvalidState, problems[1].Category);
        Assert.Equal("Invalid State 3", problems[2].Detail);
        Assert.Equal(ProblemCategory.InvalidState, problems[2].Category);
    }

    [Fact]
    public async Task Api_Many_NotAllowed()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/many-not-allowed");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Equal(3, problems.Count);
        Assert.Equal("Not Allowed 1", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotAllowed, problems[0].Category);
        Assert.Equal("Not Allowed 2", problems[1].Detail);
        Assert.Equal(ProblemCategory.NotAllowed, problems[1].Category);
        Assert.Equal("Not Allowed 3", problems[2].Detail);
        Assert.Equal(ProblemCategory.NotAllowed, problems[2].Category);
    }

    [Fact]
    public async Task Api_Many_InternalServerError()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/many-internal-server-error");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Equal(3, problems.Count);
        Assert.Equal("Internal Server Error 1", problems[0].Detail);
        Assert.Equal(ProblemCategory.InternalServerError, problems[0].Category);
        Assert.Equal("Internal Server Error 2", problems[1].Detail);
        Assert.Equal(ProblemCategory.InternalServerError, problems[1].Category);
        Assert.Equal("Internal Server Error 3", problems[2].Detail);
        Assert.Equal(ProblemCategory.InternalServerError, problems[2].Category);
    }

    [Fact]
    public async Task Api_Many_CustomProblem()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/many-custom-problem");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.ExpectationFailed, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));

        Assert.Equal(3, problems.Count);
        Assert.Equal("Custom Problem 1", problems[0].Detail);
        Assert.Equal(ProblemCategory.CustomProblem, problems[0].Category);
        Assert.Equal("Custom Problem 2", problems[1].Detail);
        Assert.Equal(ProblemCategory.CustomProblem, problems[1].Category);
        Assert.Equal("Custom Problem 3", problems[2].Detail);
        Assert.Equal(ProblemCategory.CustomProblem, problems[2].Category);
    }

    [Fact]
    public async Task Api_NotFound_InvalidParameter()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/not-found-invalid-parameter");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.Equal(2, problems.Count);

        Assert.Equal("Not Found", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[0].Category);

        Assert.Equal("Invalid Parameter", problems[1].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[1].Category);
        Assert.Equal("MyProperty", problems[1].Property);
    }

    [Fact]
    public async Task Api_NotFound_InvalidParameter_ValidationFailed()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/not-found-invalid-parameter-validation-failed");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.Equal(3, problems.Count);

        Assert.Equal("Not Found", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[0].Category);

        Assert.Equal("Invalid Parameter", problems[1].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[1].Category);
        Assert.Equal("MyProperty1", problems[1].Property);
        
        Assert.Equal("Validation Failed", problems[2].Detail);
        Assert.Equal(ProblemCategory.ValidationFailed, problems[2].Category);
        Assert.Equal("MyProperty2", problems[2].Property);
    }

    [Fact]
    public async Task Api_NotFound_InvalidParameter_ValidationFailed_InvalidState()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/not-found-invalid-parameter-validation-failed-invalid-state");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.Equal(4, problems.Count);

        Assert.Equal("Not Found", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[0].Category);

        Assert.Equal("Invalid Parameter", problems[1].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[1].Category);
        Assert.Equal("MyProperty1", problems[1].Property);
        
        Assert.Equal("Validation Failed", problems[2].Detail);
        Assert.Equal(ProblemCategory.ValidationFailed, problems[2].Category);
        Assert.Equal("MyProperty2", problems[2].Property);

        Assert.Equal("Invalid State", problems[3].Detail);
        Assert.Equal(ProblemCategory.InvalidState, problems[3].Category);
    }

    [Fact]
    public async Task Api_NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/not-found-invalid-parameter-validation-failed-invalid-state-not-allowed");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.Equal(5, problems.Count);

        Assert.Equal("Not Found", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[0].Category);

        Assert.Equal("Invalid Parameter", problems[1].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[1].Category);
        Assert.Equal("MyProperty1", problems[1].Property);
        
        Assert.Equal("Validation Failed", problems[2].Detail);
        Assert.Equal(ProblemCategory.ValidationFailed, problems[2].Category);
        Assert.Equal("MyProperty2", problems[2].Property);

        Assert.Equal("Invalid State", problems[3].Detail);
        Assert.Equal(ProblemCategory.InvalidState, problems[3].Category);

        Assert.Equal("Not Allowed", problems[4].Detail);
        Assert.Equal(ProblemCategory.NotAllowed, problems[4].Category);
    }

    [Fact]
    public async Task Api_NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed_InternalServerError()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/not-found-invalid-parameter-validation-failed-invalid-state-not-allowed-internal-server-error");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.Equal(6, problems.Count);

        Assert.Equal("Not Found", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[0].Category);

        Assert.Equal("Invalid Parameter", problems[1].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[1].Category);
        Assert.Equal("MyProperty1", problems[1].Property);
        
        Assert.Equal("Validation Failed", problems[2].Detail);
        Assert.Equal(ProblemCategory.ValidationFailed, problems[2].Category);
        Assert.Equal("MyProperty2", problems[2].Property);

        Assert.Equal("Invalid State", problems[3].Detail);
        Assert.Equal(ProblemCategory.InvalidState, problems[3].Category);

        Assert.Equal("Not Allowed", problems[4].Detail);
        Assert.Equal(ProblemCategory.NotAllowed, problems[4].Category);

        Assert.Equal("Internal Server Error", problems[5].Detail);
        Assert.Equal(ProblemCategory.InternalServerError, problems[5].Category);
    }

    [Fact]
    public async Task Api_NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed_InternalServerError_CustomProblem()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/not-found-invalid-parameter-validation-failed-invalid-state-not-allowed-internal-server-error-custom-problem");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.ExpectationFailed, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.Equal(7, problems.Count);

        Assert.Equal("Not Found", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[0].Category);

        Assert.Equal("Invalid Parameter", problems[1].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[1].Category);
        Assert.Equal("MyProperty1", problems[1].Property);
        
        Assert.Equal("Validation Failed", problems[2].Detail);
        Assert.Equal(ProblemCategory.ValidationFailed, problems[2].Category);
        Assert.Equal("MyProperty2", problems[2].Property);

        Assert.Equal("Invalid State", problems[3].Detail);
        Assert.Equal(ProblemCategory.InvalidState, problems[3].Category);

        Assert.Equal("Not Allowed", problems[4].Detail);
        Assert.Equal(ProblemCategory.NotAllowed, problems[4].Category);

        Assert.Equal("Internal Server Error", problems[5].Detail);
        Assert.Equal(ProblemCategory.InternalServerError, problems[5].Category);

        Assert.Equal("Custom Problem", problems[6].Detail);
        Assert.Equal(ProblemCategory.CustomProblem, problems[6].Category);
    }

    [Fact]
    public async Task Api_NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed_InternalServerError_CustomProblem_Many()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/problems/not-found-invalid-parameter-validation-failed-invalid-state-not-allowed-internal-server-error-custom-problem-many");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(HttpStatusCode.ExpectationFailed, response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.Equal(9, problems.Count);

        Assert.Equal("Not Found", problems[0].Detail);
        Assert.Equal(ProblemCategory.NotFound, problems[0].Category);

        Assert.Equal("Invalid Parameter", problems[1].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[1].Category);
        Assert.Equal("MyProperty1", problems[1].Property);
        
        Assert.Equal("Validation Failed", problems[2].Detail);
        Assert.Equal(ProblemCategory.ValidationFailed, problems[2].Category);
        Assert.Equal("MyProperty2", problems[2].Property);

        Assert.Equal("Invalid State", problems[3].Detail);
        Assert.Equal(ProblemCategory.InvalidState, problems[3].Category);

        Assert.Equal("Not Allowed", problems[4].Detail);
        Assert.Equal(ProblemCategory.NotAllowed, problems[4].Category);

        Assert.Equal("Internal Server Error", problems[5].Detail);
        Assert.Equal(ProblemCategory.InternalServerError, problems[5].Category);

        Assert.Equal("Custom Problem 1", problems[6].Detail);
        Assert.Equal(ProblemCategory.CustomProblem, problems[6].Category);

        Assert.Equal("Custom Problem 2", problems[7].Detail);
        Assert.Equal(ProblemCategory.CustomProblem, problems[7].Category);

        Assert.Equal("Custom Problem 3", problems[8].Detail);
        Assert.Equal(ProblemCategory.CustomProblem, problems[8].Category);
    }
}
