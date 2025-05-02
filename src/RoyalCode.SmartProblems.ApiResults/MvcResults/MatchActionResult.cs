using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoyalCode.SmartProblems.MvcResults;

/// <summary>
/// A Mvc <see cref="ObjectResult"/> for <see cref="Result"/>.
/// </summary>
public sealed class MatchActionResult : MatchObjectResultBase<Result>
{
    #region Implicit Operators

    /// <summary>
    /// Creates a new <see cref="MatchActionResult"/> for the <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to be converted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MatchActionResult(Result result) => new(result);

    /// <summary>
    /// Creates a new <see cref="MatchActionResult"/> for the <see cref="Problems"/>.
    /// </summary>
    /// <param name="problems">The collection of problems.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MatchActionResult(Problems problems) => new(problems);
    
    /// <summary>
    /// Creates a new <see cref="MatchActionResult"/> for the <see cref="Problem"/>.
    /// </summary>
    /// <param name="problem">The problem.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MatchActionResult(Problem problem) => new(problem);

    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="MatchActionResult"/>.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">Optional, the path created by the operation.</param>
    public MatchActionResult(
        Result result,
        string? createdPath = null) : base(result, createdPath)
    { }

    /// <inheritdoc />
    protected override Task ExecuteMatchAsync(ActionContext context)
        => Result.Match(context, ExecuteSuccessResultAsync, ExecuteErrorResultAsync);

    private Task ExecuteSuccessResultAsync(ActionContext context)
    {
        Value = null;

        if (CreatedPath is not null)
        {
            StatusCode = StatusCodes.Status201Created;
            context.HttpContext.Response.Headers.Append("Location", CreatedPath);
        }
        else
        {
            StatusCode = StatusCodes.Status204NoContent;
        }
        
        return BaseExecuteResultAsync(context);
    }
}

/// <summary>
/// A MVC <see cref="ObjectResult"/> for <see cref="Result{TValue}"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public sealed class MatchActionResult<TValue> : MatchObjectResultBase<Result<TValue>>
{
    #region Implicit Operators

    /// <summary>
    /// Creates a new <see cref="MatchActionResult{TValue}"/> for the <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="result">The <see cref="Result{TValue}"/> to be converted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MatchActionResult<TValue>(Result<TValue> result) => new(result);
    
    /// <summary>
    /// Creates a new <see cref="MatchActionResult{TValue}"/> for the <see cref="Problems"/>.
    /// </summary>
    /// <param name="problems">The collection of problems.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MatchActionResult<TValue>(Problems problems) => new(problems);
    
    /// <summary>
    /// Creates a new <see cref="MatchActionResult{TValue}"/> for the <see cref="Problem"/>.
    /// </summary>
    /// <param name="problem">The problem.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MatchActionResult<TValue>(Problem problem) => new(problem);
    
    /// <summary>
    /// Creates a new <see cref="MatchActionResult{TValue}"/> for the <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MatchActionResult<TValue>(TValue value) => new(value);

    #endregion
    
    /// <summary>
    /// Creates a new instance of <see cref="MatchActionResult{TValue}"/>.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">Optional, the path created by the operation.</param>
    /// <param name="formatPathWithValue">If true, the <paramref name="createdPath"/> will be formatted with the value of the result.</param>
    public MatchActionResult(
        Result<TValue> result,
        string? createdPath = null,
        bool formatPathWithValue = false) : base(result, createdPath)
    {
        FormatPathWithValue = formatPathWithValue;
    }

    /// <summary>
    /// Creates a new instance of <see cref="MatchActionResult{TValue}"/>.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPathProvider">A function to provide the value for the <c>Location</c> header.</param>
    public MatchActionResult(Result<TValue> result, Func<TValue, string> createdPathProvider) 
        : base(result)
    {
        CreatedPathProvider = createdPathProvider;
    }

    /// <summary>
    /// If true, the <see cref="MatchObjectResultBase{TResult}.CreatedPath"/> will be formatted with the value of the result.
    /// </summary>
    public bool FormatPathWithValue { get; }

    /// <summary>
    /// A function to provide the value for the <c>Location</c> header.
    /// </summary>
    public Func<TValue, string>? CreatedPathProvider { get; set; }

    /// <inheritdoc />
    protected override Task ExecuteMatchAsync(ActionContext context)
        => Result.Match(context, ExecuteSuccessResultAsync, ExecuteErrorResultAsync);

    private Task ExecuteSuccessResultAsync(TValue value, ActionContext context)
    {
        Value = value;

        if (CreatedPath is not null)
        {
            StatusCode = StatusCodes.Status201Created;

            var createdPath = FormatPathWithValue
                    ? string.Format(CreatedPath, value)
                    : CreatedPath;
            
            context.HttpContext.Response.Headers.Append("Location", createdPath);
        }
        else if (CreatedPathProvider is not null)
        {
            StatusCode = StatusCodes.Status201Created;
            context.HttpContext.Response.Headers.Append("Location", CreatedPathProvider(value));
        }
        else
        {
            StatusCode = StatusCodes.Status200OK;
        }

        return BaseExecuteResultAsync(context);
    }
}