using System.Linq.Expressions;
using System.Reflection;

namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// <para>
///     Best-effort extraction of <see cref="FindCriterion"/> from a filter expression, used to generate
///     rich not-found problem messages from a predicate that has already executed the actual query.
/// </para>
/// <para>
///     One criterion is extracted per <c>&amp;&amp;</c>-joined <b>equality</b> whose "property" side is a
///     direct member access on the lambda's parameter (e.g. <c>e =&gt; e.Name == value</c>, in either order).
/// </para>
/// <para>
///     The extraction is all-or-nothing and never throws. Any construct it doesn't recognize — an operator
///     other than <c>==</c> (<c>!=</c>, <c>&gt;</c>, <c>&lt;</c>, …), <c>||</c> anywhere in the tree, a
///     non-direct member (<c>e.A == e.B</c>, <c>e.State.Name == x</c>), a method call on the value side, a
///     bare boolean member, negation, etc. — degrades the whole predicate to an empty result rather than
///     producing a partial or misleading one.
/// </para>
/// <para>
///     Value extraction never compiles the expression nor invokes methods, but it does read fields and
///     property getters of captured closures (so <c>e =&gt; e.Email == request.Email</c> yields a rich
///     criterion). A captured property whose getter has side effects, or which returns a different value
///     on each read, will therefore be read once more than the query itself did. To skip expression
///     analysis altogether, supply the criteria directly, e.g. via
///     <see cref="FindResult{TEntity}.Problem(ReadOnlySpan{FindCriterion})"/>.
/// </para>
/// </summary>
public static class FindCriteriaExtractor
{
    /// <summary>
    /// <para>
    ///     Extracts one <see cref="FindCriterion"/> per <c>&amp;&amp;</c>-joined equality of the
    ///     <paramref name="filter"/>, in declaration order.
    /// </para>
    /// <para>
    ///     All-or-nothing: when any part of the expression is not an extractable equality, an empty
    ///     array is returned. This method never throws for a filter that compiles.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The type of entity filtered by the expression.</typeparam>
    /// <param name="filter">The filter expression used to query the entity.</param>
    /// <returns>
    ///     The extracted criteria, in declaration order, or an empty array when the expression
    ///     contains any construct that cannot be safely converted into criteria.
    /// </returns>
    public static FindCriterion[] Extract<TEntity>(Expression<Func<TEntity, bool>> filter)
    {
        List<FindCriterion> criteria = [];

        try
        {
            if (!TryCollectCriteria(filter.Body, filter.Parameters[0], criteria))
                return [];
        }
        catch
        {
            return [];
        }

        return [.. criteria];
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
    /// on <see cref="FindCriteriaExtractor"/>); only <see cref="Expression.Lambda(Expression, ParameterExpression[])"/>
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
