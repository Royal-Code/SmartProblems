using RoyalCode.SmartProblems.Entities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

// ReSharper disable ParameterHidesMember

namespace RoyalCode.SmartProblems;

/// <summary>
/// <para>
///     Struct that represents a result of a request.
/// </para>
/// <para>
///     It can be a success or a failure. 
///     In case of success, it contains the value.
///     In case of failure, it contains a list of problems.
/// </para>
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public readonly struct Result<TValue>
{
    #region Implicit Operators

    /// <summary>
    /// <para>
    ///     Converts a value to a successful result.
    /// </para>
    /// </summary>
    /// <param name="value">The value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TValue>(TValue value) => new(value);

    /// <summary>
    /// <para>
    ///     Converts a problem to a failed result.
    /// </para>
    /// </summary>
    /// <param name="problem">The problem.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TValue>(Problem problem) => new([problem]);

    /// <summary>
    /// <para>
    ///     Converts a list of problems to a failed result.
    /// </para>
    /// </summary>
    /// <param name="problems">The problems.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TValue>(Problems problems) => new(problems);

    /// <summary>
    /// <para>
    ///     Converts an exception to a failed result.
    /// </para>
    /// </summary>
    /// <param name="ex">The exception.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TValue>(Exception ex) => new([Problems.InternalError(ex)]);

    /// <summary>
    /// <para>
    ///     Converts a result with a value to a result without a value.
    /// </para>
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    ///     A new result without a value.
    /// </returns>
    public static implicit operator Result(Result<TValue> result) => result.IsSuccess ? Result.Ok() : result.problems;

    /// <summary>
    /// Implicit operator for converting a <see cref="FindResult{TEntity}"/> into a result.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TValue>(FindResult<TValue> result) => result.ToResult();

    /// <summary>
    /// <para>
    ///     Adds a problem to a failed result.
    /// </para>
    /// <para>
    ///     When the result is successful, it throws an <see cref="InvalidOperationException"/>.
    /// </para>
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="problem">The problem to add.</param>
    /// <returns>
    ///     The result with the added problem.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     When the result is successful.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue> operator +(Result<TValue> result, Problem problem)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException(R.CanNotAddMessage);

        result.problems.Add(problem);
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a list of problems to a failed result.
    /// </para>
    /// <para>
    ///     When the result is successful, it throws an <see cref="InvalidOperationException"/>.
    /// </para>
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="problems">The problems to add.</param>
    /// <returns>
    ///     The result with the added problems.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     When the result is successful.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue> operator +(Result<TValue> result, Problems problems)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException(R.CanNotAddMessage);

        result.problems.AddRange(problems);
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds the problems of a result to another failed result.
    /// </para>
    /// <para>
    ///     When the result is successful, it throws an <see cref="InvalidOperationException"/>.
    /// </para>
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="other">The result with the problems to add.</param>
    /// <returns>
    ///     The result with the added problems.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     When the result is successful.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue> operator +(Result<TValue> result, Result other)
    {
        if (!other.HasProblems(out var otherProblems))
            return result;

        if (result.IsSuccess)
            throw new InvalidOperationException(R.CanNotAddMessage);

        result.problems.AddRange(otherProblems);
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
    public static Result operator +(Result result, Result<TValue> other)
    {
        if (other.IsSuccess)
            return result;

        result += other.problems;
        return result;
    }

    /// <summary>
    /// <para>
    ///     AAdds the problems of a result to another failed result.
    /// </para>
    /// <para>
    ///     When the result is successful, it throws an <see cref="InvalidOperationException"/>.
    /// </para>
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="other">The result with the problems to add.</param>
    /// <returns>
    ///     The result with the added problems.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     When the result is successful.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue> operator +(Result<TValue> result, Result<TValue> other)
    {
        if (other.IsSuccess)
            return result;

        if (result.IsSuccess)
            throw new InvalidOperationException(R.CanNotAddMessage);

        result.problems.AddRange(other.problems);
        return result;
    }

    #endregion

    private readonly TValue? value;
    private readonly Problems? problems;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The value.</param>
    public Result(TValue value) 
    {
        ArgumentNullException.ThrowIfNull(value);
        this.value = value;
    }

    /// <summary>
    /// Creates a failed result.
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
    [MemberNotNullWhen(true, nameof(value))]
    public bool IsSuccess => problems is null;

    /// <summary>
    /// Checks if the result is a failure.
    /// </summary>
    [MemberNotNullWhen(true, nameof(problems))]
    [MemberNotNullWhen(false, nameof(value))]
    public bool IsFailure => problems is not null;

    /// <summary>
    /// Gets the problems of the result.
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

    /// <summary>
    /// Validates the result and throws an exception if it's a failure.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureSuccess()
    {
        if (IsFailure)
            throw problems.ToException();
    }

    /// <summary>
    /// Validates the result and throws an exception if it's a failure.
    /// When success returns the value.
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureHasValue([NotNull] out TValue value)
    {
        if (IsFailure)
            throw problems.ToException();

        value = this.value;
    }

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
    ///     Check if the result is a failure and return the problems.
    /// </para>
    /// <para>
    ///     When the result is a success, returns the value.
    /// </para>
    /// </summary>
    /// <param name="problems">he problems.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// <para>
    ///     True if the result is a failure, false otherwise.
    /// </para>
    /// <para>
    ///     The value will be null when the result is a failure.
    /// </para>
    /// <para>
    ///     The problems will be null when the result is a success.
    /// </para>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasProblemsOrGetValue([NotNullWhen(true)] out Problems? problems, [NotNullWhen(false)] out TValue? value)
    {
        problems = this.problems;
        value = this.value;
        return IsFailure;
    }

    /// <summary>
    /// <para>
    ///     Check if the result is a success and return the value.
    /// </para>
    /// <para>
    ///     When the result is a failure, the value will be null.
    /// </para>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     True if the result is a success, false otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasValue([NotNullWhen(true)] out TValue? value)
    {
        value = this.value;
        return IsSuccess;
    }

    /// <summary>
    /// <para>
    ///     Check if the result is a success and return the value.
    /// </para>
    /// <para>
    ///     When the result is a failure, the value will be null and the problems will be set.
    /// </para>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="problems">he problems.</param>
    /// <returns>
    /// <para>
    ///     True if the result is a success, false otherwise.
    /// </para>
    /// <para>
    ///     The value will be null when the result is a failure.
    /// </para>
    /// <para>
    ///     The problems will be null when the result is a success.
    /// </para>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasValueOrGetProblems([NotNullWhen(true)] out TValue? value, [NotNullWhen(false)] out Problems? problems)
    {
        value = this.value;
        problems = this.problems;
        return IsSuccess;
    }

    #endregion

    #region Match/Async

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(value) : onFailure(problems);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the functions.</typeparam>
    /// <param name="param">The parameter passed to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult, TParam>(
        TParam param,
        Func<TValue, TParam, TResult> onSuccess,
        Func<Problems, TParam, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(value, param) : onFailure(problems, param);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the success function.</typeparam>
    /// <param name="param">The parameter passed to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult, TParam>(
        TParam param,
        Func<TValue, TParam, TResult> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(value, param) : onFailure(problems);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> MatchAsync<TResult>(
        Func<TValue, Task<TResult>> onSuccess,
        Func<Problems, Task<TResult>> onFailure)
    {
        return IsSuccess ? onSuccess(value) : onFailure(problems);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> MatchAsync<TResult>(
        Func<TValue, Task<TResult>> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(value) : Task.FromResult(onFailure(problems));
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the functions.</typeparam>
    /// <param name="param">The parameter passed to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> MatchAsync<TResult, TParam>(
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, TParam, Task<TResult>> onFailure)
    {
        return IsSuccess ? onSuccess(value, param) : onFailure(problems, param);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the functions.</typeparam>
    /// <param name="param">The parameter passed to the functions.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> MatchAsync<TResult, TParam>(
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, TParam, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(value, param) : Task.FromResult(onFailure(problems, param));
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the success function.</typeparam>
    /// <param name="param">The parameter passed to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> MatchAsync<TResult, TParam>(
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, Task<TResult>> onFailure)
    {
        return IsSuccess ? onSuccess(value, param) : onFailure(problems);
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the result, if it is a success or a failure.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the success function.</typeparam>
    /// <param name="param">The parameter passed to the success function.</param>
    /// <param name="onSuccess">The function to execute when the result is a success.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    /// <returns>
    ///     The result of the executed function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> MatchAsync<TResult, TParam>(
        TParam param,
        Func<TValue, TParam, Task<TResult>> onSuccess,
        Func<Problems, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(value, param) : Task.FromResult(onFailure(problems));
    }

    #endregion

    #region Map/Async

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TOther> Map<TOther>(Func<TValue, TOther> map)
    {
        return IsSuccess ? map(value) : new Result<TOther>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TOther> Map<TOther, TParam>(TParam param, Func<TValue, TParam, TOther> map)
    {
        return IsSuccess ? map(value, param) : new Result<TOther>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TOther> Map<TOther>(Func<TValue, Result<TOther>> map)
    {
        return IsSuccess ? map(value) : new Result<TOther>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TOther> Map<TOther, TParam>(TParam param, Func<TValue, TParam, Result<TOther>> map)
    {
        return IsSuccess ? map(value, param) : new Result<TOther>(problems);
    }
    
    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A value task with the new result with the mapped value, when the result is a success,
    ///     otherwise the result with the problems.
    /// </returns>
    public async ValueTask<Result<TOther>> MapAsync<TOther>(Func<TValue, Task<TOther>> map)
    {
        return IsSuccess ? await map(value) : new Result<TOther>(problems);
    }
    
    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public async ValueTask<Result<TOther>> MapAsync<TOther, TParam>(TParam param, Func<TValue, TParam, Task<TOther>> map)
    {
        return IsSuccess ? await map(value, param) : new Result<TOther>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map a new value to a result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     A new result with the mapped value, when the result is a success, otherwise the result with the problems.
    /// </returns>
    public async ValueTask<Result<TOther>> MapAsync<TOther, TParam>(
        TParam param,
        CancellationToken ct,
        Func<TValue, TParam, CancellationToken, Task<TOther>> map)
    {
        return IsSuccess ? await map(value, param, ct) : new Result<TOther>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public async ValueTask<Result<TOther>> MapAsync<TOther>(Func<TValue, Task<Result<TOther>>> map)
    {
        return IsSuccess ? await map(value) : new Result<TOther>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public async ValueTask<Result<TOther>> MapAsync<TOther, TParam>(
        TParam param, 
        Func<TValue, TParam, Task<Result<TOther>>> map)
    {
        return IsSuccess ? await map(value, param) : new Result<TOther>(problems);
    }

    /// <summary>
    /// <para>
    ///     Map to a new result when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The new type of the value.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the function.</typeparam>
    /// <param name="param">The parameter passed to the function.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <param name="map">A function to map the value.</param>
    /// <returns>
    ///     The new result of the function, when the result is a success,
    ///     a new result with the problems, otherwise.
    /// </returns>
    public async ValueTask<Result<TOther>> MapAsync<TOther, TParam>(
        TParam param,
        CancellationToken ct,
        Func<TValue, TParam, CancellationToken, Task<Result<TOther>>> map)
    {
        return IsSuccess ? await map(value, param, ct) : new Result<TOther>(problems);
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
    public Result<TValue> Continue(Action<TValue> action)
    {
        if (IsSuccess)
            action(value);

        return this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Continue(Func<TValue, Result> action)
    {
        if (IsSuccess)
        {
            var actionResult = action(value);
            if (actionResult.HasProblems(out var actionProblems))
                return actionProblems;
        }

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
    /// <returns>The same result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Continue<TParam>(TParam param, Action<TValue, TParam> action)
    {
        if (IsSuccess)
            action(value, param);

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
    /// <returns>The same result or a new with the problems.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Continue<TParam>(TParam param, Func<TValue, TParam, Result> action)
    {
        if (IsSuccess)
        {
            var actionResult = action(value, param);
            if (actionResult.HasProblems(out var actionProblems))
                return actionProblems;
        }

        return this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result<TValue>> ContinueAsync(Func<TValue, Task> action)
    {
        if (IsSuccess)
            await action(value);

        return this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result<TValue>> ContinueAsync(Func<TValue, Task<Result>> action)
    {
        if (IsSuccess)
        {
            var actionResult = await action(value);
            if (actionResult.HasProblems(out var actionProblems))
                return actionProblems;
        }

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
    /// <returns>The same result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result<TValue>> ContinueAsync<TParam>(TParam param, Func<TValue, TParam, Task> action)
    {
        if (IsSuccess)
            await action(value, param);

        return this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result<TValue>> ContinueAsync<TParam>(
        TParam param,
        CancellationToken ct,
        Func<TValue, TParam, CancellationToken, Task> action)
    {
        if (IsSuccess)
            await action(value, param, ct);

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
    /// <returns>The same result or a new with the problems.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result<TValue>> ContinueAsync<TParam>(TParam param, Func<TValue, TParam, Task<Result>> action)
    {
        if (IsSuccess)
        {
            var actionResult = await action(value, param);
            if (actionResult.HasProblems(out var actionProblems))
                return actionProblems;
        }

        return this;
    }

    /// <summary>
    /// <para>
    ///     Execute an action when the result is a success.
    /// </para>
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter of the action.</typeparam>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The same result or a new with the problems.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<Result<TValue>> ContinueAsync<TParam>(
        TParam param,
        CancellationToken ct,
        Func<TValue, TParam, CancellationToken, Task<Result>> action)
    {
        if (IsSuccess)
        {
            var actionResult = await action(value, param, ct);
            if (actionResult.HasProblems(out var actionProblems))
                return actionProblems;
        }

        return this;
    }

    #endregion
}
