using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using ProblemDetailsOptions = OutraLib.FluentProblems.Descriptions.ProblemDetailsOptions;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods to map the problem details description page endpoint.
/// </summary>
public static class ProblemDetailsDescriptionPageEndpointRouteBuilderExtensions
{
    /// <summary>
    /// <para>
    ///     Maps the problem details description page to the default route.
    /// </para>
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint convention builder.</returns>
    public static IEndpointConventionBuilder MapProblemDetailsDescriptionPage(this IEndpointRouteBuilder endpoints)
    {
        return MapProblemDetailsDescriptionPage(endpoints, "/.problems");
    }

    /// <summary>
    /// <para>
    ///     Maps the problem details description page to the specified route.
    /// </para>
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="pattern">The route pattern of the page.</param>
    /// <returns>The endpoint convention builder.</returns>
    public static IEndpointConventionBuilder MapProblemDetailsDescriptionPage(this IEndpointRouteBuilder endpoints, string pattern)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("The route pattern cannot be null or empty.", nameof(pattern));

        return endpoints
            .MapGet(pattern, (HttpContext context, IOptions<ProblemDetailsOptions> options) =>
            {
                string pageUri = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{context.Request.Path}";
                var model = OutraLib.FluentProblems.Documentation.ProblemDetailsDescriptionPageModel.Create(options.Value, pageUri);
                string html = OutraLib.FluentProblems.Documentation.ProblemDetailsDescriptionPageHtmlRenderer.Render(model);

                return TypedResults.Content(html, "text/html; charset=utf-8");
            })
            .WithName("ProblemDetailsDescriptionPage")
            .WithDisplayName("Problem Details Description Page")
            .ExcludeFromDescription()
            .Produces(StatusCodes.Status200OK, contentType: "text/html");
    }
}