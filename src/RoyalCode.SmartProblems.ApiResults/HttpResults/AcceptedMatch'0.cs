using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using RoyalCode.SmartProblems.Metadata;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems.HttpResults;

/// <summary>
/// <para>
///     A <see cref="IResult"/> for <see cref="SmartProblems.Result"/> match for the success or error case.
/// </para>
/// <para>
///     When success, returns an <see cref="Accepted"/> (202) with an optional <c>Location</c> header
///     and no body: the request was accepted for processing, but the processing has not been completed.
///     When error, returns a <see cref="MatchErrorResult"/>.
/// </para>
/// </summary>
public sealed class AcceptedMatch : IResult, INestedHttpResult, IEndpointMetadataProvider
{
    /// <summary>
    /// Creates a new <see cref="AcceptedMatch"/> for the <see cref="SmartProblems.Result"/>, without a location.
    /// </summary>
    /// <param name="result">The <see cref="SmartProblems.Result"/> to be converted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch(Result result) => new(result);

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch"/> for the <see cref="Accepted"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch(Accepted result) => new(result);

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch"/> for the <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch(MatchErrorResult result) => new(result);

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch"/> for the <see cref="Problem"/>.
    /// </summary>
    /// <param name="problem"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch(Problem problem) => new(new MatchErrorResult(problem));

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch"/> for the <see cref="Problems"/>.
    /// </summary>
    /// <param name="problems"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch(Problems problems) => new(new MatchErrorResult(problems));

    /// <summary>
    /// Creates a new <see cref="IResult"/> for the <see cref="SmartProblems.Result"/>.
    /// </summary>
    /// <param name="result">The <see cref="SmartProblems.Result"/> to be converted.</param>
    /// <param name="location">Optional location, for the <c>Location</c> header, of a resource to monitor the accepted request.</param>
    public AcceptedMatch(Result result, string? location = null)
    {
        Result = result.Match<IResult, string?>(
            location,
            static uri => TypedResults.Accepted(uri),
            static (error, uri) => new MatchErrorResult(error));
    }

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch"/> for the <see cref="Accepted"/> match.
    /// </summary>
    /// <param name="result">The <see cref="Accepted"/> to be converted.</param>
    public AcceptedMatch(Accepted result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch"/> for the <see cref="MatchErrorResult"/> match.
    /// </summary>
    /// <param name="result">The <see cref="MatchErrorResult"/> to be converted.</param>
    public AcceptedMatch(MatchErrorResult result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch"/> for the <see cref="SmartProblems.Result"/> match.
    /// </summary>
    /// <param name="result"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AcceptedMatch(IResult result)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <inheritdoc/>
    public IResult Result { get; }

    /// <inheritdoc/>
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        // 202 without a body: content types explicitly empty so no JSON body is announced.
        builder.Metadata.Add(new ResponseTypeMetadata(StatusCodes.Status202Accepted, []));
        MatchErrorResult.PopulateMetadata(method, builder);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);
}
