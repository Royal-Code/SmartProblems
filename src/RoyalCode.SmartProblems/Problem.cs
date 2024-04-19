namespace RoyalCode.SmartProblems;

/// <summary>
/// An object that represents a problem that occurred in the system, with details and category.
/// </summary>
public sealed class Problem
{
    /// <summary>
    /// <para>
    ///     Description of the problem, like a message to the user.
    /// </para>
    /// </summary>
    public required string Detail { get; init; }
    
    /// <summary>
    /// <para>
    ///     The category of the problem.
    /// </para>
    /// </summary>
    public required ProblemCategory Category { get; init; }
    
    /// <summary>
    /// <para>
    ///    The type id of the problem, related to the problem details type.
    /// </para>
    /// </summary>
    public string? TypeId { get; init; }
    
    /// <summary>
    /// Optional, the property that the problem is related to.
    /// </summary>
    public string? Property { get; init; }
    
    /// <summary>
    /// Optional, extra information about the problem.
    /// </summary>
    public IDictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// Adds a new key-value pair to the extensions.
    /// </summary>
    /// <param name="key">The key of the data.</param>
    /// <param name="value">The value of the data.</param>
    /// <returns>
    ///     The same instance of the problem.
    /// </returns>
    public Problem With(string key, object value)
    {
        Extensions ??= new Dictionary<string, object>();
        Extensions[key] = value;
        return this;
    }
}