using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RoyalCode.SmartProblems.Tests;

internal static class Util
{
    public static IServiceProvider CreateServiceProvider(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddOptions();
        services.AddHttpContextAccessor();
        services.AddLogging();

        services.AddProblemDetailsDescriptions();

        configure?.Invoke(services);

        return services.BuildServiceProvider();
    }

    public static DefaultHttpContext CreateHttpContext(Action<IServiceCollection>? configure = null)
    {
        var sp = CreateServiceProvider(configure);
        var scope = sp.CreateScope();

        return new DefaultHttpContext()
        {
            RequestServices = scope.ServiceProvider,
            Response =
            {
                Body = new MemoryStream()
            }
        };
    }

    public static string GetBody(this HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        return new StreamReader(context.Response.Body).ReadToEnd();
    }
}
