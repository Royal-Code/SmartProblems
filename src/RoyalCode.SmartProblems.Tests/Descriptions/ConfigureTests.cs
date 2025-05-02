using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoyalCode.SmartProblems.Descriptions;

namespace RoyalCode.SmartProblems.Tests.Descriptions;

public class ConfigureTests
{
    [Fact]
    public void Descriptor_Configure_FromJsonFile()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
            .AddHttpContextAccessor()
            .AddLogging()
            .AddProblemDetailsDescriptions(options =>
        {
            options.Descriptor.AddFromJsonFile("problem-details.json");
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ProblemDetailsOptions>>().Value;

        // Act
        var found = options.Descriptor.TryGetDescription("my-custom-type", out var description);

        // Assert
        Assert.True(found);
        Assert.NotNull(description);
        Assert.Equal("My custom problem type", description.Title);
    }
}
