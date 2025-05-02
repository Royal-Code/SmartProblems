using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// <para>
///     A class that provides a way to get display names for types and properties.
/// </para>
/// <para>
///     The names are cached for performance.
/// </para>
/// </summary>
public class DisplayNames
{
    /// <summary>
    /// The instance of the DisplayNames class.
    /// </summary>
    public static DisplayNames Instance { get; } = new();

    private readonly ConcurrentDictionary<Type, string> typeNames = new();
    private readonly ConcurrentDictionary<(Type, string), string> propertyNames = new();
    private readonly Func<Type, string> createTypeDisplayName;
    private readonly Func<(Type, string), string> createPropertyDisplayName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayNames"/> class.
    /// </summary>
    public DisplayNames()
    {
        createTypeDisplayName = CreateTypeDisplayName;
        createPropertyDisplayName = CreatePropertyDisplayName;
    }

    /// <summary>
    /// Gets the display name of a given type.
    /// </summary>
    /// <param name="type">The type to get the display name for.</param>
    /// <returns>The DisplayName, if available, or the name of the type. When null, returns “?”.</returns>
    public string GetDisplayName(Type? type)
    {
        if (type is null)
            return "?";

        return typeNames.GetOrAdd(type, createTypeDisplayName);
    }

    /// <summary>
    /// Gets the display name of a given property of a given type.
    /// </summary>
    /// <param name="type">The type that should contain the property.</param>
    /// <param name="property">The name of the property.</param>
    /// <returns>
    ///     The DisplayName, if available, or the name of the property.
    ///     When the type is null, the name of the property will be returned, 
    ///     and if the property is null, it returns “?”.
    /// </returns>
    public string GetDisplayName(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]Type? type,
        string? property)
    {
        if (type is null || property is null)
            return property ?? "?";

        return propertyNames.GetOrAdd((type, property), createPropertyDisplayName);
    }

    private static string CreateTypeDisplayName(Type type)
    {
        var attr = type.GetCustomAttribute<DisplayNameAttribute>();
        if (attr is not null)
            return attr.DisplayName;

        return type.Name;
    }

    [SuppressMessage(
        "Trimming", 
        "IL2077:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The source field does not have matching annotations.", 
        Justification = "Annotation are added in the caller.")]
    private static string CreatePropertyDisplayName((Type type, string property) key)
        => CreatePropertyDisplayName(key.type, key.property);

    private static string CreatePropertyDisplayName(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type,
        string property)
    {
        var propertyInfo = type.GetRuntimeProperty(property);
        if (propertyInfo == null)
            return property;

        var attr = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
        if (attr is not null)
            return attr.DisplayName;

        return property;
    }
}