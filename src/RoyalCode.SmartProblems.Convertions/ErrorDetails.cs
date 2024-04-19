namespace RoyalCode.SmartProblems.Convertions;

/// <summary>
/// A class that represents the details of an error.
/// </summary>
public class ErrorDetails : DetailsBase
{
    /// <summary>
    /// Creates a new instance of <see cref="ErrorDetails"/> class.
    /// </summary>
    /// <param name="problem">The problem to be converted.</param>
    public static implicit operator ErrorDetails(Problem problem)
    {
        var error = new ErrorDetails(problem.Detail)
        {
            Extensions = problem.Extensions
        };

        if (!string.IsNullOrEmpty(problem.Property))
            error.WithProperty(problem.Property, true);

        return error;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorDetails"/> class.
    /// </summary>
    /// <param name="detail">The detail of the error.</param>
    public ErrorDetails(string detail)
    {
        Detail = detail;
    }

    /// <summary>
    /// Describes the issue in detail.
    /// </summary>
    public string Detail { get; set; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ErrorDetails details &&
               Detail == details.Detail &&
               EqualityComparer<IDictionary<string, object>?>.Default.Equals(Extensions, details.Extensions);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Detail, Extensions);
    }
}