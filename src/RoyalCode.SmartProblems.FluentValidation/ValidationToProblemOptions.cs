
using RoyalCode.SmartProblems;

namespace FluentValidation;

public sealed class ValidationToProblemOptions
{
    /// <summary>
    /// The <see cref="ProblemCategory"/> used to convert FluentValidation errors to <see cref="Problem"/>.
    /// </summary>
    public ProblemCategory Category { get; set; } = ProblemCategory.InvalidParameter;

    /// <summary>
    /// <para>
    ///     Determines if the error code should be included in the <see cref="Problem.Extensions"/>.
    /// </para>
    /// <para>
    ///     The default value is <c>true</c>.
    /// </para>
    /// </summary>
    public bool IncludeErrorCode { get; set; } = true;

    /// <summary>
    /// <para>
    ///     Determines the key used to store the error code in the <see cref="Problem.Extensions"/>.
    /// </para>
    /// <para>
    ///     The default value is <c>"error_code"</c>.
    /// </para>
    /// </summary>
    public string ErrorCodeExtensionField { get; set; } = "error_code";
}
