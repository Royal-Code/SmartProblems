
#pragma warning disable IDE0130 // Namespace does not match folder structure: extension methods for EF.

using System.Runtime.CompilerServices;
using RoyalCode.SmartProblems;
using RoyalCode.SmartProblems.Entities;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extensions for SmartProblems to work with Entity Framework.
/// </summary>
public static partial class SmartProblemsEFExtensions
{
    #region AddTo

    /// <summary>
    /// Adds the entity to the DbContext if the result is successful.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="result">The result containing the entity.</param>
    /// <param name="context">The DbContext to add the entity to.</param>
    /// <returns>The original result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TEntity> AddTo<TEntity>(this Result<TEntity> result, DbContext context) 
        where TEntity : class
    {
        return result.Continue(context, static (entity, ctx) =>
        {
            ctx.Add(entity);
        });
    }

    /// <summary>
    /// Adds the entity to the DbContext if the result is successful, asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="result">The result containing the entity.</param>
    /// <param name="context">The DbContext to add the entity to.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<TEntity>> AddToAsync<TEntity>(
        this Result<TEntity> result, DbContext context, CancellationToken ct = default) 
        where TEntity : class
    {
        return result.ContinueAsync(context, ct, static async (entity, ctx, ct) =>
        {
            await ctx.AddAsync(entity, ct);
        });
    }

    /// <summary>
    /// Adds the entity to the DbContext if the result is successful, asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="result">The result containing the entity.</param>
    /// <param name="context">The DbContext to add the entity to.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The original result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TEntity>> AddToAsync<TEntity>(
        this Task<Result<TEntity>> result, DbContext context, CancellationToken ct = default) 
        where TEntity : class
    {
        return result.ContinueAsync(context, ct, static async (entity, ctx, ct) =>
        {
            await ctx.AddAsync(entity, ct);
        });
    }

    /// <summary>
    /// Adds the entity to the DbContext if the result is successful, asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="result">The result containing the entity.</param>
    /// <param name="context">The DbContext to add the entity to.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The original result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TEntity>> AddToAsync<TEntity>(
        this ValueTask<Result<TEntity>> result, DbContext context, CancellationToken ct = default) 
        where TEntity : class
    {
        return result.ContinueAsync(context, ct, static async (entity, ctx, ct) =>
        {
            await ctx.AddAsync(entity, ct);
        });
    }

    #endregion

    #region SaveChanges

    /// <summary>
    /// Saves the changes in the DbContext if the result is successful.
    /// </summary>
    /// <param name="result">The result indicating whether the operation was successful.</param>
    /// <param name="context">The DbContext to save changes in.</param>
    /// <returns>The original result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result SaveChanges(this Result result, DbContext context)
    {
        return result.Continue(context, static (ctx) =>
        {
            ctx.SaveChanges();
        });
    }

    /// <summary>
    /// Saves the changes in the DbContext if the result is successful, asynchronously.
    /// </summary>
    /// <param name="result">The result indicating whether the operation was successful.</param>
    /// <param name="context">The DbContext to save changes in.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The original result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result> SaveChangesAsync(
        this Result result, DbContext context, CancellationToken ct = default)
    {
        return result.ContinueAsync(context, ct, static async (ctx, ct) =>
        {
            await ctx.SaveChangesAsync(ct);
        });
    }

    /// <summary>
    /// Saves the changes in the DbContext if the result is successful, asynchronously.
    /// </summary>
    /// <param name="result">The result indicating whether the operation was successful.</param>
    /// <param name="context">The DbContext to save changes in.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The original result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result> SaveChangesAsync(
        this Task<Result> result, DbContext context, CancellationToken ct = default)
    {
        return result.ContinueAsync(context, ct, static async (ctx, ct) =>
        {
            await ctx.SaveChangesAsync(ct);
        });
    }

    /// <summary>
    /// Saves the changes in the DbContext if the result is successful.
    /// </summary>
    /// <param name="result">The result indicating whether the operation was successful.</param>
    /// <param name="context">The DbContext to save changes in.</param>
    /// <returns>The original result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TEntity> SaveChanges<TEntity>(this Result<TEntity> result, DbContext context)
        where TEntity : class
    {
        return result.Continue(context, static (_, ctx) =>
        {
            ctx.SaveChanges();
        });
    }

    /// <summary>
    /// Saves the changes in the DbContext if the result is successful, asynchronously.
    /// </summary>
    /// <param name="result">The result indicating whether the operation was successful.</param>
    /// <param name="context">The DbContext to save changes in.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The original result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<TEntity>> SaveChangesAsync<TEntity>(
        this Result<TEntity> result, DbContext context, CancellationToken ct = default)
        where TEntity : class
    {
        return result.ContinueAsync(context, ct, static async (_, ctx, ct) =>
        {
            await ctx.SaveChangesAsync(ct);
        });
    }

    /// <summary>
    /// Saves the changes in the DbContext if the result is successful, asynchronously.
    /// </summary>
    /// <param name="result">The result indicating whether the operation was successful.</param>
    /// <param name="context">The DbContext to save changes in.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The original result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TEntity>> SaveChangesAsync<TEntity>(
        this Task<Result<TEntity>> result, DbContext context, CancellationToken ct = default)
        where TEntity : class
    {
        return result.ContinueAsync(context, ct, static async (_, ctx, ct) =>
        {
            await ctx.SaveChangesAsync(ct);
        });
    }

    #endregion

    #region RemoveFrom

    /// <summary>
    /// Removes the entity from the DbContext if the result is successful.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="task">The task of an operation that returns a find result.</param>
    /// <param name="context">The DbContext to remove the entity from.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Result{TEntity}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TEntity>> RemoveFromAsync<TEntity>(
        this Task<FindResult<TEntity>> task, 
        DbContext context, 
        CancellationToken ct)
        where TEntity : class
    {
        return task.ContinueAsync(context, ct, static async (entity, ctx, ct) =>
        {
            ctx.Remove(entity);
            return Result.Ok();
        });
    }

    #endregion

}