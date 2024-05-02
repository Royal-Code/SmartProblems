using System.Text.Json.Serialization;

namespace RoyalCode.SmartProblems.Conversions;

/// <summary>
/// A class that represents the details of an invalid parameter.
/// </summary>
public class InvalidParameterDetails : DetailsBase
{
    /// <summary>
    /// Creates a new instance of <see cref="InvalidParameterDetails"/> class.
    /// </summary>
    /// <param name="problem">The problem to be converted.</param>
    public static implicit operator InvalidParameterDetails(Problem problem)
    {
        var invalidParameter = new InvalidParameterDetails(problem.Property, problem.Detail)
        {
            Extensions = problem.Extensions
        };

        return invalidParameter;
    }

    /// <summary>
    /// Creates a new instance of <see cref="InvalidParameterDetails"/> class.
    /// </summary>
    /// <param name="reason">The reason for the invalid parameter.</param>
    [JsonConstructor]
    public InvalidParameterDetails(string reason)
    {
        Reason = reason;
    }

    /// <summary>
    /// Creates a new instance of <see cref="InvalidParameterDetails"/> class.
    /// </summary>
    /// <param name="name">The name of the parameter or property.</param>
    /// <param name="reason">The reason for the invalid parameter.</param>
    public InvalidParameterDetails(string? name, string reason)
    {
        Name = name;
        Reason = reason;
    }

    /// <summary>
    /// The name of the parameter or property.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    /// <summary>
    /// A message that describes the reason for the invalid parameter.
    /// </summary>
    public string Reason { get; set; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is InvalidParameterDetails details &&
               Name == details.Name &&
               Reason == details.Reason &&
               EqualityComparer<IDictionary<string, object?>?>.Default.Equals(Extensions, details.Extensions);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Reason, Extensions);
    }
}
