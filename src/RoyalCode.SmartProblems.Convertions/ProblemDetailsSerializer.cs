using RoyalCode.SmartProblems.Convertions.Internals;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace RoyalCode.SmartProblems.Convertions;

/// <summary>
/// A <see cref="JsonSerializerContext"/> for the <see cref="ProblemDetailsExtended"/>.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(DetailsBase))]
[JsonSerializable(typeof(CustomDetails))]
[JsonSerializable(typeof(ErrorDetails))]
//[JsonSerializable(typeof(IEnumerable<ErrorDetails>))]
[JsonSerializable(typeof(InvalidParameterDetails))]
//[JsonSerializable(typeof(IEnumerable<InvalidParameterDetails>))]
[JsonSerializable(typeof(NotFoundDetails))]
//[JsonSerializable(typeof(IEnumerable<NotFoundDetails>))]
[JsonSerializable(typeof(ProblemDetails))]
//[JsonSerializable(typeof(IEnumerable<ProblemDetails>))]
//[JsonSerializable(typeof(Dictionary<string, object>))]
//[JsonSerializable(typeof(IDictionary<string, object>))]
//[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(ProblemDetailsExtended))]
public partial class ProblemDetailsSerializer : JsonSerializerContext
{
    /// <summary>
    /// The default <see cref="JsonTypeInfo{T}"/> for <see cref="ProblemDetailsExtended"/>.
    /// </summary>
    public static JsonTypeInfo<ProblemDetailsExtended> DefaultProblemDetailsExtended 
        => Default.ProblemDetailsExtended;
}