using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.SmartProblems.Http;

/// <summary>
/// Result of the attempt to read the response content by <see cref="FailureTypeReader"/>.
/// </summary>
public readonly struct ReadResult
{
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