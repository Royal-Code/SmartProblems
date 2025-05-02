using RoyalCode.SmartProblems;
using RoyalCode.SmartProblems.MvcResults;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// Extensions for adapt <see cref="Result"/> to <see cref="ObjectResult"/>.
/// </summary>
public static class MvcResultsExtensions
{
    /// <summary>
    /// Convert a <see cref="Result"/> to a <see cref="MatchActionResult"/>
    /// to be used in a MVC controller as a return type.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to be converted.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <returns>The <see cref="MatchActionResult"/> for the response.</returns>
    public static MatchActionResult ToActionResult(this Result result,
        string? createdPath = null)
        => new(result, createdPath);

    /// <summary>
    /// Convert a <see cref="Result{TValue}"/> to an <see cref="MatchActionResult{TValue}"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The <see cref="Result{TValue}"/> to be converted.</param>
    /// <returns>The <see cref="MatchActionResult{TValue}"/> for the response.</returns>
    public static MatchActionResult<TValue> ToActionResult<TValue>(this Result<TValue> result)
        => new(result);

    /// <summary>
    /// Convert a <see cref="Result{TValue}"/> to an <see cref="MatchActionResult{TValue}"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The <see cref="Result{TValue}"/> to be converted.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="MatchActionResult{TValue}"/> for the response.</returns>
    public static MatchActionResult<TValue> ToActionResult<TValue>(this Result<TValue> result,
        string createdPath, bool formatPathWithValue = false)
        => new(result, createdPath, formatPathWithValue);

    /// <summary>
    /// Convert a <see cref="Result{TValue}"/> to an <see cref="MatchActionResult{TValue}"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The <see cref="Result{TValue}"/> to be converted.</param>
    /// <param name="createdPathProvider">A function that provides the path for created responses.</param>
    /// <returns>The <see cref="MatchActionResult{TValue}"/> for the response.</returns>
    public static MatchActionResult<TValue> ToActionResult<TValue>(this Result<TValue> result,
        Func<TValue, string> createdPathProvider)
        => new(result, createdPathProvider);
}