using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using RoyalCode.SmartProblems.HttpResults;
using RoyalCode.SmartProblems.Metadata;
using System.Text.Json;

namespace RoyalCode.SmartProblems.Tests.HttpResults;

public class AcceptedMatchTests
{
    [Fact]
    public void AcceptedMatch_Implicit_Result_Success()
    {
        // Arrange
        var result = Result.Ok();

        // Act
        AcceptedMatch match = result;

        // Assert
        Assert.NotNull(match);
        Assert.IsType<Accepted>(match.Result);
    }

    [Fact]
    public void AcceptedMatch_Implicit_Problem()
    {
        // Arrange
        Problem problem = Problems.InvalidParameter("invalid");

        // Act
        AcceptedMatch match = problem;

        // Assert
        Assert.IsType<MatchErrorResult>(match.Result);
    }

    [Fact]
    public void AcceptedMatch_Implicit_Problems()
    {
        // Arrange
        Problems problems = [Problems.NotFound("missing")];

        // Act
        AcceptedMatch match = problems;

        // Assert
        Assert.IsType<MatchErrorResult>(match.Result);
    }

    [Fact]
    public void AcceptedMatch_Constructor_NullResult_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new AcceptedMatch((IResult)null!));
    }

    [Fact]
    public async Task AcceptedMatch_Execute_Success_WithoutLocation()
    {
        // Arrange
        AcceptedMatch match = new(Result.Ok());
        HttpContext context = Util.CreateHttpContext();

        // Act
        await match.ExecuteAsync(context);

        // Assert
        Assert.Equal(202, context.Response.StatusCode);
        Assert.False(context.Response.Headers.ContainsKey("Location"));
        Assert.Equal(string.Empty, context.GetBody());
    }

    [Fact]
    public async Task AcceptedMatch_Execute_Success_WithLocation()
    {
        // Arrange
        AcceptedMatch match = new(Result.Ok(), "/status/42");
        HttpContext context = Util.CreateHttpContext();

        // Act
        await match.ExecuteAsync(context);

        // Assert
        Assert.Equal(202, context.Response.StatusCode);
        Assert.Equal("/status/42", context.Response.Headers.Location);
        Assert.Equal(string.Empty, context.GetBody());
    }

    [Fact]
    public async Task AcceptedMatch_Execute_Problem_IsNever202()
    {
        // Arrange
        Result result = Problems.InvalidParameter("invalid");
        AcceptedMatch match = new(result, "/status/42");
        HttpContext context = Util.CreateHttpContext();

        // Act
        await match.ExecuteAsync(context);

        // Assert
        Assert.Equal(400, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.False(context.Response.Headers.ContainsKey("Location"));
    }

    [Fact]
    public void AcceptedMatch_Metadata_202_WithoutContentType()
    {
        // Arrange
        var builder = new RouteEndpointBuilder(null, RoutePatternFactory.Parse("/"), 0);

        // Act
        AcceptedMatch.PopulateMetadata(null!, builder);

        // Assert
        var metadata = builder.Metadata.OfType<ResponseTypeMetadata>().First(m => m.StatusCode == 202);
        Assert.Null(metadata.Type);
        Assert.Empty(metadata.ContentTypes);

        // the problems metadata is preserved
        Assert.Contains(builder.Metadata.OfType<IProducesResponseTypeMetadata>(), m => m.StatusCode != 202);
    }

    [Fact]
    public void AcceptedMatchT_Implicit_ResultT_Success()
    {
        // Arrange
        Result<TestValue> result = new TestValue { Name = "abc" };

        // Act
        AcceptedMatch<TestValue> match = result;

        // Assert
        Assert.IsType<Accepted<TestValue>>(match.Result);
    }

    [Fact]
    public void AcceptedMatchT_Implicit_ToNonGeneric_PreservesResult()
    {
        // Arrange
        Result<TestValue> result = new TestValue { Name = "abc" };
        AcceptedMatch<TestValue> match = result;

        // Act
        AcceptedMatch nonGeneric = match;

        // Assert
        Assert.Same(match.Result, nonGeneric.Result);
    }

    [Fact]
    public void AcceptedMatchT_Constructor_NullLocationFunction_Throws()
    {
        // Arrange
        Result<TestValue> result = new TestValue { Name = "abc" };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AcceptedMatch<TestValue>(result, (Func<TestValue, string?>)null!));
    }

    [Fact]
    public async Task AcceptedMatchT_Execute_Success_WithBody_WithoutLocation()
    {
        // Arrange
        Result<TestValue> result = new TestValue { Name = "abc" };
        AcceptedMatch<TestValue> match = new(result);
        HttpContext context = Util.CreateHttpContext();

        // Act
        await match.ExecuteAsync(context);

        // Assert
        Assert.Equal(202, context.Response.StatusCode);
        Assert.False(context.Response.Headers.ContainsKey("Location"));

        var value = JsonSerializer.Deserialize<TestValue>(context.GetBody(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(value);
        Assert.Equal("abc", value!.Name);
    }

    [Fact]
    public async Task AcceptedMatchT_Execute_Success_WithBody_AndLocationFromValue()
    {
        // Arrange
        Result<TestValue> result = new TestValue { Name = "abc" };
        AcceptedMatch<TestValue> match = new(result, v => $"/status/{v.Name}");
        HttpContext context = Util.CreateHttpContext();

        // Act
        await match.ExecuteAsync(context);

        // Assert
        Assert.Equal(202, context.Response.StatusCode);
        Assert.Equal("/status/abc", context.Response.Headers.Location);

        var value = JsonSerializer.Deserialize<TestValue>(context.GetBody(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(value);
        Assert.Equal("abc", value!.Name);
    }

    [Fact]
    public async Task AcceptedMatchT_Execute_Problem_IsNever202_AndFunctionNotInvoked()
    {
        // Arrange
        Result<TestValue> result = Problems.NotFound("missing");
        var invoked = false;
        AcceptedMatch<TestValue> match = new(result, v =>
        {
            invoked = true;
            return "/status/x";
        });
        HttpContext context = Util.CreateHttpContext();

        // Act
        await match.ExecuteAsync(context);

        // Assert
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.False(invoked);
    }

    [Fact]
    public void AcceptedMatchT_Metadata_202_WithJsonBody()
    {
        // Arrange
        var builder = new RouteEndpointBuilder(null, RoutePatternFactory.Parse("/"), 0);

        // Act
        AcceptedMatch<TestValue>.PopulateMetadata(null!, builder);

        // Assert
        var metadata = builder.Metadata.OfType<ResponseTypeMetadata>().First(m => m.StatusCode == 202);
        Assert.Equal(typeof(TestValue), metadata.Type);
        Assert.Contains("application/json", metadata.ContentTypes);
    }

    [Fact]
    public async Task AcceptedMatch_Extensions_ProduceSameBehavior()
    {
        // Arrange
        var match = Result.Ok().AcceptedMatch("/status/7");
        HttpContext context = Util.CreateHttpContext();

        // Act
        await match.ExecuteAsync(context);

        // Assert
        Assert.Equal(202, context.Response.StatusCode);
        Assert.Equal("/status/7", context.Response.Headers.Location);
    }

    public class TestValue
    {
        public string Name { get; set; } = string.Empty;
    }
}
