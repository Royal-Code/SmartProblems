
namespace RoyalCode.SmartProblems;

/// <summary>
/// Extensions for async operations with <see cref="Result"/> and <see cref="Result{TValue}"/>.
/// </summary>
public static class AsyncResultExtensions
{
    #region Result MatchAsync

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
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The new result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TResult>(this Task<Result> task,
        Func<Task<TResult>> onSuccess,
        Func<Problems, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(onSuccess, onFailure);
    }

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
        Func<Task<TResult>> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(onSuccess, onFailure);
    }

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
    public static async Task<TResult> MatchAsync<TResult>(this ValueTask<Result> task,
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
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The new result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TResult>(this ValueTask<Result> task,
        Func<Task<TResult>> onSuccess,
        Func<Problems, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(onSuccess, onFailure);
    }

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
    public static async Task<TResult> MatchAsync<TResult>(this ValueTask<Result> task,
        Func<Task<TResult>> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(onSuccess, onFailure);
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
    public static async Task<TResult> MatchAsync<TResult, TParam>(this Task<Result> task,
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
    /// <param name="param">A parameter to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The new result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TResult, TParam>(this Task<Result> task,
        TParam param,
        Func<TParam, Task<TResult>> onSuccess,
        Func<Problems, TParam, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
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
    public static async Task<TResult> MatchAsync<TResult, TParam>(this Task<Result> task,
        TParam param,
        Func<TParam, Task<TResult>> onSuccess,
        Func<Problems, TParam, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
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
    public static async Task<TResult> MatchAsync<TResult, TParam>(this ValueTask<Result> task,
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
    /// <param name="param">A parameter to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The new result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TResult, TParam>(this ValueTask<Result> task,
        TParam param,
        Func<TParam, Task<TResult>> onSuccess,
        Func<Problems, TParam, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
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
    public static async Task<TResult> MatchAsync<TResult, TParam>(this ValueTask<Result> task,
        TParam param,
        Func<TParam, Task<TResult>> onSuccess,
        Func<Problems, TParam, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
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
    public static async Task<TResult> MatchAsync<TResult, TParam>(this Task<Result> task,
        TParam param,
        Func<TParam, TResult> onSuccess,
        Func<Problems, TResult> onFailure)
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
    public static async Task<TResult> MatchAsync<TResult, TParam>(this Task<Result> task,
        TParam param,
        Func<TParam, Task<TResult>> onSuccess,
        Func<Problems, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
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
    public static async Task<TResult> MatchAsync<TResult, TParam>(this Task<Result> task,
        TParam param,
        Func<TParam, Task<TResult>> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
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
    public static async Task<TResult> MatchAsync<TResult, TParam>(this ValueTask<Result> task,
        TParam param,
        Func<TParam, TResult> onSuccess,
        Func<Problems, TResult> onFailure)
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
    public static async Task<TResult> MatchAsync<TResult, TParam>(this ValueTask<Result> task,
        TParam param,
        Func<TParam, Task<TResult>> onSuccess,
        Func<Problems, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
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
    public static async Task<TResult> MatchAsync<TResult, TParam>(this ValueTask<Result> task,
        TParam param,
        Func<TParam, Task<TResult>> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
    }

    #endregion

    #region Result<TValue> MatchAsync

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> task,
        Func<TValue, TResult> onSuccess,
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
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> task,
        Func<TValue, Task<TResult>> onSuccess,
        Func<Problems, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> task,
        Func<TValue, Task<TResult>> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, TResult> onSuccess,
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
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, Task<TResult>> onSuccess,
        Func<Problems, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, Task<TResult>> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the functions.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, TResult> onSuccess,
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
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the functions.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, TParam, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the functions.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, TParam, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the functions.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, TResult> onSuccess,
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
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the functions.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, TParam, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the functions.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, TParam, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the success function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, TResult> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return result.Match(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the success function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the success function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the success function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, TResult> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return result.Match(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the success function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, Task<TResult>> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the success function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    public static async Task<TResult> MatchAsync<TValue, TResult, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        var result = await task;
        return await result.MatchAsync(param, onSuccess, onFailure);
    }

    #endregion

    #region Result MapAsync

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
    /// <param name="valueFactory">The value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue>(this Task<Result> task, Task<TValue> valueFactory)
    {
        var result = await task;
        return await result.MapAsync(valueFactory);
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
    public static async Task<Result<TValue>> MapAsync<TValue>(this ValueTask<Result> task, TValue value)
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
    /// <param name="valueFactory">The value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue>(this ValueTask<Result> task, Task<TValue> valueFactory)
    {
        var result = await task;
        return await result.MapAsync(valueFactory);
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
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="valueFactory">A function to generate the value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue>(this Task<Result> task, Func<Task<TValue>> valueFactory)
    {
        var result = await task;
        return await result.MapAsync(valueFactory);
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
    public static async Task<Result<TValue>> MapAsync<TValue>(this ValueTask<Result> task, Func<TValue> valueFactory)
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
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="valueFactory">A function to generate the value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue>(this ValueTask<Result> task, Func<Task<TValue>> valueFactory)
    {
        var result = await task;
        return await result.MapAsync(valueFactory);
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
        Func<TParam, Task<TValue>> valueFactory)
    {
        var result = await task;
        return await result.MapAsync(param, valueFactory);
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
    public static async Task<Result<TValue>> MapAsync<TValue, TParam>(this ValueTask<Result> task,
        TParam param,
        Func<TParam, TValue> valueFactory)
    {
        var result = await task;
        return result.Map(param, valueFactory);
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
    public static async Task<Result<TValue>> MapAsync<TValue, TParam>(this ValueTask<Result> task,
        TParam param,
        Func<TParam, Task<TValue>> valueFactory)
    {
        var result = await task;
        return await result.MapAsync(param, valueFactory);
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
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="map">A function to generate the new result.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue>(this Task<Result> task, Func<Task<Result<TValue>>> map)
    {
        var result = await task;
        return await result.MapAsync(map);
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
    public static async Task<Result<TValue>> MapAsync<TValue>(this ValueTask<Result> task, Func<Result<TValue>> map)
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
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="map">A function to generate the new result.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public static async Task<Result<TValue>> MapAsync<TValue>(this ValueTask<Result> task, Func<Task<Result<TValue>>> map)
    {
        var result = await task;
        return await result.MapAsync(map);
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
        Func<TParam, Task<Result<TValue>>> map)
    {
        var result = await task;
        return await result.MapAsync(param, map);
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
    public static async Task<Result<TValue>> MapAsync<TValue, TParam>(this ValueTask<Result> task,
        TParam param,
        Func<TParam, Result<TValue>> map)
    {
        var result = await task;
        return result.Map(param, map);
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
    public static async Task<Result<TValue>> MapAsync<TValue, TParam>(this ValueTask<Result> task,
        TParam param,
        Func<TParam, Task<Result<TValue>>> map)
    {
        var result = await task;
        return await result.MapAsync(param, map);
    }

    #endregion

    #region Result<TValue> MapAsync 

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
        this Task<Result<TValue>> task,
        Func<TValue, TOther> map)
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
        this Task<Result<TValue>> task,
        Func<TValue, Task<TOther>> map)
    {
        var result = await task;
        return await result.MapAsync(map);
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
    public static async ValueTask<Result<TOther>> MapAsync<TValue, TOther>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, TOther> map)
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
    public static async ValueTask<Result<TOther>> MapAsync<TValue, TOther>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, Task<TOther>> map)
    {
        var result = await task;
        return await result.MapAsync(map);
    }

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public static async Task<Result<TOther>> MapAsync<TValue, TOther, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, TOther> map)
    {
        var result = await task;
        return result.Map(param, map);
    }

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public static async Task<Result<TOther>> MapAsync<TValue, TOther, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<TOther>> map)
    {
        var result = await task;
        return await result.MapAsync(param, map);
    }

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public static async ValueTask<Result<TOther>> MapAsync<TValue, TOther, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, TOther> map)
    {
        var result = await task;
        return result.Map(param, map);
    }

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public static async ValueTask<Result<TOther>> MapAsync<TValue, TOther, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<TOther>> map)
    {
        var result = await task;
        return await result.MapAsync(param, map);
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
        this Task<Result<TValue>> task,
        Func<TValue, Result<TOther>> map)
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
        this Task<Result<TValue>> task,
        Func<TValue, Task<Result<TOther>>> map)
    {
        var result = await task;
        return await result.MapAsync(map);
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
    public static async ValueTask<Result<TOther>> MapAsync<TValue, TOther>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, Result<TOther>> map)
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
    public static async ValueTask<Result<TOther>> MapAsync<TValue, TOther>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, Task<Result<TOther>>> map)
    {
        var result = await task;
        return await result.MapAsync(map);
    }

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public static async Task<Result<TOther>> MapAsync<TValue, TOther, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Result<TOther>> map)
    {
        var result = await task;
        return result.Map(param, map);
    }

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public static async Task<Result<TOther>> MapAsync<TValue, TOther, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<Result<TOther>>> map)
    {
        var result = await task;
        return await result.MapAsync(param, map);
    }

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public static async ValueTask<Result<TOther>> MapAsync<TValue, TOther, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Result<TOther>> map)
    {
        var result = await task;
        return result.Map(param, map);
    }

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public static async ValueTask<Result<TOther>> MapAsync<TValue, TOther, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<Result<TOther>>> map)
    {
        var result = await task;
        return await result.MapAsync(param, map);
    }

    #endregion

    #region Result ContinueAsync 

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result> ContinueAsync(
        this Task<Result> task, 
        Action action)
    {
        var result = await task;
        return result.Continue(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result> ContinueAsync(
        this Task<Result> task,
        Func<Task> action)
    {
        var result = await task;
        return await result.ContinueAsync(action);

    }
    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result> ContinueAsync(
        this ValueTask<Result> task,
        Action action)
    {
        var result = await task;
        return result.Continue(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result> ContinueAsync(
        this ValueTask<Result> task,
        Func<Task> action)
    {
        var result = await task;
        return await result.ContinueAsync(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or the action result.</returns>
    public static async Task<Result> ContinueAsync(
        this Task<Result> task, 
        Func<Result> action)
    {
        var result = await task;
        return result.Continue(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or the action result.</returns>
    public static async Task<Result> ContinueAsync(
        this Task<Result> task,
        Func<Task<Result>> action)
    {
        var result = await task;
        return await result.ContinueAsync(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or the action result.</returns>
    public static async Task<Result> ContinueAsync(
        this ValueTask<Result> task,
        Func<Result> action)
    {
        var result = await task;
        return result.Continue(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or the action result.</returns>
    public static async Task<Result> ContinueAsync(
        this ValueTask<Result> task,
        Func<Task<Result>> action)
    {
        var result = await task;
        return await result.ContinueAsync(action);
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
    public static async Task<Result> ContinueAsync<TParam>(
        this Task<Result> task, 
        TParam param, 
        Action<TParam> action)
    {
        var result = await task;
        return result.Continue(param, action);
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
    public static async Task<Result> ContinueAsync<TParam>(
        this Task<Result> task,
        TParam param,
        Func<TParam, Task> action)
    {
        var result = await task;
        return await result.ContinueAsync(param, action);
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
    public static async Task<Result> ContinueAsync<TParam>(
        this ValueTask<Result> task,
        TParam param,
        Action<TParam> action)
    {
        var result = await task;
        return result.Continue(param, action);
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
    public static async Task<Result> ContinueAsync<TParam>(
        this ValueTask<Result> task,
        TParam param,
        Func<TParam, Task> action)
    {
        var result = await task;
        return await result.ContinueAsync(param, action);
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
    /// <returns>The same result or the action result.</returns>
    public static async Task<Result> ContinueAsync<TParam>(
        this Task<Result> task,
        TParam param, 
        Func<TParam, Result> action)
    {
        var result = await task;
        return result.Continue(param, action);
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
    /// <returns>The same result or the action result.</returns>
    public static async Task<Result> ContinueAsync<TParam>(
        this Task<Result> task,
        TParam param,
        Func<TParam, Task<Result>> action)
    {
        var result = await task;
        return await result.ContinueAsync(param, action);
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
    /// <returns>The same result or the action result.</returns>
    public static async Task<Result> ContinueAsync<TParam>(
        this ValueTask<Result> task,
        TParam param,
        Func<TParam, Result> action)
    {
        var result = await task;
        return result.Continue(param, action);
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
    /// <returns>The same result or the action result.</returns>
    public static async Task<Result> ContinueAsync<TParam>(
        this ValueTask<Result> task,
        TParam param,
        Func<TParam, Task<Result>> action)
    {
        var result = await task;
        return await result.ContinueAsync(param, action);
    }

    #endregion

    #region Result<TValue> ContinueAsync 

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue>(
        this Task<Result<TValue>> task,
        Action<TValue> action)
    {
        var result = await task;
        return result.Continue(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue>(
        this Task<Result<TValue>> task,
        Func<TValue, Task> action)
    {
        var result = await task;
        return await result.ContinueAsync(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue>(
        this ValueTask<Result<TValue>> task,
        Action<TValue> action)
    {
        var result = await task;
        return result.Continue(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, Task> action)
    {
        var result = await task;
        return await result.ContinueAsync(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue>(
        this Task<Result<TValue>> task, 
        Func<TValue, Result> action)
    {
        var result = await task;
        return result.Continue(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue>(
        this Task<Result<TValue>> task,
        Func<TValue, Task<Result>> action)
    {
        var result = await task;
        return await result.ContinueAsync(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, Result> action)
    {
        var result = await task;
        return result.Continue(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue>(
        this ValueTask<Result<TValue>> task,
        Func<TValue, Task<Result>> action)
    {
        var result = await task;
        return await result.ContinueAsync(action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue, TParam>(
        this Task<Result<TValue>> task, 
        TParam param, 
        Action<TValue, TParam> action)
    {
        var result = await task;
        return result.Continue(param, action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task> action)
    {
        var result = await task;
        return await result.ContinueAsync(param, action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Action<TValue, TParam> action)
    {
        var result = await task;
        return result.Continue(param, action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task> action)
    {
        var result = await task;
        return await result.ContinueAsync(param, action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue, TParam>(
        this Task<Result<TValue>> task,
        TParam param, 
        Func<TValue, TParam, Result> action)
    {
        var result = await task;
        return result.Continue(param, action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue, TParam>(
        this Task<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<Result>> action)
    {
        var result = await task;
        return await result.ContinueAsync(param, action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Result> action)
    {
        var result = await task;
        return result.Continue(param, action);
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter of the action.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="task">The task of an operation that returns a result.</param>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    public static async Task<Result<TValue>> ContinueAsync<TValue, TParam>(
        this ValueTask<Result<TValue>> task,
        TParam param,
        Func<TValue, TParam, Task<Result>> action)
    {
        var result = await task;
        return await result.ContinueAsync(param, action);
    }

    #endregion


}