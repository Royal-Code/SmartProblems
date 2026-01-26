using RoyalCode.SmartProblems.Tests.Http;
using RoyalCode.SmartProblems.TestsApi.Apis;
using System.Net.Http.Json;

namespace RoyalCode.SmartProblems.Tests.Web;

public class MatchApiTests(AppFixture fixture) : IClassFixture<AppFixture>
{
    [Fact]
    public async Task CreatePerson_Success_Returns201_WithLocation_And_Body()
    {
        // Arrange
        var client = fixture.CreateClient();
        var input = new { name = "John", age = 20 };

        // Act
        var response = await client.PostAsJsonAsync("/api/match", input);
        var content = await response.Content.ReadFromJsonAsync<PersonDetails>();

        // Assert
        Assert.Equal(201, (int)response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Location", out var locationHdr));
        Assert.Contains(locationHdr, l => l.StartsWith("/api/match/"));
        Assert.NotNull(content);
        Assert.Equal("John", content!.Name);
        Assert.Equal(20, content.Age);
    }

    [Fact]
    public async Task CreatePerson_InvalidInput_Returns400_WithProblemDetails()
    {
        // Arrange
        var client = fixture.CreateClient();
        var input = new { name = "", age = 10 }; // invalid: empty name, age < 16

        // Act
        var response = await client.PostAsJsonAsync("/api/match", input);
        var result = await response.ToResultAsync<PersonDetails>();

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.True(problems!.Count >= 1);
    }

    [Fact]
    public async Task GetPerson_NotFound_Returns404_ProblemDetails()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/match/999");
        var result = await response.ToResultAsync<PersonDetails>();

        // Assert
        Assert.Equal(404, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Single(problems!);
        Assert.Equal(ProblemCategory.NotFound, problems![0].Category);
    }

    [Fact]
    public async Task GetPerson_AfterCreate_Returns200_WithBody()
    {
        // Arrange
        var client = fixture.CreateClient();
        var input = new { name = "Mary", age = 22 };
        var createResp = await client.PostAsJsonAsync("/api/match", input);
        var created = await createResp.Content.ReadFromJsonAsync<PersonDetails>();
        Assert.NotNull(created);

        // Act
        var response = await client.GetAsync($"/api/match/{created!.Id}");
        var body = await response.Content.ReadFromJsonAsync<PersonDetails>();

        // Assert
        Assert.Equal(200, (int)response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Mary", body!.Name);
        Assert.Equal(22, body.Age);
    }

    [Fact]
    public async Task UpdatePersonName_Success_Returns200()
    {
        // Arrange
        var client = fixture.CreateClient();
        var input = new { name = "Tom", age = 30 };
        var createResp = await client.PostAsJsonAsync("/api/match", input);
        var created = await createResp.Content.ReadFromJsonAsync<PersonDetails>();
        Assert.NotNull(created);

        // Act
        var patchResp = await client.PatchAsJsonAsync($"/api/match/{created!.Id}/name", new { name = "Tommy" });

        // Assert
        Assert.Equal(200, (int)patchResp.StatusCode);
    }

    [Fact]
    public async Task UpdatePersonName_Invalid_Returns400_ProblemDetails()
    {
        // Arrange
        var client = fixture.CreateClient();
        var input = new { name = "Ana", age = 18 };
        var createResp = await client.PostAsJsonAsync("/api/match", input);
        var created = await createResp.Content.ReadFromJsonAsync<PersonDetails>();
        Assert.NotNull(created);

        // Act
        var patchResp = await client.PatchAsJsonAsync($"/api/match/{created!.Id}/name", new { name = "" });
        var result = await patchResp.ToResultAsync();

        // Assert
        Assert.Equal(400, (int)patchResp.StatusCode);
        Assert.True(result.HasProblems(out var problems));
    }

    [Fact]
    public async Task UpdatePersonAge_Success_Returns200()
    {
        // Arrange
        var client = fixture.CreateClient();
        var input = new { name = "Leo", age = 25 };
        var createResp = await client.PostAsJsonAsync("/api/match", input);
        var created = await createResp.Content.ReadFromJsonAsync<PersonDetails>();
        Assert.NotNull(created);

        // Act
        var patchResp = await client.PatchAsJsonAsync($"/api/match/{created!.Id}/age", new { age = 40 });

        // Assert
        Assert.Equal(200, (int)patchResp.StatusCode);
    }

    [Fact]
    public async Task UpdatePersonAge_Invalid_Returns400_ProblemDetails()
    {
        // Arrange
        var client = fixture.CreateClient();
        var input = new { name = "Zoe", age = 20 };
        var createResp = await client.PostAsJsonAsync("/api/match", input);
        var created = await createResp.Content.ReadFromJsonAsync<PersonDetails>();
        Assert.NotNull(created);

        // Act
        var patchResp = await client.PatchAsJsonAsync($"/api/match/{created!.Id}/age", new { age = 10 });
        var result = await patchResp.ToResultAsync();

        // Assert
        Assert.Equal(400, (int)patchResp.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
    }

    [Fact]
    public async Task DeletePerson_Success_Returns204()
    {
        // Arrange
        var client = fixture.CreateClient();
        var input = new { name = "Rick", age = 28 };
        var createResp = await client.PostAsJsonAsync("/api/match", input);
        var created = await createResp.Content.ReadFromJsonAsync<PersonDetails>();
        Assert.NotNull(created);

        // Act
        var deleteResp = await client.DeleteAsync($"/api/match/{created!.Id}");

        // Assert
        Assert.Equal(204, (int)deleteResp.StatusCode);
    }

    [Fact]
    public async Task DeletePerson_NotFound_Returns404_ProblemDetails()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var deleteResp = await client.DeleteAsync("/api/match/999");
        var result = await deleteResp.ToResultAsync();

        // Assert
        Assert.Equal(404, (int)deleteResp.StatusCode);
        Assert.True(result.HasProblems(out var problems));
    }
}
