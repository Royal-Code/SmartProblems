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
///     When success, returns a <see cref="NoContent"/>.
///     When error, returns a <see cref="MatchErrorResult"/>.
/// </para>
/// </summary>
public sealed class NoContentMatch : IResult, INestedHttpResult, IEndpointMetadataProvider
{
    /// <summary>
    /// Creates a new <see cref="NoContentMatch"/> for the <see cref="SmartProblems.Result"/>.
    /// </summary>
    /// <param name="result">The <see cref="OperationResult"/> to be converted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NoContentMatch(Result result) => new(result);

    /// <summary>
    /// Creates a new <see cref="NoContentMatch"/> for the <see cref="NoContent"/>.
    /// </summary>
    /// <param name="result">The <see cref="NoContent"/> to be converted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NoContentMatch(NoContent result) => new(result);

    /// <summary>
    /// Creates a new <see cref="NoContentMatch"/> for the <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NoContentMatch(MatchErrorResult result) => new(result);

    /// <summary>
    /// Creates a new <see cref="NoContentMatch"/> for the <see cref="Problem"/>.
    /// </summary>
    /// <param name="problem"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NoContentMatch(Problem problem) => new(new MatchErrorResult(problem));

    /// <summary>
    /// Creates a new <see cref="NoContentMatch"/> for the <see cref="Problems"/>.
    /// </summary>
    /// <param name="problems"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NoContentMatch(Problems problems) => new(new MatchErrorResult(problems));

    /// <summary>
    /// Creates a new <see cref="IResult"/> for the <see cref="SmartProblems.Result"/>.
    /// </summary>
    /// <param name="result">The <see cref="SmartProblems.Result"/> to be converted.</param>
    public NoContentMatch(Result result)
    {
        Result = result.Match<IResult>(
            TypedResults.NoContent,
            static error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="NoContent"/> match.
    /// </summary>
    /// <param name="result"></param>
    public NoContentMatch(NoContent result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="MatchErrorResult"/> match.
    /// </summary>
    /// <param name="result"></param>
    public NoContentMatch(MatchErrorResult result)
    {
        Result = result;
    }

    /// <inheritdoc/>
    public IResult Result { get; }

    /// <inheritdoc/>
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ResponseTypeMetadata(StatusCodes.Status204NoContent));
        MatchErrorResult.PopulateMetadata(method, builder);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);
}