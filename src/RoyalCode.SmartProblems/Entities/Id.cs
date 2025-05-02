using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// Represents an identifier for an entity.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TId">The type of identifier.</typeparam>
public readonly struct Id<TEntity, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TId>
    where TEntity : class
{
    /// <summary>
    /// Implicit operator to convert a value of type <typeparamref name="TId"/> for an <see cref="Id{TEntity, TId}"/>.
    /// </summary>
    /// <param name="id">The identifier value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Id<TEntity, TId>(TId id) => new(id);

    /// <summary>
    /// Creates a new <see cref="Id{TEntity, TId}"/> with the specified identifier value.
    /// </summary>
    /// <param name="value">The identifier value.</param>
    public Id(TId value) => Value = value;

    /// <summary>
    /// The identifier value.
    /// </summary>
    public TId Value { get; }

    /// <summary>
    /// Tries to convert a string to an <see cref="Id{TEntity, TId}"/>.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <param name="id">The resulting <see cref="Id{TEntity, TId}"/>.</param>
    /// <returns>True if the conversion was successful; otherwise, false.</returns>
    [RequiresUnreferencedCode("Requires Unreferenced Code of TId")]
    public static bool TryParse(string? input, out Id<TEntity, TId> id)
    {
        if (input == null)
        {
            id = default;
            return true;
        }

        try
        {
            var type = typeof(TId);
            var converter = TypeDescriptor.GetConverter(type);
            if (converter is not null && converter.CanConvertFrom(typeof(string)))
            {
                id = new Id<TEntity, TId>((TId)converter.ConvertFromString(input)!);
                return true;
            }
        }
        catch { /* Ignorar exceções, retorno false no final. */ }

        id = default;
        return false;
    }
}
