using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using RoyalCode.SmartProblems.Metadata;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems.HttpResults;

/// <summary>
/// <para>
///     A <see cref="IResult"/> for <see cref="SmartProblems.Result"/> match for the success or error case.
/// </para>
/// <para>
///     When success, returns an <see cref="Accepted{TValue}"/> (202) with the value as the body and an
///     optional <c>Location</c> header: the request was accepted for processing, but the processing has
///     not been completed. When error, returns a <see cref="MatchErrorResult"/>.
/// </para>
/// </summary>
/// <typeparam name="T">The returned value type when success.</typeparam>
public sealed class AcceptedMatch<T> : IResult, INestedHttpResult, IEndpointMetadataProvider
{
    /// <summary>
    /// Creates a new <see cref="AcceptedMatch{T}"/> for the <see cref="Result{TValue}"/>, without a location.
    /// </summary>
    /// <param name="result">The <see cref="Result{TValue}"/> to be converted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch<T>(Result<T> result) => new(result);

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch{T}"/> for the <see cref="Accepted{TValue}"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch<T>(Accepted<T> result) => new(result);

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch{T}"/> for the <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch<T>(MatchErrorResult result) => new(result);

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch{T}"/> for the <see cref="Problem"/>.
    /// </summary>
    /// <param name="problem"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch<T>(Problem problem) => new(new MatchErrorResult(problem));

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch{T}"/> for the <see cref="Problems"/>.
    /// </summary>
    /// <param name="problems"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch<T>(Problems problems) => new(new MatchErrorResult(problems));

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch"/> from an <see cref="AcceptedMatch{T}"/>.
    /// </summary>
    /// <param name="match"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AcceptedMatch(AcceptedMatch<T> match) => new(match.Result);

    /// <summary>
    /// Creates a new <see cref="IResult"/> for the <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="result">The <see cref="Result{TValue}"/> to be converted.</param>
    /// <param name="location">Optional location, for the <c>Location</c> header, of a resource to monitor the accepted request.</param>
    public AcceptedMatch(Result<T> result, string? location = null)
    {
        Result = result.Match<IResult, string?>(
            location,
            static (value, uri) => TypedResults.Accepted(uri, value),
            static (error, uri) => new MatchErrorResult(error));
    }

    /// <summary>
    /// Creates a new <see cref="IResult"/> for the <see cref="Result{TValue}"/>, with the location
    /// created from the success value.
    /// </summary>
    /// <param name="result">The <see cref="Result{TValue}"/> to be converted.</param>
    /// <param name="locationFunction">The function to create the location from the success value.</param>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="locationFunction"/> is null.
    /// </exception>
    public AcceptedMatch(Result<T> result, Func<T, string?> locationFunction)
    {
        ArgumentNullException.ThrowIfNull(locationFunction);

        Result = result.Match<IResult, Func<T, string?>>(
            locationFunction,
            static (value, f) => TypedResults.Accepted(f(value), value),
            static (error, f) => new MatchErrorResult(error));
    }

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch{T}"/> for the <see cref="Accepted{TValue}"/> match.
    /// </summary>
    /// <param name="result">The <see cref="Accepted{TValue}"/> to be converted.</param>
    public AcceptedMatch(Accepted<T> result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch{T}"/> for the <see cref="MatchErrorResult"/> match.
    /// </summary>
    /// <param name="result">The <see cref="MatchErrorResult"/> to be converted.</param>
    public AcceptedMatch(MatchErrorResult result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a new <see cref="AcceptedMatch{T}"/> for the <see cref="Result{TValue}"/> match.
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
        builder.Metadata.Add(new ResponseTypeMetadata(typeof(T), StatusCodes.Status202Accepted, MediaTypeNames.Application.Json));
        MatchErrorResult.PopulateMetadata(method, builder);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);
}
