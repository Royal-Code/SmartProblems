using RoyalCode.SmartProblems.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

#pragma warning disable IDE0130 // Namespace does not match folder structure: extension methods for EF.

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extensions for SmartProblems to work with Entity Framework.
/// </summary>
public static partial class SmartProblemsEFExtensions
{
    /// <summary>
    /// It tries to find an entity in the database using the identifier provided.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TId">The type of identifier.</typeparam>
    /// <param name="db">Database context.</param>
    /// <param name="id">Identifier of the entity to be found.</param>
    /// <param name="ct">Optional, cancellation token.</param>
    /// <returns>A reference to the entity found or a problem if not found.</returns>
    public static async Task<FindResult<TEntity, TId>> TryFindAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TId>(
        this DbContext db, 
        Id<TEntity, TId> id,
        CancellationToken ct = default)
        where TEntity : class
    {
        var entity = await db.Set<TEntity>().FindAsync([id.Value], cancellationToken: ct);
        return new FindResult<TEntity, TId>(entity, id.Value);
    }

    /// <summary>
    /// It tries to find an entity in the database using the identifier provided.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TId">The type of identifier.</typeparam>
    /// <param name="set">Collection of database entities.</param>
    /// <param name="id">Identifier of the entity to be found.</param>
    /// <param name="ct">Optional, cancellation token.</param>
    /// <returns>A reference to the entity found or a problem if not found.</returns>
    public static async Task<FindResult<TEntity, TId>> TryFindAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TId>(
        this DbSet<TEntity> set,
        TId id,
        CancellationToken ct = default)
        where TEntity : class
    {
        var entity = await set.FindAsync([id], cancellationToken: ct);
        return new FindResult<TEntity, TId>(entity, id);
    }

    /// <summary>
    /// <para>
    ///     It tries to find an entity in the database using the identifier provided.
    /// </para>
    /// <para>
    ///     Without this overload, a call passing an <see cref="Id{TEntity, TId}"/> would bind to the
    ///     <typeparamref name="TId"/> overload above, inferring <c>TId = Id&lt;TEntity, TId&gt;</c> and handing
    ///     the whole wrapper to <c>FindAsync</c>, which the EF key comparison then rejects.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TId">The type of identifier.</typeparam>
    /// <param name="set">Collection of database entities.</param>
    /// <param name="id">Identifier of the entity to be found.</param>
    /// <param name="ct">Optional, cancellation token.</param>
    /// <returns>A reference to the entity found or a problem if not found.</returns>
    public static async Task<FindResult<TEntity, TId>> TryFindAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TId>(
        this DbSet<TEntity> set,
        Id<TEntity, TId> id,
        CancellationToken ct = default)
        where TEntity : class
    {
        var entity = await set.FindAsync([id.Value], cancellationToken: ct);
        return new FindResult<TEntity, TId>(entity, id.Value);
    }

    /// <summary>
    /// Try to find an entity in the database using the filter provided.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="db">Database context.</param>
    /// <param name="filter">Filter to find the entity.</param>
    /// <param name="ct">Optional, cancellation token.</param>
    /// <returns>A reference to the entity found or a problem if not found.</returns>
    public static async Task<FindResult<TEntity>> TryFindByAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>(
        this DbContext db,
        Expression<Func<TEntity, bool>> filter,
        CancellationToken ct = default)
        where TEntity : class
    {
        return await db.Set<TEntity>().FirstOrDefaultAsync(filter, ct)
            ?? GenerateProblem(filter);
    }

    /// <summary>
    /// Try to find an entity in the database using the filter provided.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="set">Collection of database entities.</param>
    /// <param name="filter">Filter to find the entity.</param>
    /// <param name="ct">Optional, cancellation token.</param>
    /// <returns>A reference to the entity found or a problem if not found.</returns>
    public static async Task<FindResult<TEntity>> TryFindByAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>(
        this DbSet<TEntity> set,
        Expression<Func<TEntity, bool>> filter,
        CancellationToken ct = default)
        where TEntity : class
    {
        return await set.FirstOrDefaultAsync(filter, ct)
            ?? GenerateProblem(filter);
    }

    /// <summary>
    /// Try to find an entity in the database using the filter provided.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="db">Database context.</param>
    /// <param name="filter">Filter to find the entity.</param>
    /// <param name="byName">Name of the field through which the entity is queried.</param>
    /// <param name="propertyName">Name of the property containing the filter value.</param>
    /// <param name="propertyValue">Value of the filter used.</param>
    /// <param name="ct">Optional, cancellation token.</param>
    /// <returns>A reference to the entity found or a problem if not found.</returns>
    public static async Task<FindResult<TEntity>> TryFindByAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>(
        this DbContext db,
        Expression<Func<TEntity, bool>> filter,
        string byName, string propertyName, object? propertyValue,
        CancellationToken ct = default)
        where TEntity : class
    {
        return await db.Set<TEntity>().FirstOrDefaultAsync(filter, ct)
            ?? FindResult<TEntity>.Problem(byName, propertyName, propertyValue);
    }

