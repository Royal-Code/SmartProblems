using System.Diagnostics.CodeAnalysis;
using RoyalCode.SmartProblems;

namespace System.Net.Http;

/// <summary>
/// Result of the attempt to read the response content by <see cref="FailureTypeReader"/>.
/// </summary>
public readonly struct ReadResult
{
    /// <summary>
    /// Convert the result to a boolean value.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator bool(ReadResult result) => result.Readed;
    
    /// <summary>
    /// Convert the result to <see cref="Problems"/>.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static implicit operator Problems(ReadResult result) => result.Problems ?? throw new InvalidOperationException("The result is not readed.");
    
    /// <summary>
    /// Determines if the result was readed and the problems was set.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Problems))]
    public bool Readed { get; init; }
    
    /// <summary>
    /// When the result was readed, the problems readed from the response.
    /// </summary>
    public Problems? Problems { get; init; }
}