using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoyalCode.SmartProblems.Interceptors;

/// <summary>
/// A interceptor for the <see cref="ProblemDetails"/>.
/// </summary>
public interface IMatchProblemDetailsInterceptor
{
    /// <summary>
    /// Interceptor for the <see cref="ProblemDetails"/> when is being writed as a result.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"/> that contains the request information.</param>
    /// <param name="problemDetails">The <see cref="ProblemDetails"/> to be writed.</param>
    /// <param name="problems">The <see cref="Problems"/> that contains the errors.</param>
    void WritingProblemDetails(HttpRequest request, ProblemDetails problemDetails, Problems problems);
}