    /// <summary>
    /// Try to find an entity in the database using the filter provided.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="set">Collection of database entities.</param>
    /// <param name="filter">Filter to find the entity.</param>
    /// <param name="byName">Name of the field through which the entity is queried.</param>
    /// <param name="propertyName">Name of the property containing the filter value.</param>
    /// <param name="propertyValue">Value of the filter used.</param>
    /// <param name="ct">Optional, cancellation token.</param>
    /// <returns>A reference to the entity found or a problem if not found.</returns>
    public static async Task<FindResult<TEntity>> TryFindByAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>(
        this DbSet<TEntity> set,
        Expression<Func<TEntity, bool>> filter,
        string byName, string propertyName, object? propertyValue,
        CancellationToken ct = default)
        where TEntity : class
    {
        return await set.FirstOrDefaultAsync(filter, ct)
            ?? FindResult<TEntity>.Problem(byName, propertyName, propertyValue);
    }

    /// <summary>
    /// It tries to find an entity in the database using the value of the given property.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TValue">Type of property value.</typeparam>
    /// <param name="db">Database context.</param>
    /// <param name="propertySelector">Selector of the property through which the entity will be consulted.</param>
    /// <param name="filterValue">Value of the filter used.</param>
    /// <param name="ct">Optional, cancellation token.</param>
    /// <returns>
    ///     A reference to the entity found or a problem if not found.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     If the property selection expression is not valid or is not a property.
    /// </exception>
    public static async Task<FindResult<TEntity>> TryFindByAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity, TValue>(
        this DbContext db,
        Expression<Func<TEntity, TValue>> propertySelector,
        TValue filterValue,
        CancellationToken ct = default)
        where TEntity : class
    {
        // validates if propertySelector is a valid expression, which is a property
        if (propertySelector.Body is not MemberExpression propertyExpression)
            throw new ArgumentException("Property selector must be a property expression.", nameof(propertySelector));

        var predicateExpression = Expression.Lambda<Func<TEntity, bool>>(
            Expression.Equal(propertySelector.Body, ExpressionCapture.Capture(filterValue)),
            propertySelector.Parameters);

        return await db.Set<TEntity>().FirstOrDefaultAsync(predicateExpression, ct)
            ?? GenerateProblem<TEntity, TValue>(propertyExpression.Member.Name, filterValue);
    }

    /// <summary>
    /// It tries to find an entity in the database using the value of the given property.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TValue">Type of property value.</typeparam>
    /// <param name="set">Collection of database entities.</param>
    /// <param name="propertySelector">Selector of the property through which the entity will be consulted.</param>
    /// <param name="filterValue">Value of the filter used.</param>
    /// <param name="ct">Optional, cancellation token.</param>
    /// <returns>
    ///     A reference to the entity found or a problem if not found.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     If the property selection expression is not valid or is not a property.
    /// </exception>
    public static async Task<FindResult<TEntity>> TryFindByAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity, TValue>(
        this DbSet<TEntity> set,
        Expression<Func<TEntity, TValue>> propertySelector,
        TValue filterValue,
        CancellationToken ct = default)
        where TEntity : class
    {
        // validates if propertySelector is a valid expression, which is a property
        if (propertySelector.Body is not MemberExpression propertyExpression)
            throw new ArgumentException("Property selector must be a property expression.", nameof(propertySelector));

        var predicateExpression = Expression.Lambda<Func<TEntity, bool>>(
            Expression.Equal(propertySelector.Body, ExpressionCapture.Capture(filterValue)),
            propertySelector.Parameters);

        return await set.FirstOrDefaultAsync(predicateExpression, ct)
            ?? GenerateProblem<TEntity, TValue>(propertyExpression.Member.Name, filterValue);
    }

    private static FindResult<TEntity> GenerateProblem<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity, TValue>(string propertyName, TValue filterValue)
    {
        // extracts display name from property
        var displayName = DisplayNames.Instance.GetDisplayName(typeof(TEntity), propertyName);

        return FindResult<TEntity>.Problem(displayName, propertyName, filterValue);
    }

    /// <summary>
    /// <para>
    ///     Builds a rich not-found problem from a filter expression, delegating the best-effort
    ///     extraction of <see cref="FindCriterion"/> to <see cref="FindCriteriaExtractor"/>: one
    ///     criterion per <c>&amp;&amp;</c>-joined equality, all-or-nothing, never throwing. When the
    ///     expression cannot be safely converted, the generic not-found message is used. Use
    ///     <see cref="FindCriteria{TEntity}"/> to supply the values directly and skip expression
    ///     analysis altogether.
    /// </para>
    /// </summary>
    private static FindResult<TEntity> GenerateProblem<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>(Expression<Func<TEntity, bool>> filter)
    {
        return FindResult<TEntity>.Problem(FindCriteriaExtractor.Extract(filter));
    }
}
