using FluentValidation.Results;
using RoyalCode.SmartProblems;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FluentValidation;

/// <summary>
/// Extensions methods to convert FluentValidation errors to <see cref="Result"/> and other operations.
/// </summary>
public static class ValidationsExtensions
{
    /// <summary>
    /// The options to convert FluentValidation errors to <see cref="Problem"/>.
    /// </summary>
    public static ValidationToProblemOptions Options { get; } = new();

    /// <summary>
    /// Convert a list of <see cref="ValidationFailure"/> from FluentValidations to <see cref="Problems"/>.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Problems ToProblems(this IList<ValidationFailure> errors, ValidationToProblemOptions options)
    {
        var problems = new Problems();

        foreach (var error in errors)
        {
            switch (error.Severity)
            {
                case Severity.Warning:
                case Severity.Info:
                    break;
                case Severity.Error:

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
                default:
                    throw new NotSupportedException("Invalid value for Severity of FluentValidation.ValidationFailure.");
            }
        }

        return problems;
    }

    /// <summary>
    /// Convert a <see cref="ValidationResult"/> from FluentValidations to <see cref="Result"/>.
    /// </summary>
    /// <param name="result">The <see cref="ValidationResult"/> to convert.</param>
    /// <returns>The <see cref="Result"/> converted.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasProblems<T>(this AbstractValidator<T> validator, T model, [NotNullWhen(true)] out Problems? problems)
    {
        return validator.Validate(model).ToResult().HasProblems(out problems);
    }
    
    /// <summary>
    /// <para>
    ///     Execute the validation of a model and convert to a result.
    /// </para>
    /// <para>
    ///     When the model is valid, it is returned, when it is invalid, the problems are returned.
    /// </para>
    /// </summary>
    /// <param name="validator">The model validator.</param>
    /// <param name="model">The model instance to validate.</param>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <returns>
    ///     The result of the validation, with the model validated when it is valid, or the problems.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> EnsureIsValid<T>(this AbstractValidator<T> validator, T model)
    {
        return validator.Validate(model).ToResult().Map(model);
    }

    /// <summary>
    /// <para>
    ///     Async execute the validation of a model and convert to a result.
    /// </para>
    /// <para>
    ///     When the model is valid, it is returned, when it is invalid, the problems are returned.
    /// </para>
    /// </summary>
    /// <param name="validator">The model validator.</param>
    /// <param name="model">The model instance to validate.</param>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <returns>
    ///     The result of the validation, with the model validated when it is valid, or the problems.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T>> EnsureIsValidAsync<T>(this AbstractValidator<T> validator, T model)
    {
        return validator.ValidateAsync(model).ContinueWith((t, o) => t.Result.ToResult().Map((T)o!), model);
    }

    /// <summary>
    /// <para>
    ///     Execute the validation of a model and convert to a result.
    /// </para>
    /// <para>
    ///     When the model is valid, it is returned, when it is invalid, the problems are returned.
    /// </para>
    /// <para>
    ///     When the input result is invalid, it is returned without validation.
    /// </para>
    /// </summary>
    /// <param name="result">The input result to validate the model.</param>
    /// <param name="validator">The model validator.</param>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <returns>
    ///     The result of the validation, with the model validated when it is valid, or the problems.
    /// </returns>
    public static Result<T> Validate<T>(this Result<T> result, AbstractValidator<T> validator)
    {
        return result.HasValue(out var model)
            ? validator.EnsureIsValid(model)
            : result;
    }
    
    /// <summary>
    /// <para>
    ///     Execute the validation of a model and convert to a result.
    /// </para>
    /// <para>
    ///     When the model is valid, it is returned, when it is invalid, the problems are returned.
    /// </para>
    /// <para>
    ///     When the input result is invalid, it is returned without validation.
    /// </para>
    /// </summary>
    /// <param name="task">A task with the input result to validate the model.</param>
    /// <param name="validator">The model validator.</param>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <returns>
    ///     The result of the validation, with the model validated when it is valid, or the problems.
    /// </returns>
    public static async Task<Result<T>> Validate<T>(this Task<Result<T>> task, AbstractValidator<T> validator)
    {
        var result = await task;
        return result.Validate(validator);
    }
    
    /// <summary>
    /// <para>
    ///     Execute the validation of a model and convert to a result.
    /// </para>
    /// <para>
    ///     When the model is valid, it is returned, when it is invalid, the problems are returned.
    /// </para>
    /// <para>
    ///     When the input result is invalid, it is returned without validation.
    /// </para>
    /// </summary>
    /// <param name="task">A task with the input result to validate the model.</param>
    /// <param name="validator">The model validator.</param>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <returns>
    ///     The result of the validation, with the model validated when it is valid, or the problems.
    /// </returns>
    public static async Task<Result<T>> Validate<T>(this ValueTask<Result<T>> task, AbstractValidator<T> validator)
    {
        var result = await task;
        return result.Validate(validator);
    }
    
    /// <summary>
    /// <para>
    ///     Execute the validation of a model and convert to a result.
    /// </para>
    /// <para>
    ///     When the model is valid, it is returned, when it is invalid, the problems are returned.
    /// </para>
    /// <para>
    ///     When the input result is invalid, it is returned without validation.
    /// </para>
    /// </summary>
    /// <param name="result">The input result to validate the model.</param>
    /// <param name="validator">The model validator.</param>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <returns>
    ///     The result of the validation, with the model validated when it is valid, or the problems.
    /// </returns>
    public static async Task<Result<T>> ValidateAsync<T>(this Result<T> result, AbstractValidator<T> validator)
    {
        return result.HasValue(out var model)
            ? await validator.EnsureIsValidAsync(model)
            : result;
    }
    
    /// <summary>
    /// <para>
    ///     Execute the validation of a model and convert to a result.
    /// </para>
    /// <para>
    ///     When the model is valid, it is returned, when it is invalid, the problems are returned.
    /// </para>
    /// <para>
    ///     When the input result is invalid, it is returned without validation.
    /// </para>
    /// </summary>
    /// <param name="task">A task with the input result to validate the model.</param>
    /// <param name="validator">The model validator.</param>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <returns>
    ///     The result of the validation, with the model validated when it is valid, or the problems.
    /// </returns>
    public static async Task<Result<T>> ValidateAsync<T>(this Task<Result<T>> task, AbstractValidator<T> validator)
    {
        var result = await task;
        return await result.ValidateAsync(validator);
    }
    
    /// <summary>
    /// <para>
    ///     Execute the validation of a model and convert to a result.
    /// </para>
    /// <para>
    ///     When the model is valid, it is returned, when it is invalid, the problems are returned.
    /// </para>
    /// <para>
    ///     When the input result is invalid, it is returned without validation.
    /// </para>
    /// </summary>
    /// <param name="task">A task with the input result to validate the model.</param>
    /// <param name="validator">The model validator.</param>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    /// <returns>
    ///     The result of the validation, with the model validated when it is valid, or the problems.
    /// </returns>
    public static async Task<Result<T>> ValidateAsync<T>(this ValueTask<Result<T>> task, AbstractValidator<T> validator)
    {
        var result = await task;
        return await result.ValidateAsync(validator);
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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