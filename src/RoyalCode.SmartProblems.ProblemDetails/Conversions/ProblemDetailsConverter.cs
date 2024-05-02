using Microsoft.AspNetCore.Mvc;
using RoyalCode.SmartProblems.Descriptions;

namespace RoyalCode.SmartProblems.Conversions;

/// <summary>
/// Converts the <see cref="Problems"/> to <see cref="ProblemDetails"/>.
/// </summary>
public static class ProblemDetailsConverter
{
    /// <summary>
    /// Convert the <paramref name="problems"/> to <see cref="ProblemDetails"/>.
    /// </summary>
    /// <param name="problems">The result to be converted.</param>
    /// <param name="options">The options to be used in the conversion.</param>
    /// <returns>A new instance of <see cref="ProblemDetails"/>.</returns>
    public static ProblemDetails ToProblemDetails(
       this Problems problems, ProblemDetailsOptions options)
    {
        if (problems.Count == 1)
        {
            var message = problems[0];
            return message.ToProblemDetails(options);
        }

        var builder = new ProblemDetailsBuilder();
        foreach (var message in problems)
        {
            AddProblem(message, builder);
        }

        return builder.Build(options);
    }

    /// <summary>
    /// Convert one message to a problem details.
    /// </summary>
    /// <param name="problem">The result message</param>
    /// <param name="options">The options for the conversion.</param>
    /// <returns>A new instance of <see cref="ProblemDetails"/>.</returns>
    public static ProblemDetails ToProblemDetails(
       this Problem problem, ProblemDetailsOptions options)
    {
        IDictionary<string, object?>? extensions;
        if (problem.Property is not null)
        {
            extensions = new Dictionary<string, object?>(StringComparer.Ordinal);
            if (problem.Extensions is not null)
                foreach (var (key, value) in problem.Extensions)
                    extensions.Add(key, value);

            extensions.Add("property", problem.Property);
        }
        else
        {
            extensions = problem.Extensions;
        }

        return ProblemDetailsBuilder.CreateProblemDetails(options, 
            problem.TypeId, problem.Category, problem.Detail, extensions);
    }

    private static void AddProblem(Problem problem, ProblemDetailsBuilder builder)
    {
        if (problem.TypeId is not null)
        {
            builder.AddCustomProblem(problem);
            return;
        }

        switch (problem.Category)
        {
            case ProblemCategory.NotFound:
                builder.AddNotFound(problem);
                break;
            case ProblemCategory.InvalidParameter:
                builder.AddInvalidParameter(problem);
                break;
            case ProblemCategory.ValidationFailed:
                builder.AddValidationFailed(problem);
                break;
            case ProblemCategory.InvalidState:
                builder.AddError(problem, true);
                break;
            case ProblemCategory.NotAllowed:
                builder.AddNotAllowed(problem);
                break;
            case ProblemCategory.InternalServerError:
                builder.AddInternalError(problem);
                break;
            case ProblemCategory.CustomProblem:
                builder.AddCustomProblem(problem);
                break;
            default:
                throw new InvalidOperationException("Invalid category");
        }
    }
}
