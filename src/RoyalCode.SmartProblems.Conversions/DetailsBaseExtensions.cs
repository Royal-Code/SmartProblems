using System.Text.Json;

namespace RoyalCode.SmartProblems.Conversions;

/// <summary>
/// Extension methods for <see cref="DetailsBase"/>.
/// </summary>
public static class DetailsBaseExtensions
{
    private static JsonSerializerOptions? serializationOptions;
    internal static JsonSerializerOptions SerializerOptions => serializationOptions ??= new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Adds a key-value pair to the extensions dictionary.
    /// </summary>
    /// <param name="details">The instance of <see cref="DetailsBase"/>.</param>
    /// <param name="key">The key of the extension.</param>
    /// <param name="value">The value of the extension.</param>
    /// <returns>The same instance of <see cref="ErrorDetails"/>.</returns>
    public static TDetails With<TDetails>(this TDetails details, string key, object value)
        where TDetails : DetailsBase
    {
        details.Extensions ??= new Dictionary<string, object?>(StringComparer.Ordinal);
        details.Extensions.Add(key, value);
        return details;
    }
}
