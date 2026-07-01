using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.SmartProblems.Descriptions;
using RoyalCode.SmartProblems.Tests.Http;

namespace RoyalCode.SmartProblems.Tests.Web;

public class ProblemDetailsDescriptionPageTests(AppFixture fixture) : IClassFixture<AppFixture>
{
    [Fact]
    public async Task GetDefaultPage_ReturnsHtmlWithConfiguredDescriptions()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/.problems");
        var html = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("my-custom-type", html);
        Assert.Contains("not-found", html);
        Assert.Contains("problem-aggregate-problems-details", html);
    }

    [Fact]
    public async Task GetCustomPattern_ReturnsHtml()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddProblemDetailsDescriptions(options =>
        {
            options.Descriptor.Add(new ProblemDetailsDescription(
                "custom-route-type",
                "Custom route type",
                "A problem detail type exposed by a custom documentation route.",
                HttpStatusCode.Conflict));
        });

        await using var app = builder.Build();
        app.MapProblemDetailsDescriptionPage("/problems/catalog");

        await app.StartAsync();
        var client = app.GetTestClient();

        // Act
        var response = await client.GetAsync("/problems/catalog");
        var html = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("custom-route-type", html);
        Assert.Contains("http://localhost/problems/catalog#custom-route-type", html);
    }
}
