using RoyalCode.SmartProblems.Entities;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0130 // Namespace does not match folder structure: extension methods for EF.

namespace Microsoft.EntityFrameworkCore;

public static partial class SmartProblemsEFExtensions
{
    /// <summary>
    /// Starts a composed search for an entity that requires more than one field to identify — a composite
    /// key or otherwise — combining filters with <see cref="FindCriteria{TEntity}.By{TValue}(System.Linq.Expressions.Expression{Func{TEntity, TValue}}, TValue)"/>
    /// and executing with <see cref="FindCriteria{TEntity}.TryFindAsync(CancellationToken)"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="db">Database context.</param>
    /// <returns>A new <see cref="FindCriteria{TEntity}"/> to add filters to.</returns>
    public static FindCriteria<TEntity> FindByCriteria<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>(
        this DbContext db)
        where TEntity : class
        => new(db.Set<TEntity>());

    /// <summary>
    /// Starts a composed search for an entity that requires more than one field to identify — a composite
    /// key or otherwise — combining filters with <see cref="FindCriteria{TEntity}.By{TValue}(System.Linq.Expressions.Expression{Func{TEntity, TValue}}, TValue)"/>
    /// and executing with <see cref="FindCriteria{TEntity}.TryFindAsync(CancellationToken)"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="set">Collection of database entities.</param>
    /// <returns>A new <see cref="FindCriteria{TEntity}"/> to add filters to.</returns>
    public static FindCriteria<TEntity> FindByCriteria<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>(
        this DbSet<TEntity> set)
        where TEntity : class
        => new(set);

    /// <summary>
    /// <para>
    ///     Starts a composed search for an entity that requires more than one field to identify — a
    ///     composite key or otherwise — combining filters with <see cref="FindCriteria{TEntity}.By{TValue}(System.Linq.Expressions.Expression{Func{TEntity, TValue}}, TValue)"/>
    ///     and executing with <see cref="FindCriteria{TEntity}.TryFindAsync(CancellationToken)"/>.
    /// </para>
    /// <para>
    ///     Use this overload to search over a query with <c>Include</c>, <c>AsNoTracking</c> or similar
    ///     already applied.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="query">The query to search over.</param>
    /// <returns>A new <see cref="FindCriteria{TEntity}"/> to add filters to.</returns>
    public static FindCriteria<TEntity> FindByCriteria<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>(
        this IQueryable<TEntity> query)
        where TEntity : class
        => new(query);
}
