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
///     When success, returns a <see cref="Created"/>.
///     When error, returns a <see cref="MatchErrorResult"/>.
/// </para>
/// </summary>
public sealed class CreatedMatch : IResult, INestedHttpResult, IEndpointMetadataProvider
{
    /// <summary>
    /// Creates a new <see cref="CreatedMatch"/> for the <see cref="Created"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CreatedMatch(Created result) => new(result);

    /// <summary>
    /// Creates a new <see cref="CreatedMatch{T}"/> for the <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CreatedMatch(MatchErrorResult result) => new(result);

    /// <summary>
    /// Creates a new <see cref="CreatedMatch"/> for the <see cref="Problem"/>.
    /// </summary>
    /// <param name="problem"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CreatedMatch(Problem problem) => new(new MatchErrorResult(problem));

    /// <summary>
    /// Creates a new <see cref="CreatedMatch"/> for the <see cref="Problems"/>.
    /// </summary>
    /// <param name="problems"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CreatedMatch(Problems problems) => new(new MatchErrorResult(problems));

    /// <summary>
    /// Creates a new <see cref="IResult"/> for the <see cref="SmartProblems.Result"/>.
    /// </summary>
    /// <param name="result">The <see cref="SmartProblems.Result"/> to be converted.</param>
    /// <param name="createdPath">The location of the created resource.</param>
    public CreatedMatch(Result result, string createdPath)
    {
        Result = result.Match<IResult, string>(
            createdPath,
            TypedResults.Created,
            static (error, uri) => new MatchErrorResult(error));
    }

    /// <summary>
    /// Creates a new <see cref="CreatedMatch"/> for the <see cref="Created"/> match.
    /// </summary>
    /// <param name="result">The <see cref="Created"/> to be converted.</param>
    public CreatedMatch(Created result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a new <see cref="CreatedMatch"/> for the <see cref="MatchErrorResult"/> match.
    /// </summary>
    /// <param name="result">The <see cref="MatchErrorResult"/> to be converted.</param>
    public CreatedMatch(MatchErrorResult result)
    {
        Result = result;
    }

    /// <summary>
    /// Creates a new <see cref="CreatedMatch"/> for the <see cref="SmartProblems.Result"/> match.
    /// </summary>
    /// <param name="result"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CreatedMatch(IResult result)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <inheritdoc/>
    public IResult Result { get; }

    /// <inheritdoc/>
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ResponseTypeMetadata(StatusCodes.Status201Created));
        MatchErrorResult.PopulateMetadata(method, builder);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);
}