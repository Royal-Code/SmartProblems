using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems;

/// <summary>
/// <para>
///     Struct that represents a result of a request.
/// </para>
/// <para>
///     It can be a success or a failure. In case of failure, it contains a list of problems.
/// </para>
/// </summary>
public readonly struct Result
{
    #region Implicit Operators

    /// <summary>
    /// <para>
    ///     Converts a problem to a result.
    /// </para>
    /// <para>
    ///     The result will be a failure with the given problem.
    /// </para>
    /// </summary>
    /// <param name="problem">The problem.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result(Problem problem) => new([problem]);

    /// <summary>
    /// <para>
    ///     Converts a list of problems to a result.
    /// </para>
    /// <para>
    ///     The result will be a failure with the given problems.
    /// </para>
    /// </summary>
    /// <param name="problems">The problems.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result(Problems problems) => new(problems);

    /// <summary>
    /// <para>
    ///     Generate a problem from the exception and converts it to a result.
    /// </para>
    /// <para>
    ///     The result will be a failure with the generated problem.
    /// </para>
    /// </summary>
    /// <param name="ex">The exception.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result(Exception ex) => new([Problems.InternalError(ex)]);

    /// <summary>
    /// <para>
    ///     Adds a problem to a result.
    /// </para>
    /// <para>
    ///     If the result is a success, the result will be a failure with the given problem.
    /// </para>
    /// </summary>
    /// <param name="result">The previous result.</param>
    /// <param name="problem">The problem to add.</param>
    /// <returns>
    ///     A new result with the added problem, when the previous result is not a failure,
    ///     or the previous result with the added problem.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result operator +(Result result, Problem problem)
    {
        if (!result.IsFailure)
            return problem;

        result.problems.Add(problem);
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a list of problems to a result.
    /// </para>
    /// <para>
    ///     If the result is a success, the result will be a failure with the given problem.
    /// </para>
    /// </summary>
    /// <param name="result">The previous result.</param>
    /// <param name="problems">The problems to add.</param>
    /// <returns>
    ///     A new result with the added problem, when the previous result is not a failure,
    ///     or the previous result with the added problem.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result operator +(Result result, Problems problems)
    {
        if (!result.IsFailure)
            return new Result(problems);

        result.problems.AddRange(problems);
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds the problems of a result to another result.
    /// </para>
    /// </summary>
    /// <param name="result">The previous result.</param>
    /// <param name="other">Other result.</param>
    /// <returns>
    ///     A new result with the added problem, when the previous result is not a failure,
    ///     or the previous result with the added problem.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result operator +(Result result, Result other)
    {
        if (other.IsSuccess)
            return result;

        if (!result.IsFailure)
            return new Result(other.problems);

        result.problems.AddRange(other.problems);
        return result;
    }

    #endregion

    #region Factory methods

    /// <summary>
    /// <para>
    ///     An elegant way to create a new result of success.
    /// </para>
    /// </summary>
    /// <returns>A new result of success.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result Ok() => new();
    
    #endregion
    
    private readonly Problems? problems;

    /// <summary>
    /// Creates a new result of success.
    /// </summary>
    public Result() { }

    /// <summary>
    /// Creates a new result of failure with the given problem.
    /// </summary>
    /// <param name="problems">The problems.</param>
    public Result(Problems problems)
    {
        ArgumentNullException.ThrowIfNull(problems);
        this.problems = problems;
    }

    /// <summary>
    /// Checks if the result is a success.
    /// </summary>
    [MemberNotNullWhen(false, nameof(problems))]
    public bool IsSuccess => problems is null;

    /// <summary>
    /// Checks if the result is a failure.
    /// </summary>
    [MemberNotNullWhen(true, nameof(problems))]
    public bool IsFailure => problems is not null;

    /// <summary>
    /// Get a problem of the result with the given index.
    /// </summary>
    /// <param name="index">The index of the problem.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    ///     if the result is not a failure.
    /// </exception>
    public Problem this[int index] 
    {
        get
        {
            if (IsSuccess)
                throw new InvalidOperationException(R.ResultIsNotAFailure);

            return problems[index];
        }
    }

    #region Match 

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The new type of the result.</typeparam>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The new result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(
        Func<TResult> onSuccess, 
        Func<Problems, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(problems);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The new type of the result.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the functions.</typeparam>
    /// <param name="param">A parameter to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The new result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult, TParam>(
        TParam param,
        Func<TParam, TResult> onSuccess,
        Func<Problems, TParam, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(param) : onFailure(problems, param);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The new type of the result.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the functions.</typeparam>
    /// <param name="param">The parameter to pass to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The new result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult, TParam>(
        TParam param,
        Func<TParam, TResult> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(param) : onFailure(problems);
    }

    #endregion

    #region Has/Is 

    /// <summary>
    /// <para>
    ///     Check if the result is a failure and return the problems.
    /// </para>
    /// <para>
    ///     When the result is a success, the problems will be null.
    /// </para>
    /// </summary>
    /// <param name="problems">The problems.</param>
    /// <returns>
    ///     True if the result is a failure, false otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        problems = this.problems;
        return IsFailure;
    }

    /// <summary>
    /// <para>
    ///     Check if the result is a success and return the problems.
    /// </para>
    /// <para>
    ///     When the result is a success, the problems will be null, otherwise the problems will be returned.
    /// </para>
    /// </summary>
    /// <param name="problems">The problems.</param>
    /// <returns>
    ///     True if the result is a success, false otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSuccessOrGetProblems([NotNullWhen(false)] out Problems? problems)
    {
        problems = this.problems;
        return IsSuccess;
    }

    #endregion

    #region Map/Async

    /// <summary>
    /// <para>
    ///     Map a value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(TValue value)
    {
        return IsSuccess ? new Result<TValue>(value) : new Result<TValue>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map a value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="valueFactory">A function to generate the value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(Func<TValue> valueFactory)
    {
        return IsSuccess ? new Result<TValue>(valueFactory()) : new Result<TValue>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map a value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the function.</typeparam>
    /// <param name="param">The parameter to pass to the function.</param>
    /// <param name="valueFactory">A function to generate the value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue, TParam>(TParam param, Func<TParam, TValue> valueFactory)
    {
        return IsSuccess ? valueFactory(param) : new Result<TValue>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="map">A function to generate the new result.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(Func<Result<TValue>> map)
    {
        return IsSuccess ? map() : new Result<TValue>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the function.</typeparam>
    /// <param name="param">The parameter to pass to the function.</param>  
    /// <param name="map">A function to generate the new result.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue, TParam>(TParam param, Func<TParam, Result<TValue>> map)
    {
        return IsSuccess ? map(param) : new Result<TValue>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map a value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="valueFactory">A function to generate the value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result<TValue>> MapAsync<TValue>(Func<Task<TValue>> valueFactory)
    {
        return IsSuccess ? new Result<TValue>(await valueFactory()) : new Result<TValue>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map a value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the function.</typeparam>
    /// <param name="param">The parameter to pass to the function.</param>
    /// <param name="valueFactory">A function to generate the value.</param>
    /// <returns>
    ///     A new result with the value, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result<TValue>> MapAsync<TValue, TParam>(TParam param, Func<TParam, Task<TValue>> valueFactory)
    {
        return IsSuccess ? await valueFactory(param) : new Result<TValue>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="map">A function to generate the new result.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result<TValue>> MapAsync<TValue>(Func<Task<Result<TValue>>> map)
    {
        return IsSuccess ? await map() : new Result<TValue>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter of the function.</typeparam>
    /// <param name="param">The parameter to pass to the function.</param>  
    /// <param name="map">A function to generate the new result.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result<TValue>> MapAsync<TValue, TParam>(TParam param, Func<TParam, Task<Result<TValue>>> map)
    {
        return IsSuccess ? await map(param) : new Result<TValue>(problems);
    }

    #endregion

    #region Continue/Async

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Continue(Action action)
    {
        if (IsSuccess)
            action();

        return this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or the action result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Continue(Func<Result> action)
    {
        return IsSuccess ? action() : this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Continue<TParam>(TParam param, Action<TParam> action)
    {
        if (IsSuccess)
            action(param);

        return this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or the action result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Continue<TParam>(TParam param, Func<TParam, Result> action)
    {
        return IsSuccess ? action(param) : this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result> ContinueAsync(Func<Task> action)
    {
        if (IsSuccess)
            await action();

        return this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or the action result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result> ContinueAsync(Func<Task<Result>> action)
    {
        return IsSuccess ? await action() : this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result> ContinueAsync<TParam>(TParam param, Func<TParam, Task> action)
    {
        if (IsSuccess)
            await action(param);

        return this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or the action result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result> ContinueAsync<TParam>(TParam param, Func<TParam, Task<Result>> action)
    {
        return IsSuccess ? await action(param) : this;
    }

    #endregion
}