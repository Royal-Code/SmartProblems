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
///     When success, returns a <see cref="Ok{TValue}"/>.
///     When error, returns a <see cref="MatchErrorResult"/>.
/// </para>
/// </summary>
/// <typeparam name="T">The returned value type when success.</typeparam>
public sealed class OkMatch<T> : IResult, INestedHttpResult, IEndpointMetadataProvider
{
    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OkMatch<T>(Result<T> result) => new(result);

    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="Ok{TValue}"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OkMatch<T>(Ok<T> result) => new(result);

    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="Ok{TValue}"/>.
    /// </summary>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OkMatch<T>(T value) => new(TypedResults.Ok(value));

    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OkMatch<T>(MatchErrorResult result) => new(result);

    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="Problem"/>.
    /// </summary>
    /// <param name="problem"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OkMatch<T>(Problem problem) => new(new MatchErrorResult(problem));

    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="Problems"/>.
    /// </summary>
    /// <param name="problems"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OkMatch<T>(Problems problems) => new(new MatchErrorResult(problems));

    /// <summary>
    /// Creates a new <see cref="IResult"/> for the <see cref="SmartProblems.Result"/> match.
    /// </summary>
    /// <param name="result">The <see cref="SmartProblems.Result"/> to be converted.</param>
    public OkMatch(Result<T> result)
    {
        Result = result.Match<IResult>(
            TypedResults.Ok,
            static error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="Ok{TValue}"/> match.
    /// </summary>
    /// <param name="result"></param>
    public OkMatch(Ok<T> result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="MatchErrorResult"/> match.
    /// </summary>
    /// <param name="result"></param>
    public OkMatch(MatchErrorResult result)
    {
        Result = result;
    }

    /// <inheritdoc/>
    public IResult Result { get; }

    /// <inheritdoc/>
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ResponseTypeMetadata(typeof(T), StatusCodes.Status200OK, MediaTypeNames.Application.Json));
        MatchErrorResult.PopulateMetadata(method, builder);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);
}