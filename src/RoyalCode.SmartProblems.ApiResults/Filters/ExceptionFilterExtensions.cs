using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoyalCode.SmartProblems.Filters;
using RoyalCode.SmartProblems.HttpResults;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extensions methods for <see cref="IEndpointConventionBuilder"/>.
/// </summary>
public static class ExceptionFilterExtensions
{
    /// <summary>
    /// Adds a filter to the MinimalApi pipeline to treat exceptions as Problem Details.
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="ecb">The <see cref="IEndpointConventionBuilder"/>.</param>
    /// <param name="logLevel">The log level, used to log <see cref="MatchErrorResult"/> when returned by the API.</param>
    /// <param name="loggerType">A type, for create the <see cref="ILogger"/> from the <see cref="ILoggerFactory"/>.</param>
    /// <returns>The same instance of <paramref name="ecb"/>.</returns>
    public static TBuilder WithExceptionFilter<TBuilder>(
        this TBuilder ecb,
        LogLevel logLevel = LogLevel.None,
        Type? loggerType = null) where TBuilder : IEndpointConventionBuilder
    {
        ecb.Add(builder =>
        {
            var displayName = builder.DisplayName;
            builder.FilterFactories.Add(CreateFactory(displayName, logLevel, loggerType));
        });
        return ecb;
    }

    private static Func<EndpointFilterFactoryContext, EndpointFilterDelegate, EndpointFilterDelegate> CreateFactory(
        string? displayName,
        LogLevel logLevel,
        Type? loggerType)
    {
        return (routeHandlerContext, next) =>
        {
            var loggerFactory = routeHandlerContext.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerType is not null
                ? loggerFactory.CreateLogger(loggerType)
                : loggerFactory.CreateLogger<ExceptionFilter>();

            var filter = new ExceptionFilter(displayName ?? "Unknown Endpoint", logLevel, logger, next);
            return filter.Handle;
        };
    }
}
