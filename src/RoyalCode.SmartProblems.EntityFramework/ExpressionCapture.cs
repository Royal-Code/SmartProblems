using System.Linq.Expressions;

namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// <para>
///     Trim-safe helper to turn a value into a closure-captured expression — exactly the shape EF Core's
///     funcletization already knows how to turn into a query parameter.
/// </para>
/// <para>
///     Using <see cref="Expression.Constant(object?)"/> directly produces a literal in the generated SQL:
///     EF Core only parameters member accesses on closures, not hand-built constants. Every distinct value
///     would then be a new entry in EF's query cache (recompiling the query) and a new ad hoc statement in
///     the database's plan cache.
/// </para>
/// </summary>
internal static class ExpressionCapture
{
    /// <summary>
    /// Wraps <paramref name="value"/> in a compiler-generated closure and returns the member access
    /// expression over it, so EF Core parameterizes it instead of inlining it as a literal.
    /// </summary>
    public static Expression Capture<TValue>(TValue value)
    {
        Expression<Func<TValue>> capture = () => value;
        return capture.Body;
    }
}
