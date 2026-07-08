
using RoyalCode.SmartProblems;

namespace FluentValidation;

/// <summary>
/// Options to convert FluentValidation errors to <see cref="Problem"/>.
/// </summary>
public sealed class ValidationToProblemOptions
{
    private string? fallbackProblemDetail;

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

    /// <summary>
    /// <para>
    ///     Detail used when a <see cref="FluentValidation.Results.ValidationResult"/> is invalid,
    ///     but no <see cref="Problem"/> is produced after filtering validation failures.
    /// </para>
    /// <para>
    ///     This can happen, for example, when the validation result contains only warning or info failures.
    /// </para>
    /// </summary>
    public string FallbackProblemDetail
    {
        get => fallbackProblemDetail ?? R.FallbackProblemDetail;
        set => fallbackProblemDetail = value ?? throw new ArgumentNullException(nameof(value));
    }
}
