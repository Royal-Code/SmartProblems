using RoyalCode.SmartProblems.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

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
    ///     Builds a rich not-found problem from a filter expression, extracting one <see cref="FindCriterion"/>
    ///     per <c>&amp;&amp;</c>-joined <b>equality</b> whose "property" side is a direct member access on the
    ///     lambda's parameter (e.g. <c>e =&gt; e.Name == value</c>, in either order).
    /// </para>
    /// <para>
    ///     This is best-effort message generation over a predicate that has already executed the actual
    ///     query: it must never throw. Any construct it doesn't recognize — an operator other than
    ///     <c>==</c> (<c>!=</c>, <c>&gt;</c>, <c>&lt;</c>, …), <c>||</c> anywhere in the tree, a non-direct
    ///     member (<c>e.A == e.B</c>, <c>e.State.Name == x</c>), a method call on the value side, a bare
    ///     boolean member, negation, etc. — degrades the whole predicate to the generic not-found message
    ///     rather than producing a partial or misleading one.
    /// </para>
    /// <para>
    ///     Value extraction never compiles the expression nor invokes methods, but it does read fields and
    ///     property getters of captured closures (so <c>e =&gt; e.Email == request.Email</c> yields a rich
    ///     message). A captured property whose getter has side effects, or which returns a different value
    ///     on each read, will therefore be read once more than the query itself did. Use
    ///     <see cref="FindCriteria{TEntity}"/> to supply the values directly and skip expression analysis
    ///     altogether.
    /// </para>
    /// </summary>
    private static FindResult<TEntity> GenerateProblem<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>(Expression<Func<TEntity, bool>> filter)
    {
        List<FindCriterion> criteria = [];

        try
        {
            if (!TryCollectCriteria(filter.Body, filter.Parameters[0], criteria))
                criteria.Clear();
        }
        catch
        {
            criteria.Clear();
        }

        return FindResult<TEntity>.Problem(criteria.ToArray());
    }

    /// <summary>
    /// Recursively collects one criterion per <c>&amp;&amp;</c>-joined leaf. Returns false — meaning the
    /// caller must discard everything collected so far — as soon as a leaf that is not an extractable
    /// equality is found anywhere in the tree (<c>||</c> included: it is not an <c>==</c>).
    /// </summary>
    private static bool TryCollectCriteria(Expression node, ParameterExpression parameter, List<FindCriterion> criteria)
    {
        if (node is BinaryExpression { NodeType: ExpressionType.AndAlso } andAlso)
        {
            return TryCollectCriteria(andAlso.Left, parameter, criteria)
                && TryCollectCriteria(andAlso.Right, parameter, criteria);
        }

        // only equality yields a criterion: the message format ("with Name 'X' was not found") asserts
        // the entity has that value, which is false for !=, >, <, >=, <= and every other operator.
        if (node is not BinaryExpression { NodeType: ExpressionType.Equal } binary)
            return false;

        var left = UnwrapConvert(binary.Left);
        var right = UnwrapConvert(binary.Right);

        var leftIsMember = IsDirectMember(left, parameter);
        var rightIsMember = IsDirectMember(right, parameter);

        Expression propertyExpression;
        Expression valueExpression;

        if (leftIsMember && !rightIsMember)
        {
            propertyExpression = left;
            valueExpression = right;
        }
        else if (rightIsMember && !leftIsMember)
        {
            propertyExpression = right;
            valueExpression = left;
        }
        else
        {
            // both sides are (or neither side is) a direct member of the parameter: ambiguous
            // (e.A == e.B) or unsupported (deep chain, capture-vs-capture) — degrade.
            return false;
        }

        if (!TryEvaluateValue(valueExpression, out var value))
            return false;

        var propertyName = ((MemberExpression)propertyExpression).Member.Name;
        criteria.Add(new FindCriterion(propertyName, value));
        return true;
    }

    private static bool IsDirectMember(Expression expression, ParameterExpression parameter)
        => expression is MemberExpression member && ReferenceEquals(member.Expression, parameter);

    private static Expression UnwrapConvert(Expression expression)
    {
        while (expression is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
            expression = unary.Operand;
        return expression;
    }

    /// <summary>
    /// Evaluates a pure member-access chain (closure fields/properties, optionally static) without ever
    /// compiling the expression or invoking methods — a <see cref="MethodCallExpression"/> or any other
    /// node fails. Note that reading a property does run its getter, which is user code (see the remarks
    /// on <c>GenerateProblem</c>); only <see cref="Expression.Lambda(Expression, ParameterExpression[])"/>
    /// plus <c>Compile</c> and explicit method calls are ruled out.
    /// </summary>
    private static bool TryEvaluateValue(Expression expression, out object? value)
    {
        switch (expression)
        {
            case ConstantExpression constant:
                value = constant.Value;
                return true;

            case MemberExpression { Expression: null } member:
                // static field/property: no target instance to resolve first.
                return TryReadMember(member, null, out value);

            case MemberExpression member:
                if (!TryEvaluateValue(member.Expression, out var target))
                {
                    value = null;
                    return false;
                }
                return TryReadMember(member, target, out value);

            default:
                value = null;
                return false;
        }
    }

    private static bool TryReadMember(MemberExpression member, object? target, out object? value)
    {
        try
        {
            switch (member.Member)
            {
                case FieldInfo field:
                    value = field.GetValue(target);
                    return true;
                case PropertyInfo property:
                    value = property.GetValue(target);
                    return true;
                default:
                    value = null;
                    return false;
            }
        }
        catch
        {
            value = null;
            return false;
        }
    }
}
