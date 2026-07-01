using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using RoyalCode.SmartProblems.Descriptions.Documentation;
using SmartProblemDetailsOptions = RoyalCode.SmartProblems.Descriptions.ProblemDetailsOptions;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Provides endpoint mapping extensions for the problem details description page.
/// </summary>
public static class ProblemDetailsDescriptionPageEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps the problem details description page to <c>/.problems</c>.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint convention builder.</returns>
    public static IEndpointConventionBuilder MapProblemDetailsDescriptionPage(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapProblemDetailsDescriptionPage("/.problems");
    }

    /// <summary>
    /// Maps the problem details description page to the specified route pattern.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="pattern">The route pattern used to serve the page.</param>
    /// <returns>The endpoint convention builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="endpoints"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="pattern"/> is null or empty.</exception>
    public static IEndpointConventionBuilder MapProblemDetailsDescriptionPage(
        this IEndpointRouteBuilder endpoints,
        string pattern)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("The route pattern cannot be null or empty.", nameof(pattern));

        return endpoints.MapGet(pattern, static (
                HttpContext context,
                IOptions<SmartProblemDetailsOptions> options) =>
            {
                var request = context.Request;
                var pageUri = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}";
                var model = ProblemDetailsDescriptionPageModel.Create(options.Value, pageUri);
                var html = ProblemDetailsDescriptionPageHtmlRenderer.Render(model);

                return TypedResults.Content(html, "text/html; charset=utf-8");
            })
            .WithName("ProblemDetailsDescriptionPage")
            .WithDisplayName("Problem Details Description Page")
            .ExcludeFromDescription()
            .Produces(StatusCodes.Status200OK, contentType: "text/html");
    }
}
