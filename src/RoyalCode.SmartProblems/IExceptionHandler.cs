using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.SmartProblems;

/// <summary>
/// <para>
///     Handler for exceptions that can be converted to problems.
/// </para>
/// <para>
///     Used by the <see cref="Problems"/> class to convert exceptions to problems 
///     on the <see cref="Problems.InternalError(Exception)"/> method.
/// </para>
/// </summary>
public interface IExceptionHandler
{
    /// <summary>
    /// Try to create a problem from the exception.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    /// <param name="problem">The problem created from the exception.</param>
    /// <returns>True when the exception was converted to a problem, false if can't convert.</returns>
    bool TryHandle(Exception exception, [NotNullWhen(true)] out Problem? problem);
}
