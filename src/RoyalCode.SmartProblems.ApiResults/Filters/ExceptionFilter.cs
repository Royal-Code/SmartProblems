using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RoyalCode.SmartProblems.HttpResults;

namespace RoyalCode.SmartProblems.Filters;

internal sealed class ExceptionFilter
{
    private readonly string displayName;
    private readonly LogLevel logLevel;
    private readonly ILogger logger;
    private readonly EndpointFilterDelegate next;

    public ExceptionFilter(string displayName, LogLevel logLevel, ILogger logger, EndpointFilterDelegate next)
    {
        this.displayName = displayName;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.logLevel = logLevel;
    }

    public async ValueTask<object?> Handle(EndpointFilterInvocationContext context)
    {
        try
        {
            var result = await next(context);

            if (logLevel != LogLevel.None
                && result is INestedHttpResult nested
                && nested.Result is MatchErrorResult matchError)
            {
                logger.Log(
                    logLevel,
                    "The endpoint result ({Endpoint}) is failure, problem(s): {Problems}",
                    displayName,
                    matchError);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An exception occurred during the execution of an API endpoint ({Endpoint})",
                displayName);

            MatchErrorResult result = Problems.InternalError(ex);
            return result;
        }
    }
}