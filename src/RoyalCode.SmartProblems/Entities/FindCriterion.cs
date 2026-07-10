namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// <para>
///     A criterion used to search for an entity, used to generate a rich not-found problem message
///     when combined with other criteria via <see cref="FindResult{TEntity}.Problem(ReadOnlySpan{FindCriterion})"/>.
/// </para>
/// </summary>
public readonly struct FindCriterion
{
    /// <summary>
    /// Creates a new <see cref="FindCriterion"/>.
    /// </summary>
    /// <param name="propertyName">Name of the property, used as extension data key of the problem.</param>
    /// <param name="value">Value used in the filter.</param>
    /// <param name="byName">
    ///     Display name used in the problem message. When null or whitespace, it is resolved via
    ///     <see cref="DisplayNames"/> at problem generation time.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     If <paramref name="propertyName"/> is null, empty or whitespace: it becomes an extension
    ///     data key of the problem, and the problem generation path must never throw.
    /// </exception>
    public FindCriterion(string propertyName, object? value, string? byName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        PropertyName = propertyName;
        Value = value;
        ByName = byName;
    }

    /// <summary>
    /// Name of the property, used as extension data key of the problem.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Value used in the filter.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Display name used in the problem message. When null or whitespace, it is resolved via
    /// <see cref="DisplayNames"/> at problem generation time.
    /// </summary>
    public string? ByName { get; }
}
