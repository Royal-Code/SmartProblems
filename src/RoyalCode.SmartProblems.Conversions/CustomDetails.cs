using RoyalCode.SmartProblems.Conversions.Internals;

namespace RoyalCode.SmartProblems.Conversions;

/// <summary>
/// Custom problem details.
/// </summary>
public class CustomDetails : DetailsBase
{
    /// <summary>
    /// Creates a new instance of <see cref="CustomDetails"/> class.
    /// </summary>
    /// <param name="problem">The problem to be converted.</param>
    public static implicit operator CustomDetails(Problem problem)
    {
        var error = new CustomDetails
        {
            TypeId = problem.TypeId ?? "about:blank",
            Detail = problem.Detail,
            Extensions = problem.Extensions
        };

        if (!string.IsNullOrEmpty(problem.Property))
            error.WithProperty(problem.Property);

        return error;
    }

    /// <summary>
    /// The type id used to identify the problem and generate the <see cref="ProblemDetails.Type"/>.
    /// </summary>
    public required string TypeId { get; init; }

    /// <summary>
    /// The problem details.
    /// </summary>
    public required string Detail { get; init; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CustomDetails details &&
               TypeId == details.TypeId &&
               Detail == details.Detail &&
               EqualityComparer<IDictionary<string, object?>?>.Default.Equals(Extensions, details.Extensions);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(TypeId, Detail, Extensions);
    }
}
