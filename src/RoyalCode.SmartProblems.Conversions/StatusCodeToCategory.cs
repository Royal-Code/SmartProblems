using System.Net;

namespace RoyalCode.SmartProblems.Conversions;

/// <summary>
/// Extension class to convert a status code to a category.
/// </summary>
public static class StatusCodeToCategory
{
    /// <summary>
    /// Convert a <see cref="HttpStatusCode"/> to a <see cref="ProblemCategory"/>.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <returns>A <see cref="ProblemCategory"/>.</returns>
    public static ProblemCategory ToCategory(this HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => ProblemCategory.InvalidParameter,
            HttpStatusCode.Forbidden => ProblemCategory.NotAllowed,
            HttpStatusCode.NotFound => ProblemCategory.NotFound,
            HttpStatusCode.Conflict => ProblemCategory.InvalidState,
            HttpStatusCode.UnprocessableEntity => ProblemCategory.ValidationFailed,
            >= HttpStatusCode.InternalServerError => ProblemCategory.InternalServerError,
            _ => ProblemCategory.CustomProblem
        };
    }
}