using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using RoyalCode.SmartProblems.Descriptions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ProblemDetailsServiceCollectionExtensions
{
    /// <summary>
    /// <para>
    ///     Add the <see cref="ProblemDetailsOptions"/> to the <see cref="IServiceCollection"/>.
    /// </para>
    /// <para>
    ///     Bind the configuration section <c>ProblemDetails</c> to the <see cref="ProblemDetailsOptions"/>.
    /// </para>
    /// <para>
    ///     Execute the <see cref="ProblemDetailsOptions.Complete(ILogger)"/> when the configuration is completed.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional, the action to configure the options.</param>
    /// <returns>Same instance of <paramref name="services"/> for chaining.</returns>
    public static IServiceCollection AddProblemDetailsDescriptions(this IServiceCollection services,
        Action<ProblemDetailsOptions>? configureOptions = null)
    {
        services.AddHttpContextAccessor();

        var builder = services.AddOptions<ProblemDetailsOptions>()
            .BindConfiguration("ProblemDetails")
            .PostConfigure<AspNetCore.Http.IHttpContextAccessor>((options, access) =>
            {
                if (options.BaseAddress == ProblemDetailsOptions.DefaultBaseAddress
                    && access.HttpContext?.Request is not null)
                {
                    options.BaseAddress = $"https://{access.HttpContext.Request.Host.Value}/.problems";
                }
            })
            .PostConfigure<ILogger<ProblemDetailsOptions>>((o, l) =>
            {
                // log completing the options configuration
                l.LogDebug("Completing the options configuration.");

                // complete the options configuration
                o.Complete(l);
            });

        if (configureOptions is not null)
            builder.Configure(configureOptions);

        return services;
    }
}
