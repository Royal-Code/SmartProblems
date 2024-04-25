using RoyalCode.SmartProblems.Convertions.Internals;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace RoyalCode.SmartProblems.Convertions;

/// <summary>
/// A <see cref="JsonSerializerContext"/> for the <see cref="ProblemDetailsExtended"/>.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(CustomDetails))]
[JsonSerializable(typeof(ErrorDetails))]
[JsonSerializable(typeof(InvalidParameterDetails))]
[JsonSerializable(typeof(NotFoundDetails))]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(ProblemDetailsExtended))]
public partial class ProblemDetailsSerializer : JsonSerializerContext
{
    /// <summary>
    /// The default <see cref="JsonTypeInfo{T}"/> for <see cref="ProblemDetailsExtended"/>.
    /// </summary>
    public static JsonTypeInfo<ProblemDetailsExtended> DefaultProblemDetailsExtended 
        => Default.ProblemDetailsExtended;
}