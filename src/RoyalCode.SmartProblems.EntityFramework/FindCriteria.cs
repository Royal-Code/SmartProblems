using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// <para>
///     Fluent builder for composed entity searches (two or more fields, a composite key or otherwise),
///     producing a <see cref="FindResult{TEntity}"/> with a rich, standardized not-found problem listing
///     every criterion used.
/// </para>
/// <para>
///     Start with one of the <c>FindByCriteria</c> extensions on <see cref="Microsoft.EntityFrameworkCore.DbContext"/>,
///     <see cref="DbSet{TEntity}"/> or <see cref="IQueryable{T}"/>, add filters with <see cref="By{TValue}(Expression{Func{TEntity, TValue}}, TValue)"/>,
///     then execute with <see cref="TryFindAsync"/>.
/// </para>
/// <para>
///     Every <c>By</c> call returns a new, independent instance: assign the result back (e.g.
///     <c>criteria = criteria.By(...)</c>) to keep adding filters, including conditionally inside an
///     <c>if</c>. Filters are always combined with AND; a criterion that itself requires OR logic can be
///     expressed with the <see cref="By(Expression{Func{TEntity, bool}}, string, string, object)"/> overload.
/// </para>
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public readonly struct FindCriteria<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>
    where TEntity : class
{
    private readonly IQueryable<TEntity> query;
    private readonly CriterionNode? criteria;

    internal FindCriteria(IQueryable<TEntity> query)
    {
        this.query = query;
    }

    private FindCriteria(IQueryable<TEntity> query, CriterionNode? criteria)
    {
        this.query = query;
        this.criteria = criteria;
    }

    /// <summary>
    /// Filters by equality on a property. The display name used in the not-found message, if generated,
    /// is resolved via <see cref="DisplayNames"/> (respecting <see cref="System.ComponentModel.DisplayNameAttribute"/>).
    /// </summary>
    /// <typeparam name="TValue">Type of property value.</typeparam>
    /// <param name="propertySelector">Selector of the property, e.g. <c>e =&gt; e.Name</c>.</param>
    /// <param name="value">Value of the filter used.</param>
    /// <returns>A new <see cref="FindCriteria{TEntity}"/> with the filter added.</returns>
    /// <exception cref="ArgumentException">
    ///     If <paramref name="propertySelector"/> is not a direct member access on the entity parameter.
    /// </exception>
    public FindCriteria<TEntity> By<TValue>(Expression<Func<TEntity, TValue>> propertySelector, TValue value)
        => ByCore(propertySelector, value, byName: null);

    /// <summary>
    /// Same as <see cref="By{TValue}(Expression{Func{TEntity, TValue}}, TValue)"/>, with an explicit
    /// display name for the not-found message instead of resolving one via <see cref="DisplayNames"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of property value.</typeparam>
    /// <param name="propertySelector">Selector of the property, e.g. <c>e =&gt; e.Name</c>.</param>
    /// <param name="value">Value of the filter used.</param>
    /// <param name="byName">Display name of the field, used in the not-found message.</param>
    /// <returns>A new <see cref="FindCriteria{TEntity}"/> with the filter added.</returns>
    /// <exception cref="ArgumentException">
    ///     If <paramref name="propertySelector"/> is not a direct member access on the entity parameter.
    /// </exception>
    public FindCriteria<TEntity> By<TValue>(Expression<Func<TEntity, TValue>> propertySelector, TValue value, string byName)
        => ByCore(propertySelector, value, byName);

    private FindCriteria<TEntity> ByCore<TValue>(Expression<Func<TEntity, TValue>> propertySelector, TValue value, string? byName)
    {
        EnsureInitialized();

        var member = ValidateSelector(propertySelector);
        GuardIdWrapperMisuse<TValue>(member);

        var equal = Expression.Equal(propertySelector.Body, ExpressionCapture.Capture(value));
        var predicate = Expression.Lambda<Func<TEntity, bool>>(equal, propertySelector.Parameters);

        var node = new CriterionNode(new FindCriterion(member.Member.Name, value, byName), criteria);
        return new FindCriteria<TEntity>(query.Where(predicate), node);
    }

    /// <summary>
    /// Filters by an arbitrary predicate, with explicit data for the not-found message — mirrors
    /// <see cref="Microsoft.EntityFrameworkCore.SmartProblemsEFExtensions.TryFindByAsync{TEntity}(DbContext, Expression{Func{TEntity, bool}}, string, string, object?, CancellationToken)"/>.
    /// Use this when a single criterion needs logic beyond equality (e.g. <c>StartsWith</c>, ranges, OR).
    /// </summary>
    /// <param name="filter">Filter to find the entity.</param>
    /// <param name="byName">Name of the field through which the entity is queried.</param>
    /// <param name="propertyName">Name of the property to be added as extra data to the problem.</param>
    /// <param name="value">Value of the filter used.</param>
    /// <returns>A new <see cref="FindCriteria{TEntity}"/> with the filter added.</returns>
    public FindCriteria<TEntity> By(Expression<Func<TEntity, bool>> filter, string byName, string propertyName, object? value)
    {
        EnsureInitialized();

        var node = new CriterionNode(new FindCriterion(propertyName, value, byName), criteria);
        return new FindCriteria<TEntity>(query.Where(filter), node);
    }

    /// <summary>
    /// <para>
    ///     Executes the search (<c>FirstOrDefaultAsync</c> semantics: does not consult the local change
    ///     tracker, always a database roundtrip), returning the entity found or a rich not-found problem
    ///     listing every criterion added via <c>By</c>, in the order they were added.
    /// </para>
    /// </summary>
    /// <param name="ct">Optional, cancellation token.</param>
    /// <returns>A reference to the entity found or a problem if not found.</returns>
    public async Task<FindResult<TEntity>> TryFindAsync(CancellationToken ct = default)
    {
        EnsureInitialized();

        var entity = await query.FirstOrDefaultAsync(ct);
        if (entity is not null)
            return entity;

        return FindResult<TEntity>.Problem(MaterializeCriteria());
    }

    private void EnsureInitialized()
    {
        if (query is null)
        {
            throw new InvalidOperationException(
                "FindCriteria was not initialized. Do not use default(FindCriteria<TEntity>); " +
                "start with FindByCriteria(...) on a DbContext, a DbSet<TEntity> or an IQueryable<TEntity>.");
        }
    }

    /// <summary>
    /// <para>
    ///     Rejects <c>By(e =&gt; e.StateId, someIdWrapper)</c> with an actionable message.
    /// </para>
    /// <para>
    ///     Passing an <see cref="Id{TEntity, TId}"/> as the value compiles: <c>TValue</c> is inferred as the
    ///     wrapper (there is an implicit conversion from the raw value to it) and the selector body gets a
    ///     <c>Convert</c> node. It would then fail deep inside <see cref="Expression.Equal(Expression, Expression)"/>
    ///     with "the binary operator Equal is not defined for the types Id&lt;,&gt;", which says nothing about
    ///     the actual mistake.
    /// </para>
    /// </summary>
    private static void GuardIdWrapperMisuse<TValue>(MemberExpression member)
    {
        // a conversion was needed: the property type and the value type differ.
        if (member.Type == typeof(TValue))
            return;

        var valueType = typeof(TValue);
        if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Id<,>))
        {
            throw new ArgumentException(
                $"Cannot filter '{member.Member.Name}' (of type {member.Type.Name}) by an Id<,> wrapper. " +
                $"Pass the underlying value instead, e.g. By(e => e.{member.Member.Name}, id.Value).",
                "value");
        }
    }

    private static MemberExpression ValidateSelector<TValue>(Expression<Func<TEntity, TValue>> propertySelector)
    {
        var body = propertySelector.Body;
        while (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
            body = unary.Operand;

        if (body is not MemberExpression member || !ReferenceEquals(member.Expression, propertySelector.Parameters[0]))
        {
            throw new ArgumentException(
                "Property selector must be a direct member access on the entity parameter, e.g. e => e.Name.",
                nameof(propertySelector));
        }

        return member;
    }

    private FindCriterion[] MaterializeCriteria()
    {
        var node = criteria;
        if (node is null)
            return [];

        var array = new FindCriterion[node.Count];
        for (var i = node.Count - 1; i >= 0; i--, node = node!.Previous)
            array[i] = node!.Criterion;

        return array;
    }

    private sealed class CriterionNode(FindCriterion criterion, CriterionNode? previous)
    {
        public readonly FindCriterion Criterion = criterion;
        public readonly CriterionNode? Previous = previous;
        public readonly int Count = (previous?.Count ?? 0) + 1;
    }
}
