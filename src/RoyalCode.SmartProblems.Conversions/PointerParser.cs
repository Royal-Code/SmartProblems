﻿
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.SmartProblems.Conversions;

/// <summary>
/// Static methods to parse JSON pointers to C# properties and vice versa.
/// </summary>
public static class PointerParser
{
    /// <summary>
    /// Gets or sets the parser used to convert JSON pointers to C# properties and vice versa.
    /// </summary>
    public static IPointerParser Parser { get; set; } = new DefaultPointerParser();

    /// <summary>
    /// <para>
    ///     Convert a JSON pointer to a C# property.
    /// </para>
    /// <para>
    ///     Some examples:
    /// </para>
    /// <list type="table">
    ///     <item>
    ///         <c>#/foo/0/bar</c> = <c>foo[0].bar</c>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="pointer">The JSON pointer to convert.</param>
    /// <returns>The C# property converted.</returns>
    public static string? PointerToProperty([NotNullIfNotNull(nameof(pointer))] this string? pointer)
        => Parser.PointerToProperty(pointer);

    /// <summary>
    /// <para>
    ///     Convert a C# property to a JSON pointer.
    /// </para>
    /// <para>
    ///     Some examples:
    /// </para>
    /// <list type="table">
    ///     <item>
    ///         <c>foo[0].bar</c> = <c>#/foo/0/bar</c>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="property">The C# property to convert.</param>
    /// <returns>The JSON pointer converted.</returns>
    public static string? PropertyToPointer(this string? property)
        => Parser.PropertyToPointer(property);
}
