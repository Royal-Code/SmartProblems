namespace RoyalCode.SmartProblems;

/// <summary>
/// Extensions for async operations with <see cref="Result"/> and <see cref="Result{TValue}"/>.
/// </summary>
public static class AsyncResultExtensions
{
    #region Result

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The new type of the result.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The new result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TResult>(this Task<Result> task,
        Func<TResult> onSuccess, 
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return result.Match(onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The new type of the result.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the functions.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">A parameter to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The new result of the executed function.
    /// </returns>
    public static async Task<TResult> Match<TResult, TParam>(this Task<Result> task,
        TParam param,
        Func<TParam, TResult> onSuccess,
        Func<Problems, TParam, TResult> onFailure)
    {
        var result = await task;
        return result.Match(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The new type of the result.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the functions.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The new result of the executed function.
    /// </returns>
    public static async Task<TResult> Match<TResult, TParam>(this Task<Result> task,
        TParam param,
        Func<TParam, TResult> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return result.Match(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Map a value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue>(this Task<Result> task, TValue value)
    {
        var result = await task;
        return result.Map(value);
    }

    /// <summary>
    /// <para>
    ///     Map a value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="valueFactory">A function to generate the value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue>(this Task<Result> task, Func<TValue> valueFactory)
    {
        var result = await task;
        return result.Map(valueFactory);
    }

    /// <summary>
    /// <para>
    ///     Map a value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the function.</param>
    /// <param name="valueFactory">A function to generate the value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue, TParam>(this Task<Result> task, 
        TParam param,
        Func<TParam, TValue> valueFactory)
    {
        var result = await task;
        return result.Map(param, valueFactory);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="map">A function to generate the new result.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue>(this Task<Result> task, Func<Result<TValue>> map)
    {
        var result = await task;
        return result.Map(map);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the function.</param>  
    /// <param name="map">A function to generate the new result.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue, TParam>(this Task<Result> task,
        TParam param, 
        Func<TParam, Result<TValue>> map)
    {
        var result = await task;
        return result.Map(param, map);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result> OnSuccessAsync(this Task<Result> task, Action action)
    {
        var result = await task;
        return result.OnSuccess(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result> OnSuccessAsync<TParam>(this Task<Result> task, TParam param, Action<TParam> action)
    {
        var result = await task;
        return result.OnSuccess(param, action);
    }
    
    #endregion

    #region Result<TValue>

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public static async Task<Result<TOther>> MapAsync<TValue, TOther>(this Task<Result<TValue>> task, Func<TValue, TOther> map)
    {
        var result = await task;
        return result.Map(map);
    }
    
    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public static async Task<Result<TOther>> MapAsync<TValue, TOther>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, TOther> map)
    {
        var result = await task;
        return result.Map(map);
    }

    #endregion
}