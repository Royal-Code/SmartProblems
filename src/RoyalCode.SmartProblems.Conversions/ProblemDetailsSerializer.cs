﻿using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using RoyalCode.SmartProblems.Conversions.Internals;

namespace RoyalCode.SmartProblems.Conversions;

/// <summary>
/// A <see cref="JsonSerializerContext"/> for the <see cref="ProblemDetailsExtended"/>.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(DetailsBase))]
[JsonSerializable(typeof(CustomDetails))]
[JsonSerializable(typeof(ErrorDetails))]
[JsonSerializable(typeof(ProblemDetails))]
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