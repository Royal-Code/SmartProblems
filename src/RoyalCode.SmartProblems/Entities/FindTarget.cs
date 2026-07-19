using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// <para>
///     Descriptor of the original entity searched by a projected find operation
///     (see <see cref="FindResult{TEntity}.ProjectedFrom{TSource}(TEntity, IReadOnlyList{FindCriterion})"/>
///     and <see cref="FindResult{TEntity, TId}.ProjectedFrom{TSource}(TEntity, TId)"/>).
/// </para>
/// <para>
///     A projected result carries a DTO type as its generic argument, but the record that was not found
///     is the original entity: this descriptor preserves the entity display name, the entity technical
///     name and the normalized criteria so both <see cref="ProblemCategory.NotFound"/> and
///     <see cref="ProblemCategory.InvalidParameter"/> problems can be generated lazily, with the correct
///     category, naming the entity instead of the DTO.
/// </para>
/// </summary>
internal sealed class FindTarget
{
    /// <summary>
    /// Creates a descriptor for the entity type <paramref name="entityType"/>, normalizing the criteria:
    /// a criterion without <see cref="FindCriterion.ByName"/> gets it resolved against the entity via
    /// <see cref="DisplayNames"/>, and criteria with an invalid property name are ignored (mirroring
    /// <see cref="FindResult{TEntity}.Problem(ReadOnlySpan{FindCriterion})"/>).
    /// </summary>
    /// <param name="entityType">The type of the original entity searched.</param>
    /// <param name="criteria">The criteria used in the search, in declaration order.</param>
    /// <returns>A new descriptor with the entity identity and the normalized criteria.</returns>
    public static FindTarget Create(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type entityType,
        IReadOnlyList<FindCriterion> criteria)
    {
        List<FindCriterion>? normalized = null;

        for (var i = 0; i < criteria.Count; i++)
        {
            var criterion = criteria[i];
            if (string.IsNullOrWhiteSpace(criterion.PropertyName))
                continue;

            var byName = string.IsNullOrWhiteSpace(criterion.ByName)
                ? DisplayNames.Instance.GetDisplayName(entityType, criterion.PropertyName)
                : criterion.ByName!;

            (normalized ??= []).Add(new FindCriterion(criterion.PropertyName, criterion.Value, byName));
        }

        return new FindTarget(
            DisplayNames.Instance.GetDisplayName(entityType),
            entityType.Name,
            normalized is null ? [] : [.. normalized]);
    }

    private readonly FindCriterion[] criteria;

    private FindTarget(string displayName, string typeName, FindCriterion[] criteria)
    {
        DisplayName = displayName;
        TypeName = typeName;
        this.criteria = criteria;
    }

    /// <summary>
    /// Display name of the original entity, used in the problem details.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Technical name of the original entity, used as the <c>entity</c> extension data of the problem.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Creates the <see cref="ProblemCategory.NotFound"/> problem for the criteria-based search.
    /// </summary>
    public Problem CreateNotFound() => CreateProblem(invalidParameter: false, parameterName: null);

    /// <summary>
    /// Creates the <see cref="ProblemCategory.InvalidParameter"/> problem for the criteria-based search.
    /// </summary>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    public Problem CreateInvalidParameter(string? parameterName) => CreateProblem(invalidParameter: true, parameterName);

    private Problem CreateProblem(bool invalidParameter, string? parameterName)
    {
        string detail;
        if (criteria.Length is 0)
        {
            detail = string.Format(R.EntityNotFound, DisplayName);
        }
        else if (criteria.Length is 1)
        {
            detail = string.Format(R.EntityNotFoundBy, DisplayName, criteria[0].ByName, criteria[0].Value);
        }
        else
        {
            var pairs = new string[criteria.Length];
            for (var i = 0; i < criteria.Length; i++)
                pairs[i] = $"{criteria[i].ByName} '{criteria[i].Value}'";

            detail = string.Format(R.EntityNotFoundByMany, DisplayName, string.Join(", ", pairs));
        }

        var problem = invalidParameter
            ? Problems.InvalidParameter(detail, parameterName)
            : Problems.NotFound(detail);

        problem = problem.With("entity", TypeName);

        foreach (var criterion in criteria)
            problem = problem.With(criterion.PropertyName, criterion.Value);

        return problem;
    }
}
