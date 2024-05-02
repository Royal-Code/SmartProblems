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
    /// Additional information about the details of the error.
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object?>? Extensions { get; set; }

    /// <summary>
    /// Converts the details to a JSON string, using the default serializer options.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, DetailsBaseExtensions.SerializerOptions);
    }
}
