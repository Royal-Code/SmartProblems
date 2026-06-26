using RoyalCode.SmartProblems.Tests.Http;
using RoyalCode.SmartProblems.TestsApi.Apis;
using System.Net.Http.Json;

namespace RoyalCode.SmartProblems.Tests.Web;

public class ContactsApiTests(AppFixture fixture) : IClassFixture<AppFixture>
{
    [Fact]
    public async Task CreateContact_Success_Returns201_WithLocation_And_Body()
    {
        // Arrange
        var client = fixture.CreateClient();
        var rawEmail = $"  {NewEmail().ToUpperInvariant()}  ";
        var input = new
        {
            name = "  John Doe  ",
            email = rawEmail,
            phone = " 123456789 "
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/contacts", input);
        var content = await response.Content.ReadFromJsonAsync<ContactDetails>();

        // Assert
        Assert.Equal(201, (int)response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Location", out var locationHdr));
        Assert.Contains(locationHdr, l => l.StartsWith("/api/contacts/"));

        Assert.NotNull(content);
        Assert.True(content!.Id > 0);
        Assert.Equal("John Doe", content.Name);
        Assert.Equal(rawEmail.Trim().ToLowerInvariant(), content.Email);
        Assert.Equal("123456789", content.Phone);
        Assert.NotEqual(default, content.CreatedAt);
    }

    [Fact]
    public async Task CreateContact_InvalidInput_Returns400_WithInvalidParameterProblems()
    {
        // Arrange
        var client = fixture.CreateClient();
        var input = new
        {
            name = "",
            email = "invalid-email",
            phone = new string('9', 31)
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/contacts", input);
        var result = await response.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.True(problems!.Count >= 2);
        Assert.All(problems, p => Assert.Equal(ProblemCategory.InvalidParameter, p.Category));
    }

    [Fact]
    public async Task CreateContact_DuplicateEmail_Returns409_InvalidState()
    {
        // Arrange
        var client = fixture.CreateClient();
        var email = NewEmail();

        var first = new { name = "First", email, phone = "111" };
        var second = new { name = "Second", email = $" {email.ToUpperInvariant()} ", phone = "222" };

        // Act
        var firstResponse = await client.PostAsJsonAsync("/api/contacts", first);
        var secondResponse = await client.PostAsJsonAsync("/api/contacts", second);
        var secondResult = await secondResponse.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(201, (int)firstResponse.StatusCode);
        Assert.Equal(409, (int)secondResponse.StatusCode);
        Assert.True(secondResult.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.InvalidState);
    }

    [Fact]
    public async Task GetContact_NotFound_Returns404_ProblemDetails()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/contacts/{int.MaxValue}");
        var result = await response.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(404, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.NotFound);
    }

    [Fact]
    public async Task GetContact_InvalidId_Returns400_InvalidParameter()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.GetAsync("/api/contacts/0");
        var result = await response.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.InvalidParameter);
    }

    [Fact]
    public async Task UpdateContact_Success_Returns200_WithUpdatedBody()
    {
        // Arrange
        var client = fixture.CreateClient();
        var created = await CreateContactAsync(client, phone: "111");

        // Act
        var updateResponse = await client.PatchAsJsonAsync($"/api/contacts/{created.Id}", new
        {
            name = "  Updated Name  ",
            phone = " 9988 "
        });

        var updated = await updateResponse.Content.ReadFromJsonAsync<ContactDetails>();

        // Assert
        Assert.Equal(200, (int)updateResponse.StatusCode);
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated!.Id);
        Assert.Equal("Updated Name", updated.Name);
        Assert.Equal(created.Email, updated.Email);
        Assert.Equal("9988", updated.Phone);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateContact_ClearPhone_Returns200_WithNullPhone()
    {
        // Arrange
        var client = fixture.CreateClient();
        var created = await CreateContactAsync(client, phone: "123");

        // Act
        var updateResponse = await client.PatchAsJsonAsync($"/api/contacts/{created.Id}", new
        {
            clearPhone = true
        });

        var updated = await updateResponse.Content.ReadFromJsonAsync<ContactDetails>();

        // Assert
        Assert.Equal(200, (int)updateResponse.StatusCode);
        Assert.NotNull(updated);
        Assert.Null(updated!.Phone);
    }

