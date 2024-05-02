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
        var error = new CustomDetails(
            problem.Detail,
            problem.TypeId ?? "about:blank",
            problem.Property.PropertyToPointer())
        {
            Extensions = problem.Extensions
        };

        return error;
    }

    /// <summary>
    /// Creates a new instance of <see cref="CustomDetails"/> class.
    /// </summary>
    /// <param name="detail">The detail of the error.</param>
    /// <param name="typeId">The type id used to identify the problem and generate the <see cref="ProblemDetails.Type"/>.</param>
    /// <param name="pointer">The path to the property that caused the error, using JSON Pointer notation.</param>
    public CustomDetails(string detail, string typeId, string? pointer = null) 
        : base(detail, pointer)
    { 
        TypeId = typeId;
    }

    /// <summary>
    /// The type id used to identify the problem and generate the <see cref="ProblemDetails.Type"/>.
    /// </summary>
    public string TypeId { get; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CustomDetails details &&
               TypeId == details.TypeId &&
               Detail == details.Detail &&
               Pointer == details.Pointer &&
               EqualityComparer<IDictionary<string, object?>?>.Default.Equals(Extensions, details.Extensions);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(TypeId, Detail, Pointer, Extensions);
    }
}
