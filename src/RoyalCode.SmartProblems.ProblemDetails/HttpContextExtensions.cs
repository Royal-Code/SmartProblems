using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoyalCode.SmartProblems.Conversions;
using ProblemDetailsOptions = RoyalCode.SmartProblems.Descriptions.ProblemDetailsOptions;

namespace RoyalCode.SmartProblems;

/// <summary>
/// Extensions for <see cref="HttpContext"/> to work with <see cref="Problem"/> and <see cref="Problems"/>.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Convert the <paramref name="problems"/> to <see cref="ProblemDetails"/> using the
    /// <see cref="ProblemDetailsOptions"/> registered in the <see cref="IServiceCollection"/>
    /// getting it from the <see cref="HttpContext.RequestServices"/>.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/>.</param>
    /// <param name="problems">The result to be converted.</param>
    /// <returns>A new instance of <see cref="ProblemDetails"/>.</returns>
    public static ProblemDetails ToProblemDetails(this HttpContext context, Problems problems)
    {
        var options = context.RequestServices.GetRequiredService<IOptions<ProblemDetailsOptions>>().Value;
        return ProblemDetailsConverter.ToProblemDetails(problems, options);
    }
}
