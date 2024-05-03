using FluentValidation.Results;
using RoyalCode.SmartProblems;
using System.Diagnostics.CodeAnalysis;

namespace FluentValidation;

/// <summary>
/// Extensions methods to converver FluentValidation errors to <see cref="Result"/> and other operations.
/// </summary>
public static class ValidationsExtensions
{
    /// <summary>
    /// The options to convert FluentValidation errors to <see cref="Problem"/>.
    /// </summary>
    public static ValidationToProblemOptions Options { get; } = new();

    /// <summary>
    /// Convert a list fo <see cref="ValidationFailure"/> from FluentValidations to <see cref="Problems"/>.
    /// </summary>
    /// <param name="errors">A list of <see cref="ValidationFailure"/>.</param>
    /// <returns>The <see cref="Problems"/> converted.</returns>
    public static Problems ToProblems(this IList<ValidationFailure> errors)
        => ToProblems(errors, Options);

    /// <summary>
    /// Convert a list fo <see cref="ValidationFailure"/> from FluentValidations to <see cref="Problems"/>.
    /// </summary>
    /// <param name="errors">A list of <see cref="ValidationFailure"/>.</param>
    /// <param name="options">The options to convert FluentValidation errors to <see cref="Problem"/>.</param>
    /// <returns>The <see cref="Problems"/> converted.</returns>
    public static Problems ToProblems(this IList<ValidationFailure> errors, ValidationToProblemOptions options)
    {
        var problems = new Problems();

        for (var i = 0; i < errors.Count; i++)
        {
            var error = errors[i];
            switch (error.Severity)
            {
                case Severity.Warning:
                case Severity.Info:
                    break;
                default:

                    var problem = new Problem()
                    {
                        Category = options.Category,
                        Detail = error.ErrorMessage,
                        Property = error.PropertyName,
                    };

                    if (error.CustomState is Dictionary<string, object?> extensions)
                    {
                        problem.Extensions = extensions;
                    }

                    if (options.IncludeErrorCode)
                        problem.With(options.ErrorCodeExtensionField, error.ErrorCode);

                    problems.Add(problem);

                    break;
            }
        }

        return problems;
    }

    /// <summary>
    /// Convert a <see cref="ValidationResult"/> from FluentValidations to <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The <see cref="ValidationResult"/> to convert.</param>
    /// <returns>The <see cref="Result"/> converted.</returns>
    public static Result ToResult(this ValidationResult result)
    {
        return result.IsValid
            ? Result.Ok()
            : result.Errors.ToProblems();
    }

    /// <summary>
    /// Check if a <see cref="ValidationResult"/> has problems.
    /// </summary>
    /// <param name="result">The <see cref="ValidationResult"/> from FluentValidation to check.</param>
    /// <param name="problems">The <see cref="Problems"/> if the result has problems.</param>
    /// <returns>
    ///     True if the <see cref="ValidationResult"/> has is invalid and has problems, otherwise false.
    /// </returns>
    public static bool HasProblems(this ValidationResult result, [NotNullWhen(true)] out Problems? problems)
    {
        return result.ToResult().HasProblems(out problems);
    }

    /// <summary>
    /// Execute the validation of a model and check if it has problems.
    /// </summary>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <param name="validator">The model validator.</param>
    /// <param name="model">The model instance to validate.</param>
    /// <param name="problems">The <see cref="Problems"/> if the model is invalid.</param>
    /// <returns>
    ///     True if the model is invalid and has problems, otherwise false.
    /// </returns>
    public static bool HasProblems<T>(this AbstractValidator<T> validator, T model, [NotNullWhen(true)] out Problems? problems)
    {
        return validator.Validate(model).ToResult().HasProblems(out problems);
    }

    /// <summary>
    /// <para>
    ///     Adds extra data to the validation result that will be used to generate problems.
    /// </para>
    /// <para>
    ///     This extra data will be available in the <see cref="Problem.Extensions"/> property.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <typeparam name="TProperty">The type of the property to validate.</typeparam>
    /// <param name="rule">The rule builder options.</param>
    /// <param name="action">The action to add extra data to the validation result.</param>
    /// <returns>
    ///     The same instance of <paramref name="rule"/> for chaining.
    /// </returns>
    public static IRuleBuilderOptions<T, TProperty> WithExtension<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Action<ExtensionData<T, TProperty>> action)
    {
        var component = DefaultValidatorOptions.Configurable(rule).Current;
        component.CustomStateProvider = (ctx, prop) =>
        {
            var extensions = new Dictionary<string, object?>(StringComparer.Ordinal);
            action(new ExtensionData<T, TProperty>(extensions, ctx.InstanceToValidate, prop));
            return extensions;
        };

        return rule;
    }
}