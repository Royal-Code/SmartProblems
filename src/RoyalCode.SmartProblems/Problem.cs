using System.Runtime.CompilerServices;
using System.Text;

namespace RoyalCode.SmartProblems;

#pragma warning disable S4050 // implement operatiors == != Equals GetHashCode

/// <summary>
/// An object that represents a problem that occurred in the system, with details and category.
/// </summary>
public sealed class Problem
{
    #region implicit operators

    /// <summary>
    /// Creates a collection of problems with both problems.
    /// </summary>
    /// <param name="a">A problem to add.</param>
    /// <param name="b">Other problem to add.</param>
    /// <returns>A new instance of the problems collection (<see cref="Problems"/>).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Problems operator +(Problem a, Problem b)
    {
        Problems collection = [a, b];
        return collection;
    }

    #endregion

    #region Static functions

    /// <summary>
    /// Default function to convert the problem to a string.
    /// </summary>
    public Func<Problem, string> ToStringFactory { get; set; } = DefaultToString;

    #endregion

    private string? _property;

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
    public string? Property { get => _property; init => _property = value; }
    
    /// <summary>
    /// Optional, extra information about the problem.
    /// </summary>
    public IDictionary<string, object?>? Extensions { get; set; }

    /// <summary>
    /// Adds a new key-value pair to the extensions.
    /// </summary>
    /// <param name="key">The key of the data.</param>
    /// <param name="value">The value of the data.</param>
    /// <returns>
    ///     The same instance of the problem.
    /// </returns>
    public Problem With(string key, object? value)
    {
        Extensions ??= new Dictionary<string, object?>(StringComparer.Ordinal);
        Extensions[key] = value;
        return this;
    }

    /// <summary>
    /// Adds a new key-value pair to the extensions.
    /// </summary>
    /// <param name="key">The key of the data.</param>
    /// <param name="value">The value of the data.</param>
    /// <returns>
    ///     The same instance of the problem.
    /// </returns>
    public Problem With<TEnum>(string key, TEnum value)
        where TEnum : Enum
    {
        return With(key, value.ToString());
    }

    /// <summary>
    /// Chains a property to the beginning of the current property, forming a nested property (e.g. Parent.Current).
    /// </summary>
    /// <remarks>
    ///     When <see cref="Property"/> or <paramref name="parentProperty"/> is null or empty, this method does nothing.
    /// </remarks>
    /// <param name="parentProperty">Name of the property to be chained.</param>
    /// <returns>The same instance of the problem.</returns>
    public Problem ChainProperty(string? parentProperty)
    {
        if (string.IsNullOrEmpty(parentProperty) || string.IsNullOrEmpty(_property))
            return this;

        _property = $"{parentProperty}.{Property}";
        return this;
    }

    /// <summary>
    /// Chains a collection property with index to the beginning of the current property (e.g. Parent[0].Current).
    /// </summary>
    /// <remarks>
    ///     When <see cref="Property"/> or <paramref name="parentProperty"/> is null or empty, this method does nothing.
    /// </remarks>
    /// <param name="parentProperty">Name of the property to be chained.</param>
    /// <param name="index">Collection index.</param>
    /// <returns>The same instance of the problem.</returns>
    public Problem ChainProperty(string? parentProperty, int index)
    {
        if (string.IsNullOrEmpty(parentProperty) || string.IsNullOrEmpty(_property))
            return this;

        _property = $"{parentProperty}[{index}].{Property}";
        return this;
    }

    /// <inheritdoc />
    public override string ToString() => ToStringFactory(this);

    /// <summary>
    /// Default function to convert the problem to a string.
    /// </summary>
    /// <param name="problem">The problem to convert.</param>
    /// <returns>A string representation of the problem.</returns>
    private static string DefaultToString(Problem problem)
    {
        var builder = new StringBuilder();

        builder.Append($"Category: {problem.Category}, ");
        builder.Append($"Details: {problem.Detail}, ");

        if (!string.IsNullOrEmpty(problem.Property))
        {
            builder.Append($"Property: {problem.Property}, ");
        }

        if (!string.IsNullOrEmpty(problem.TypeId))
        {
            builder.Append($"TypeId: {problem.TypeId}, ");
        }

        if (problem.Extensions != null && problem.Extensions.Count > 0)
        {
            builder.Append("Extensions: { ");
            foreach (var kvp in problem.Extensions)
            {
                builder.Append($"{kvp.Key}: {kvp.Value}, ");
            }
            // Remove a última vírgula e espaço
            builder.Length -= 2;
            builder.Append(" }");
        }
        else
        {
            // Remove a última vírgula e espaço se não houver extensões
            builder.Length -= 2;
        }

        return builder.ToString();
    }
}