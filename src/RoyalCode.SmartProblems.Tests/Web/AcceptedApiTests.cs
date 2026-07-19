using RoyalCode.SmartProblems.Tests.Http;
using RoyalCode.SmartProblems.TestsApi.Apis;
using System.Net.Http.Json;

namespace RoyalCode.SmartProblems.Tests.Web;

public class AcceptedApiTests(AppFixture fixture) : IClassFixture<AppFixture>
{
    [Fact]
    public async Task Queue_Success_Returns202_WithoutBody_WithoutLocation()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/accepted/queue", new { fail = false });

        // Assert
        Assert.Equal(202, (int)response.StatusCode);
        Assert.Null(response.Headers.Location);
        Assert.Equal(0, response.Content.Headers.ContentLength ?? 0);
    }

    [Fact]
    public async Task Queue_Problem_ReturnsProblemDetails_Never202()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/accepted/queue", new { fail = true });
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.Single(problems!);
        Assert.Equal(ProblemCategory.InvalidParameter, problems![0].Category);
    }

    [Fact]
    public async Task QueueLocated_Success_Returns202_WithLocation_WithoutBody()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/accepted/queue-located", new { fail = false });

        // Assert
        Assert.Equal(202, (int)response.StatusCode);
        Assert.Equal("/api/accepted/status/fixed", response.Headers.Location?.ToString());
        Assert.Equal(0, response.Content.Headers.ContentLength ?? 0);
    }

    [Fact]
    public async Task Ticket_Success_Returns202_WithJsonBody_WithoutLocation()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/accepted/ticket", new { fail = false });
        var body = await response.Content.ReadFromJsonAsync<TicketDetails>();

        // Assert
        Assert.Equal(202, (int)response.StatusCode);
        Assert.Null(response.Headers.Location);
        Assert.NotNull(body);
        Assert.Equal("T-42", body!.Ticket);
        Assert.Equal("queued", body.Status);
    }

    [Fact]
    public async Task TicketLocated_Success_Returns202_WithJsonBody_AndLocationFromValue()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/accepted/ticket-located", new { fail = false });
        var body = await response.Content.ReadFromJsonAsync<TicketDetails>();

        // Assert
        Assert.Equal(202, (int)response.StatusCode);
        Assert.Equal("/api/accepted/status/T-42", response.Headers.Location?.ToString());
        Assert.NotNull(body);
        Assert.Equal("T-42", body!.Ticket);
    }

    [Fact]
    public async Task TicketLocated_Problem_ReturnsProblemDetails_WithoutLocation()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/accepted/ticket-located", new { fail = true });
        var result = await response.ToResultAsync<TicketDetails>();

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.Null(response.Headers.Location);
        Assert.True(result.HasProblems(out var problems));
        Assert.Equal(ProblemCategory.InvalidParameter, problems![0].Category);
    }
}