    [Fact]
    public async Task UpdateContact_InvalidPayload_Returns400_ProblemDetails()
    {
        // Arrange
        var client = fixture.CreateClient();
        var created = await CreateContactAsync(client);

        // Act
        var updateResponse = await client.PatchAsJsonAsync($"/api/contacts/{created.Id}", new { });
        var result = await updateResponse.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(400, (int)updateResponse.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.InvalidParameter);
    }

    [Fact]
    public async Task UpdateContact_DuplicateEmail_Returns409_InvalidState()
    {
        // Arrange
        var client = fixture.CreateClient();
        var first = await CreateContactAsync(client, email: NewEmail());
        var second = await CreateContactAsync(client, email: NewEmail());

        // Act
        var updateResponse = await client.PatchAsJsonAsync($"/api/contacts/{second.Id}", new
        {
            email = $" {first.Email.ToUpperInvariant()} "
        });

        var result = await updateResponse.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(409, (int)updateResponse.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.InvalidState);
    }

    [Fact]
    public async Task DeleteContact_Success_Returns204_AndThen404()
    {
        // Arrange
        var client = fixture.CreateClient();
        var created = await CreateContactAsync(client);

        // Act
        var deleteResponse = await client.DeleteAsync($"/api/contacts/{created.Id}");
        var getResponse = await client.GetAsync($"/api/contacts/{created.Id}");
        var getResult = await getResponse.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(204, (int)deleteResponse.StatusCode);
        Assert.Equal(404, (int)getResponse.StatusCode);
        Assert.True(getResult.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.NotFound);
    }

    [Fact]
    public async Task DeleteContact_NotFound_Returns404_ProblemDetails()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/contacts/{int.MaxValue}");
        var result = await response.ToResultAsync();

        // Assert
        Assert.Equal(404, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.NotFound);
    }

    [Fact]
    public async Task UpdateContact_NotFound_Returns404_ProblemDetails()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.PatchAsJsonAsync($"/api/contacts/{int.MaxValue}", new
        {
            name = "Any Name"
        });
        var result = await response.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(404, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.NotFound);
    }

    [Fact]
    public async Task UpdateContact_InvalidId_Returns400_InvalidParameter()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.PatchAsJsonAsync("/api/contacts/0", new
        {
            name = "Valid Name"
        });
        var result = await response.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.InvalidParameter);
    }

    [Fact]
    public async Task UpdateContact_InvalidEmail_Returns400_InvalidParameter()
    {
        // Arrange
        var client = fixture.CreateClient();
        var created = await CreateContactAsync(client);

        // Act
        var response = await client.PatchAsJsonAsync($"/api/contacts/{created.Id}", new
        {
            email = "invalid-email"
        });
        var result = await response.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.InvalidParameter);
    }

    [Fact]
    public async Task UpdateContact_InvalidPhone_Returns400_InvalidParameter()
    {
        // Arrange
        var client = fixture.CreateClient();
        var created = await CreateContactAsync(client);

        // Act
        var response = await client.PatchAsJsonAsync($"/api/contacts/{created.Id}", new
        {
            phone = new string('9', 31)
        });
        var result = await response.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.InvalidParameter);
    }

    [Fact]
    public async Task UpdateContact_InvalidName_Returns400_InvalidParameter()
    {
        // Arrange
        var client = fixture.CreateClient();
        var created = await CreateContactAsync(client);

        // Act
        var response = await client.PatchAsJsonAsync($"/api/contacts/{created.Id}", new
        {
            name = "   "
        });
        var result = await response.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
        Assert.Contains(problems!, p => p.Category == ProblemCategory.InvalidParameter);
    }

    [Fact]
    public async Task CreateContact_EmptyPayload_Returns400_ProblemDetails()
    {
        // Arrange
        var client = fixture.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/contacts", new { });
        var result = await response.ToResultAsync<ContactDetails>();

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.True(result.HasProblems(out var problems));
        Assert.NotNull(problems);
    }

    private static string NewEmail()
    {
        return $"contact-{Guid.NewGuid():N}@tests.local";
    }

    private static async Task<ContactDetails> CreateContactAsync(HttpClient client,
        string? name = null,
        string? email = null,
        string? phone = "555")
    {
        var response = await client.PostAsJsonAsync("/api/contacts", new
        {
            name = name ?? "Contact Test",
            email = email ?? NewEmail(),
            phone
        });

        var body = await response.Content.ReadFromJsonAsync<ContactDetails>();

        Assert.Equal(201, (int)response.StatusCode);
        Assert.NotNull(body);

        return body!;
    }
}