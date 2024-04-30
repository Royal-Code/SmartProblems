
using System.Text.Json.Serialization;

namespace RoyalCode.SmartProblems.Convertions;

/// <summary>
/// <para>
///     The details of a not found problem.
/// </para>
/// </summary>
public class NotFoundDetails : DetailsBase
{
    /// <summary>
    /// Creates a new instance of <see cref="NotFoundDetails"/> class.
    /// </summary>
    /// <param name="problem">The problem to be converted.</param>
    public static implicit operator NotFoundDetails(Problem problem)
    {
        var notFound = new NotFoundDetails(problem.Detail)
        {
            Property = problem.Property,
            Extensions = problem.Extensions
        };

        return notFound;
    }

    /// <summary>
    /// Creates a new instance of <see cref="NotFoundDetails"/> with the specified message.
    /// </summary>
    /// <param name="message">The message of the problem.</param>
    [JsonConstructor]
    public NotFoundDetails(string message)
    {
        Message = message;
    }

    /// <summary>
    /// A message describing the problem of what is not found.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The property or parameter name related to the problem.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Property { get; set; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is NotFoundDetails details &&
               Message == details.Message &&
               Property == details.Property &&
               EqualityComparer<IDictionary<string, object?>?>.Default.Equals(Extensions, details.Extensions);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Message, Property, Extensions);
    }
}
