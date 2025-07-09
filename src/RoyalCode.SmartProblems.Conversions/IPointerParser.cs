
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.SmartProblems.Conversions;

/// <summary>
/// Interface to parse JSON pointers to C# properties and vice versa.
/// </summary>
public interface IPointerParser
{
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
    string? PointerToProperty([NotNullIfNotNull(nameof(pointer))] string? pointer);

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
    string? PropertyToPointer(string? property);
}
