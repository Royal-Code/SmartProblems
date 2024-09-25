using Microsoft.AspNetCore.Http.HttpResults;
using RoyalCode.SmartProblems;
using RoyalCode.SmartProblems.HttpResults;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extensions for adapt <see cref="Result"/> of SmartProblems to <see cref="IResult"/> of Minimal APIs.
/// </summary>
public static partial class ApiResults
{
    /// <summary>
    /// Convert the <see cref="Result{T}"/> to <see cref="NoContent"/> or <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static Results<NoContent, MatchErrorResult> ToResult(this IResultExtensions _, Result result)
    {
        return result.Match<Results<NoContent, MatchErrorResult>>(
            static () => TypedResults.NoContent(),
            static error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="Result{T}"/> to <see cref="Created"/> or <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static Results<Created, MatchErrorResult> ToResult(this IResultExtensions _,
        Result result, string createdPath)
    {
        return result.Match<Results<Created, MatchErrorResult>, string>(
            createdPath,
            static path => TypedResults.Created(path),
            static error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="Result{T}"/> to <see cref="Ok{TValue}"/> or <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <returns>The <see cref="IResult"/> implementation for the response.</returns>
    public static Results<Ok<T>, MatchErrorResult> ToResult<T>(
        this IResultExtensions _, Result<T> result)
    {
        return result.Match<Results<Ok<T>, MatchErrorResult>>(
            static value => TypedResults.Ok(value),
            static error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="Result{T}"/> to <see cref="Created{T}"/> or <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="IResult"/> implementation for the response.</returns>
    public static Results<Created<T>, MatchErrorResult> ToResult<T>(this IResultExtensions _,
        Result<T> result, string createdPath, bool formatPathWithValue = false)
    {
        return result.Match<Results<Created<T>, MatchErrorResult>, (string path, bool format)>(
            (createdPath, formatPathWithValue),
            static (value, tuple) => TypedResults.Created(tuple.format ? string.Format(tuple.path, value) : tuple.path, value),
            static error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="Result{T}"/> to <see cref="Created{T}"/> or <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPathFunction">A function to create the path for created responses.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static Results<Created<T>, MatchErrorResult> ToResult<T>(this IResultExtensions _,
        Result<T> result, Func<T, string> createdPathFunction)
    {
        return result.Match<Results<Created<T>, MatchErrorResult>, Func<T, string>>(
            createdPathFunction,
            static (value, func) => TypedResults.Created(func(value), value),
            static error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="Result{TValue}"/> to <see cref="RoyalCode.SmartProblems.HttpResults.CreatedMatch{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPathFunction">A function to create the path for created responses.</param>
    /// <returns>The <see cref="RoyalCode.SmartProblems.HttpResults.CreatedMatch{T}"/> for the response.</returns>
    public static CreatedMatch<T> CreatedMatch<T>(this Result<T> result, Func<T, string> createdPathFunction)
    {
        return new CreatedMatch<T>(result, createdPathFunction);
    }

    /// <summary>
    /// Convert the <see cref="Result{TValue}"/> to <see cref="RoyalCode.SmartProblems.HttpResults.CreatedMatch{T}"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TResponse">The value type for the response.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPathFunction">A function to create the path for created responses.</param>
    /// <param name="selector">A function to select the value for the response.</param>
    /// <returns>The <see cref="RoyalCode.SmartProblems.HttpResults.CreatedMatch{T}"/> for the response.</returns>
    public static CreatedMatch<TResponse> CreatedMatch<TValue, TResponse>(
        this Result<TValue> result,
        Func<TValue, string> createdPathFunction,
        Func<TValue, TResponse> selector)
    {
        return new CreatedMatch<TResponse>(
            result.Match<IResult>(
                value => TypedResults.Created(createdPathFunction(value), selector(value)),
                error => new MatchErrorResult(error)));
    }

    /// <summary>
    /// Convert the <see cref="Result{TValue}"/> to <see cref="RoyalCode.SmartProblems.HttpResults.CreatedMatch{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="RoyalCode.SmartProblems.HttpResults.CreatedMatch{T}"/> for the response.</returns>
    public static CreatedMatch<T> CreatedMatch<T>(this Result<T> result, string createdPath, bool formatPathWithValue = false)
    {
        return new CreatedMatch<T>(result, createdPath, formatPathWithValue);
    }

    /// <summary>
    /// Convert the <see cref="Result"/> to <see cref="RoyalCode.SmartProblems.HttpResults.CreatedMatch"/>.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <returns>The <see cref="RoyalCode.SmartProblems.HttpResults.CreatedMatch"/> for the response.</returns>
    public static CreatedMatch CreatedMatch(this Result result, string createdPath)
    {
        return new CreatedMatch(result, createdPath);
    }

    /// <summary>
    /// Convert the <see cref="Result"/> to <see cref="RoyalCode.SmartProblems.HttpResults.CreatedMatch"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <returns>The <see cref="RoyalCode.SmartProblems.HttpResults.CreatedMatch"/> for the response.</returns>
    public static OkMatch<T> OkMatch<T>(this Result<T> result)
    {
        return new OkMatch<T>(result);
    }
}