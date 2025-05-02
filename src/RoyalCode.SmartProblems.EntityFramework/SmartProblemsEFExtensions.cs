using RoyalCode.SmartProblems.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure: extension methods for EF.

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extensions for SmartProblems to work with Entity Framework.
/// </summary>
public static class SmartProblemsEFExtensions
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
            Expression.Equal(propertySelector.Body, Expression.Constant(filterValue)),
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
            Expression.Equal(propertySelector.Body, Expression.Constant(filterValue)),
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

    private static FindResult<TEntity> GenerateProblem<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>(Expression<Func<TEntity, bool>> filter)
    {
        // validate filter expression, which must be a comparison expression
        if (filter.Body is not BinaryExpression binaryExpression)
            throw new ArgumentException("Filter must be a binary expression.", nameof(filter));

        // one part of the expression must be a property and the other a value
        if (binaryExpression.Left is not MemberExpression propertyExpression)
            throw new ArgumentException("Filter must be a binary expression with a property.", nameof(filter));

        // extracts the name of the property
        var propertyName = propertyExpression.Member.Name;

        // gets the display name of the property
        var displayName = DisplayNames.Instance.GetDisplayName(typeof(TEntity), propertyName);

        // extracts the value of the expression
        var valueExpression = binaryExpression.Right;

        object? value = null;

        // checks if the value is member access
        if (valueExpression is MemberExpression memberExpression && memberExpression.Expression is MemberExpression memberAccess)
        {
            // checks if the value is a constant
            if (memberAccess.Expression is not ConstantExpression constantExpression)
                throw new ArgumentException("Filter must be a binary expression with a constant value.", nameof(filter));

            object? closure = constantExpression.Value;
            object? filterObj;

            if (memberAccess.Member is FieldInfo field1)
            {
                filterObj = field1.GetValue(closure);
            }
            else
            {
                throw new InvalidOperationException("Unsupported member type for closure access.");
            }

            if (memberExpression.Member is PropertyInfo property)
            {
                value = property.GetValue(filterObj);
            }
            else
            {
                value = filterObj;
            }
        }
        else if (valueExpression is MemberExpression memberAccessExpression)
        {
            // checks if the value is a constant
            if (memberAccessExpression.Expression is not ConstantExpression constantExpression)
                throw new ArgumentException("Filter must be a binary expression with a constant value.", nameof(filter));

            object? closure = constantExpression.Value;
            if (memberAccessExpression.Member is FieldInfo field)
            {
                value = field.GetValue(closure);
            }
        }
        else
        {
            // checks if the value is a constant
            if (valueExpression is not ConstantExpression constantExpression)
                throw new ArgumentException("Filter must be a binary expression with a constant value.", nameof(filter));

            value = constantExpression.Value;
        }

        // generates the problem
        return FindResult<TEntity>.Problem(displayName, propertyName, value);
    }
}
