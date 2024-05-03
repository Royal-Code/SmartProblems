using RoyalCode.SmartProblems;

namespace FluentValidation;

/// <summary>
/// <para>
///     A readonly struct to add extra data to the validation result.
/// </para>
/// <para>
///     The data is stored in a dictionary and will be available in the <see cref="Problem.Extensions"/> property.
/// </para>
/// </summary>
/// <typeparam name="TModel">The type of the model to validate.</typeparam>
/// <typeparam name="TProperty">The type of the property to validate.</typeparam>
public readonly struct ExtensionData<TModel, TProperty>
{
    private readonly Dictionary<string, object?> data;

    /// <summary>
    /// Create a new instance of <see cref="ExtensionData"/>.
    /// </summary>
    /// <param name="data">The dictionary to add extra data.</param>
    /// <param name="model">The validated model.</param>
    /// <param name="property">The validated property value.</param>
    public ExtensionData(Dictionary<string, object?> data, TModel model, TProperty property)
    {
        this.data = data;
        Model = model;
        Property = property;
    }

    /// <summary>
    /// Validated model.
    /// </summary>
    public TModel Model { get; }

    /// <summary>
    /// Validated property value.
    /// </summary>
    public TProperty Property { get; }

    /// <summary>
    /// Add extra data to the validation result.
    /// </summary>
    /// <param name="key">The key of the extra data.</param>
    /// <param name="value">The value of the extra data.</param>
    /// <returns>
    ///     The same instance of <see cref="ExtensionData"/> for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Case the <paramref name="key"/> is null.
    /// </exception>
    /// <exception cref="AggregateException">
    ///     Case the <paramref name="key"/> already exists in the dictionary.
    /// </exception>
    public ExtensionData<TModel, TProperty> Add(string key, object? value)
    {
        data.Add(key, value);
        return this;
    }
}