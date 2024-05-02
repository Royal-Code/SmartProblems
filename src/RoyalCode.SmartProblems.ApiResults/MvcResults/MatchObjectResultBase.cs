using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoyalCode.SmartProblems.Conversions;
using RoyalCode.SmartProblems.Descriptions;

namespace RoyalCode.SmartProblems.MvcResults;

/// <summary>
/// Abstract MVC <see cref="ObjectResult"/> for <see cref="Result"/> and <see cref="Result{TValue}"/>.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public abstract class MatchObjectResultBase<TResult> : ObjectResult
{
    /// <summary>
    /// Creates a new instance of <see cref="MatchObjectResultBase{TResult}"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="createdPath">Optional, the path created by the operation.</param>
    protected MatchObjectResultBase(TResult result,
        string? createdPath = null)
        : base(result)
    {
        Result = result;
        CreatedPath = createdPath;
    }

    /// <summary>
    /// The result object, instance of <see cref="Result"/> or <see cref="Result{TValue}"/>
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// The path created by the operation.
    /// </summary>
    public string? CreatedPath { get; }

    /// <inheritdoc />
    public override Task ExecuteResultAsync(ActionContext context)
    {
        return ExecuteMatchAsync(context);
    }

    /// <summary>
    /// Protected method to execute the result.
    /// </summary>
    /// <param name="context">The <see cref="ActionContext"/>.</param>
    /// <returns>A <see cref="Task"/> that will complete when the result is executed.</returns>
    protected Task BaseExecuteResultAsync(ActionContext context) => base.ExecuteResultAsync(context);

    /// <summary>
    /// Executes the match for the result.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected abstract Task ExecuteMatchAsync(ActionContext context);

    /// <summary>
    /// Executes the result for the given <paramref name="error"/>.
    /// </summary>
    /// <param name="error">The result error.</param>
    /// <param name="context">The <see cref="ActionContext"/>.</param>
    /// <returns>A <see cref="Task"/> that will complete when the result is executed.</returns>
    protected Task ExecuteErrorResultAsync(Problems error, ActionContext context)
    {
        var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ProblemDetailsOptions>>().Value;
        var problemDetails = error.ToProblemDetails(options);

        Value = problemDetails;
        ContentTypes.Add("application/problem+json");
        StatusCode = problemDetails.Status;
        DeclaredType = typeof(ProblemDetails);

        return base.ExecuteResultAsync(context);
    }
}