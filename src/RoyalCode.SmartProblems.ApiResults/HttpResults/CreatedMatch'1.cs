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
///     When success, returns a <see cref="Created"/>.
///     When error, returns a <see cref="MatchErrorResult"/>.
/// </para>
/// </summary>
/// <typeparam name="T">The returned value type when success.</typeparam>
public sealed class CreatedMatch<T> : IResult, INestedHttpResult, IEndpointMetadataProvider
{
    /// <summary>
    /// Creates a new <see cref="CreatedMatch{T}"/> for the <see cref="Created{TValue}"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CreatedMatch<T>(Created<T> result) => new(result);

    /// <summary>
    /// Creates a new <see cref="CreatedMatch{T}"/> for the <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CreatedMatch<T>(MatchErrorResult result) => new(result);

    /// <summary>
    /// Creates a new <see cref="CreatedMatch{T}"/> for the <see cref="Problem"/>.
    /// </summary>
    /// <param name="problem"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CreatedMatch<T>(Problem problem) => new(new MatchErrorResult(problem));

    /// <summary>
    /// Creates a new <see cref="CreatedMatch{T}"/> for the <see cref="Problems"/>.
    /// </summary>
    /// <param name="problems"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CreatedMatch<T>(Problems problems) => new(new MatchErrorResult(problems));

    /// <summary>
    /// Creates a new <see cref="CreatedMatch"/> from a <see cref="CreatedMatch{T}"/>.
    /// </summary>
    /// <param name="match"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CreatedMatch(CreatedMatch<T> match) => new(match.Result);

    /// <summary>
    /// Creates a new <see cref="IResult"/> for the <see cref="SmartProblems.Result"/>.
    /// </summary>
    /// <param name="result">The <see cref="OperationResult"/> to be converted.</param>
    /// <param name="createdPath">The location of the created resource.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    public CreatedMatch(Result<T> result, string createdPath, bool formatPathWithValue = false)
    {
        Result = result.Match<IResult>(
            value => TypedResults.Created(formatPathWithValue ? string.Format(createdPath, value) : createdPath, value),
            error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Creates a new <see cref="IResult"/> for the <see cref="SmartProblems.Result"/>.
    /// </summary>
    /// <param name="result">The <see cref="SmartProblems.Result"/> to be converted.</param>
    /// <param name="createdPathFunction">The function to create the location of the created resource.</param>
    public CreatedMatch(Result<T> result, Func<T, string> createdPathFunction)
    {
        Result = result.Match<IResult>(
            value => TypedResults.Created(createdPathFunction(value), value),
            error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Creates a new <see cref="CreatedMatch{T}"/> for the <see cref="Created{TValue}"/> match.
    /// </summary>
    /// <param name="result">The <see cref="Created{TValue}"/> to be converted.</param>
    public CreatedMatch(Created<T> result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a new <see cref="CreatedMatch{T}"/> for the <see cref="MatchErrorResult"/> match.
    /// </summary>
    /// <param name="result">The <see cref="MatchErrorResult"/> to be converted.</param>
    public CreatedMatch(MatchErrorResult result)
    {
        Result = result;
    }

    /// <inheritdoc/>
    public IResult Result { get; }

    /// <inheritdoc/>
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ResponseTypeMetadata(typeof(T), StatusCodes.Status201Created, MediaTypeNames.Application.Json));
        MatchErrorResult.PopulateMetadata(method, builder);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);
}