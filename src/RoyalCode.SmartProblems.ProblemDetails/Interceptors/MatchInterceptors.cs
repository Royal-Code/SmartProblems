using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems.Interceptors;

/// <summary>
/// Internal class to manage the interceptors for the <see cref="ResultErrors"/> and <see cref="ProblemDetails"/>.
/// </summary>
public static class MatchInterceptors
{
    private static HasInterceptors hasInterceptors;

    /// <summary>
    /// Invoked when the <see cref="ProblemDetails"/> is being writed.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="problemDetails">The <see cref="ProblemDetails"/> to be writed.</param>
    /// <param name="problems">The <see cref="Problems"/> that contains the errors.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WritingProblemDetails(HttpContext httpContext, ProblemDetails problemDetails, Problems problems)
    {
        IEnumerable<IMatchProblemDetailsInterceptor> interceptors;

        switch (hasInterceptors)
        {
            case HasInterceptors.Unknown:
                hasInterceptors = TryLoad(httpContext.RequestServices, out interceptors);
                break;
            case HasInterceptors.True:
                interceptors = httpContext.RequestServices.GetRequiredService<IEnumerable<IMatchProblemDetailsInterceptor>>();
                break;
            default:
                return;
        }

        foreach (var interceptor in interceptors)
            try
            {
                interceptor.WritingProblemDetails(problemDetails, problems);
            }
            catch (Exception ex)
            {
                var logger = httpContext.RequestServices.GetService<ILogger<IMatchProblemDetailsInterceptor>>()!;
                logger.LogError(ex, "Error executing the interceptor {Interceptor}",
                    interceptor.GetType().FullName);
            }
    }

    private static HasInterceptors TryLoad<T>(IServiceProvider requestServices, out IEnumerable<T> interceptors)
    {
        // get the interceptors from the request services
        interceptors = requestServices.GetService<IEnumerable<T>>() ?? Enumerable.Empty<T>();

        // check if has interceptors
        return interceptors.Any() ? HasInterceptors.True : HasInterceptors.False;
    }

    private enum HasInterceptors : byte
    {
        Unknown = 0,
        True = 1,
        False = 2
    }
}