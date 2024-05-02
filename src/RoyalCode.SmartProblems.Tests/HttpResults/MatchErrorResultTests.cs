using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalCode.SmartProblems.HttpResults;
using System.Text.Json;

namespace RoyalCode.SmartProblems.Tests.HttpResults;

public class MatchErrorResultTests
{
    [Fact]
    public void MatchErrorResult_Implicit_Problems()
    {
        // Arrange
        Problems problems = [Problems.InvalidParameter("invalid")];

        // Act
        MatchErrorResult result = problems;

        // Assert
        Assert.NotNull(result);
        Assert.Same(problems, result.Value);
    }

    [Fact]
    public void MatchErrorResult_Implicit_Problem()
    {
        // Arrange
        Problem problem = Problems.InvalidParameter("invalid");

        // Act
        MatchErrorResult result = problem;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        var problems = Assert.IsType<Problems>(result.Value);
        Assert.Single(problems);
        Assert.Same(problem, problems[0]);
    }

    [Fact]
    public void MatchErrorResult_Implicit_Exception()
    {
        // Arrange
        Exception ex = new("error");

        // Act
        MatchErrorResult result = ex;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        var problems = Assert.IsType<Problems>(result.Value);
        Assert.Single(problems);
        Assert.Equal("error", problems[0].Detail);
    }

    [Fact]
    public async Task MatchErrorResult_Execute()
    {
        // Arrange
        var problem = Problems.InvalidParameter("invalid");
        MatchErrorResult result = problem;

        HttpContext context = Util.CreateHttpContext();

        // Act
        await result.ExecuteAsync(context);

        // Assert
        Assert.Equal(400, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        var json = context.GetBody();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.NotNull(problemDetails);
        Assert.Equal(400, problemDetails.Status);
        Assert.Equal("invalid", problemDetails.Detail);
    }
}
