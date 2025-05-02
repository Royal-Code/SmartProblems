using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems;

/// <summary>
/// A structure that represents the details of a problem.
/// </summary>
public readonly ref struct DetailBuilder
{
    /// <summary>
    /// Description of the problem.
    /// </summary>
    public required string Details { get; init; }

    /// <summary>
    /// Name of the property that caused the problem.
    /// </summary>
    public string? Property { get; init; }

    /// <summary>
    /// <para>
    ///     Identifier of the type of problem.
    /// </para>
    /// <para>
    ///     It can be used for any category of problem, 
    ///     but it is especially useful for validation problems and business rules.
    /// </para>
    /// <para>
    ///     It is directly related to the type of problem details.
    /// </para>
    /// </summary>
    public string? TypeId { get; init; }

    /// <summary>
    /// Additional data to identify and describe the problem.
    /// </summary>
    public IDictionary<string, object?>? Extensions { get; init; }

    /// <summary>
    /// Adds the value for the property, which will be converted to the ProblemDetails pointer.
    /// </summary>
    /// <param name="property">Name of the property that caused the problem.</param>
    /// <returns>A copy of the modified struct.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DetailBuilder WithProperty(string property)
    {
        return new DetailBuilder()
        {
            Details = Details,
            Property = property,
            TypeId = TypeId,
            Extensions = Extensions
        };
    }

    /// <summary>
    /// Adds the id of the ProblemDetails type.
    /// </summary>
    /// <param name="typeId">ID of the problem type.</param>
    /// <returns>A copy of the modified struct.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DetailBuilder WithTypeId(string typeId)
    {
        return new DetailBuilder()
        {
            Details = Details,
            TypeId = typeId,
            Property = Property,
            Extensions = Extensions,
        };
    }

    /// <summary>
    /// Adds an extra piece of information to the problem.
    /// </summary>
    /// <param name="key">Key, name of the data.</param>
    /// <param name="value">The data.</param>
    /// <returns>A copy of the modified struct.</returns>
    public DetailBuilder With(string key, object? value)
    {
        var extensions = Extensions ?? new Dictionary<string, object?>(StringComparer.Ordinal);
        extensions[key] = value;
        return new DetailBuilder()
        {
            Details = Details,
            TypeId = TypeId,
            Property = Property,
            Extensions = extensions,
        };
    }

    /// <summary>
    /// Adds an extra piece of information to the problem.
    /// </summary>
    /// <param name="key">Key, name of the data.</param>
    /// <param name="value">The data.</param>
    /// <returns>A copy of the modified struct.</returns>
    public DetailBuilder With<TEnum>(string key, TEnum value)
        where TEnum : Enum
    {
        var extensions = Extensions ?? new Dictionary<string, object?>(StringComparer.Ordinal);
        extensions[key] = value.ToString();
        return new DetailBuilder()
        {
            Details = Details,
            TypeId = TypeId,
            Property = Property,
            Extensions = extensions,
        };
    }

    /// <summary>
    /// Create a NotFound issue with the details of this builder.
    /// </summary>
    /// <returns>A new instance of the problem.</returns>
    public Problem NotFound() => Build(ProblemCategory.NotFound);

    /// <summary>
    /// Creates a problem from the InvalidParameter category with the details of this builder.
    /// </summary>
    /// <returns>A new instance of the problem.</returns>
    public Problem InvalidParameter() => Build(ProblemCategory.InvalidParameter);

    /// <summary>
    /// Create an issue from the ValidationFailed category with the details of this builder.
    /// </summary>
    /// <returns>A new instance of the problem.</returns>
    public Problem ValidationFailed() => Build(ProblemCategory.ValidationFailed);

    /// <summary>
    /// Create a problem from the InvalidState category with the details of this builder.
    /// </summary>
    /// <returns>A new instance of the problem.</returns>
    public Problem InvalidState() => Build(ProblemCategory.InvalidState);

    /// <summary>
    /// Create a NotAllowed issue with the details of this builder.
    /// </summary>
    /// <returns>A new instance of the problem.</returns>
    public Problem NotAllowed() => Build(ProblemCategory.NotAllowed);

    /// <summary>
    /// Create a problem from the BussinessRuleViolation category with the details of this builder.
    /// </summary>
    /// <returns>A new instance of the problem.</returns>
    public Problem Custom() => Build(ProblemCategory.CustomProblem);

    /// <summary>
    /// Create the problem instance.
    /// </summary>
    /// <param name="category">Problem category.</param>
    /// <returns>A new instance of the problem.</returns>
    public Problem Build(ProblemCategory category)
    {
        return new Problem()
        {
            Category = category,
            Detail = Details,
            Property = Property,
            TypeId = TypeId,
            Extensions = Extensions
        };
    }
}
