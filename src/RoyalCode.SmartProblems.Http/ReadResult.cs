using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.SmartProblems.Http;

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
    public static implicit operator bool(ReadResult result) => result.HasBeenRead;
    
    /// <summary>
    /// Convert the result to <see cref="Problems"/>.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static implicit operator Problems?(ReadResult result) => result.Problems;
    
    /// <summary>
    /// Determines if the result was readed and the problems was set.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Problems))]
    public bool HasBeenRead { get; init; }
    
    /// <summary>
    /// When the result has been read, it contains the problems that have been read from the answer.
    /// </summary>
    public Problems? Problems { get; init; }
}