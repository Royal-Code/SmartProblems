using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoyalCode.SmartProblems.Conversions;

/// <summary>
/// <para>
///     A base class that represents the details of an error.
/// </para>
/// <para>
///     Used to convert a <see cref="Problem"/> to a specific details class, 
///     and afterwards, convert to a ProblemDetails.
/// </para>
/// </summary>
public abstract class DetailsBase
{
    /// <summary>
    /// Describes the issue in detail.
    /// </summary>
    public string Detail { get; }

    /// <summary>
    /// The path to the property that caused the error, using JSON Pointer notation.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Pointer { get; }

    /// <summary>
    /// Additional information about the details of the error.
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object?>? Extensions { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="DetailsBase"/> class.
    /// </summary>
    /// <param name="detail">The detail of the error.</param>
    /// <param name="pointer">Optional. The path to the property that caused the error, using JSON Pointer notation.</param>
    protected DetailsBase(string detail, string? pointer = null)
    {
        Detail = detail;
        Pointer = pointer;
    }

    /// <summary>
    /// Converts the details to a JSON string, using the default serializer options.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, DetailsBaseExtensions.SerializerOptions);
    }
}
