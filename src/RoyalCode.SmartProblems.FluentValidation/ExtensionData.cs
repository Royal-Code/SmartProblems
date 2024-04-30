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
public readonly struct ExtensionData
{
    private readonly Dictionary<string, object?> data;

    /// <summary>
    /// Create a new instance of <see cref="ExtensionData"/>.
    /// </summary>
    /// <param name="data">The dictionary to add extra data.</param>
    public ExtensionData(Dictionary<string, object?> data)
    {
        this.data = data;
    }

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
    public ExtensionData Add(string key, object? value)
    {
        data.Add(key, value);
        return this;
    }
}