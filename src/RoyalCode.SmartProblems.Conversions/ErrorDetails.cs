﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoyalCode.SmartProblems.Conversions;

#if NET7_0
#pragma warning disable SYSLIB1037 // Deserialization of init-only properties is currently not supported in source generation mode.
#endif

/// <summary>
/// A class that represents the details of an error.
/// </summary>
public sealed class ErrorDetails : DetailsBase
{
    /// <summary>
    /// Creates a new instance of <see cref="ErrorDetails"/> class.
    /// </summary>
    /// <param name="problem">The problem to be converted.</param>
    public static implicit operator ErrorDetails(Problem problem)
    {
        var error = new ErrorDetails(problem.Detail, problem.Property.PropertyToPointer())
        {
            Extensions = problem.Extensions,
            Category = problem.Category
        };

        return error;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorDetails"/> class.
    /// </summary>
    /// <param name="detail">The detail of the error.</param>
    /// <param name="pointer">The path to the property that caused the error, using JSON Pointer notation.</param>
    [JsonConstructor]
    public ErrorDetails(string detail, string? pointer = null)
        : base(detail, pointer)
    { }

    /// <summary>
    /// The category of the problem.
    /// <br/>
    /// This is used to determine the status code for the error 
    /// when the result have multiple errors with different categories.
    /// </summary>
    [JsonIgnore]

    public ProblemCategory Category { get; init; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ErrorDetails details &&
               Detail == details.Detail &&
               Pointer == details.Pointer &&
               Category == details.Category &&
               EqualityComparer<IDictionary<string, object?>?>.Default.Equals(Extensions, details.Extensions);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Detail, Pointer, Category, Extensions);
    }
    
    /// <summary>
    /// Converts the details to a JSON string, using the default serializer options.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, ProblemDetailsSerializer.DefaultErrorDetails);
    }
}