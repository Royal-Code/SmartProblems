using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RoyalCode.SmartProblems.Descriptions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems.Convertions;

/// <summary>
/// Builder for <see cref="ProblemDetails"/> used by <see cref="ProblemDetailsConverter"/>.
/// </summary>
public class ProblemDetailsBuilder
{
    private List<InvalidParameterDetails>? invalidParameterErrors;
    private List<NotFoundDetails>? notFoundErrors;
    private List<ErrorDetails>? errors;
    private List<CustomDetails>? customErrors;

    private Dictionary<string, object>? extensions;
    private bool withNotAllowedErrors = false;
    private bool withValidationErrors = false;
    private bool withInvalidStateErrors = false;
    private bool withInternalErrors = false;

    public ProblemDetails Build(ProblemDetailsOptions options)
    {
        ProblemDetailsDescription description;
        if (IsGenericProblem(out var category))
        {
            description = options.Descriptor.GetDescriptionByCategory(category.Value);
        }
        else
        {
            var typeId = GetTypeId();
            if (options.Descriptor.TryGetDescription(typeId, out var codeDescription))
                description = codeDescription;
            else
                description = options.Descriptor.GetDescriptionByCategory(GetCategory());
        }

        var type = description.Type
            ?? $"{options.BaseAddress}{options.TypeComplement}{description.TypeId}";

        int status = customErrors?.Count > 1
            ? GetAggregateStatus(options)
            : (int)description.Status;

        var problem = new ProblemDetails
        {
            Type = type,
            Title = description.Title,
            Status = status,
            Detail = GetDetail(),
        };

        WriteExtensions(problem, options);

        return problem;
    }

    /// <summary>
    /// It checks if there are only generic problems and, if so, returns the most severe problem category.
    /// </summary>
    /// <param name="category">The problem category.</param>
    /// <returns>If there are only generic problems.</returns>
    private bool IsGenericProblem([NotNullWhen(true)] out ProblemCategory? category)
    {
        if (customErrors is not null && customErrors.Count > 0)
        {
            category = null;
            return false;
        }

        if (errors is not null)
        {
            if (withInternalErrors)
                category = ProblemCategory.InternalServerError;
            else if (withInvalidStateErrors)
                category = ProblemCategory.InvalidState;
            else if (withValidationErrors)
                category = ProblemCategory.ValidationFailed;
            else if (withNotAllowedErrors)
                category = ProblemCategory.NotAllowed;
            else
                category = ProblemCategory.InvalidParameter;

            return true;
        }

        if (invalidParameterErrors is not null)
        {
            category = ProblemCategory.InvalidParameter;
            return true;
        }
        if (notFoundErrors is not null)
        {
            category = ProblemCategory.NotFound;
            return true;
        }

        category = null;
        return false;
    }

    /// <summary>
    /// Get the most appropriate TypeId for the problem.
    /// </summary>
    /// <returns>A string that represents the TypeId.</returns>
    private string GetTypeId()
    {
        if (customErrors is not null)
        {
            if (customErrors.Count == 1)
                return customErrors[0].TypeId;

            return ProblemDetailsDescriptor.GenericProblemsTypeIds.AggregateProblemsDetails;
        }

        return ProblemDetailsDescriptor.Types.AboutBlank;
    }

    /// <summary>
    /// Obtains the most appropriate status for the problem when there is more than one customised problem.
    /// </summary>
    /// <param name="options">The options for the problem details conversion.</param>
    /// <returns>The HTTP status code.</returns>
    private int GetAggregateStatus(ProblemDetailsOptions options)
    {
        if (customErrors is not null)
        {
            var custom = customErrors[0];
            var description = GetDescription(options, custom.TypeId, GetCategory());
            return (int)description.Status;
        }

        return 400;
    }

    /// <summary>
    /// Get the appropriate category for the problem when there is no TypeId.
    /// </summary>
    /// <returns>The problem category.</returns>
    private ProblemCategory GetCategory()
    {
        return customErrors switch
        {
            not null => ProblemCategory.CustomProblem,
            _ => ProblemCategory.InvalidState,
        };
    }

    /// <summary>
    /// Gets the value for the field <see cref="ProblemDetails.Detail"/>.
    /// </summary>
    /// <returns>A string that describes the error</returns>
    private string GetDetail()
    {
        if (customErrors is not null)
            return customErrors.Count > 1
                ? ProblemDetailsDescriptor.Messages.AggregateMessage
                : customErrors[0].Detail ?? string.Empty;

        if (errors is not null)
        {
            if (withInternalErrors)
                return ProblemDetailsDescriptor.Messages.InternalErrorMessage;

            if (withInvalidStateErrors)
                return ProblemDetailsDescriptor.Messages.InvalidStateMessage;

            if (withValidationErrors)
                return ProblemDetailsDescriptor.Messages.ValidationFailedMessage;

            if (withNotAllowedErrors)
                return ProblemDetailsDescriptor.Messages.NotAllowedMessage;
        }

        if (invalidParameterErrors is not null)
            return ProblemDetailsDescriptor.Messages.InvalidParametersMessage;

        if (notFoundErrors is not null)
            return ProblemDetailsDescriptor.Messages.NotFoundMessage;

        return ProblemDetailsDescriptor.Messages.DefaultMessage;
    }

    /// <summary>
    /// Add the extensions fields and values to the <see cref="ProblemDetails"/>.
    /// </summary>
    /// <param name="problemDetails">The problem details to be modified.</param>
    /// <param name="options">The options for the problem details conversion.</param>
    private void WriteExtensions(
        ProblemDetails problemDetails,
        ProblemDetailsOptions options)
    {
        var pdext = problemDetails.Extensions;

        if (customErrors is not null)
        {
            if (customErrors.Count > 1)
            {
                pdext[ProblemDetailsExtended.Fields.AggregateExtensionField] = customErrors
                    .Select(p => ToProblemDetails(p, options))
                    .ToList();
            }
            else if (customErrors[0].Extensions is not null)
            {
                foreach (var (key, value) in customErrors[0].Extensions!)
                    pdext[key] = value;
            }
        }
        if (invalidParameterErrors is not null)
            pdext[ProblemDetailsExtended.Fields.InvalidParametersExtensionField] = invalidParameterErrors;

        if (notFoundErrors is not null)
            pdext[ProblemDetailsExtended.Fields.NotFoundExtensionField] = notFoundErrors;

        if (errors is not null)
        {
            // check if there are more than one type of error
            // the add status code based on the category
            AddStatusWhenHasManyCategories(options, errors);

            pdext[ProblemDetailsExtended.Fields.ErrorsExtensionField] = errors;
        }
        if (extensions is not null)
            foreach (var (key, value) in extensions)
                pdext[key] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddStatusWhenHasManyCategories(ProblemDetailsOptions options, IEnumerable<ErrorDetails> errorsDetails)
    {
        int count = 0;
        if (withInternalErrors)
            count++;
        if (withInvalidStateErrors)
            count++;
        if (withNotAllowedErrors)
            count++;
        if (withValidationErrors)
            count++;

        if (count < 2)
            return;

        foreach (var details in errorsDetails)
        {
            var description = options.Descriptor.GetDescriptionByCategory(details.Category);
            details.With("status", (int)description.Status);
        }
    }

    private ProblemDetails ToProblemDetails(CustomDetails problem, ProblemDetailsOptions options)
    {
        return CreateProblemDetails(options,
            problem.TypeId, GetCategory(), problem.Detail, problem.Extensions);
    }

    private static ProblemDetailsDescription GetDescription(ProblemDetailsOptions options,
        string? typeId, ProblemCategory category)
    {
        ProblemDetailsDescription description;

        if (typeId is null)
        {
            description = options.Descriptor.GetDescriptionByCategory(category);
        }
        else
        {
            description = options.Descriptor.TryGetDescription(typeId, out var desc)
                ? desc
                : options.Descriptor.GetDescriptionByCategory(category);
        }

        return description;
    }

    public static ProblemDetails CreateProblemDetails(ProblemDetailsOptions options,
        string? typeId, ProblemCategory problemCategory,
        string details, IDictionary<string, object?>? extensions = null)
    {
        ProblemDetailsDescription description = GetDescription(options, typeId, problemCategory);

        var type = description.Type
            ?? $"{options.BaseAddress}{options.TypeComplement}{description.TypeId}";

        ProblemDetails problemDetails = new()
        {
            Type = type,
            Title = description.Title,
            Detail = details,
            Status = (int)description.Status,
        };

        if (extensions is not null)
            foreach (var (key, value) in extensions)
                problemDetails.Extensions.Add(key, value);

        return problemDetails;
    }

    /// <summary>
    /// Add a not found error to the problem details.
    /// </summary>
    /// <param name="notFoundDetails">The not found details.</param>
    public void AddNotFound(NotFoundDetails notFoundDetails)
    {
        notFoundErrors ??= [];
        notFoundErrors.Add(notFoundDetails);
    }

    /// <summary>
    /// Add a invalid parameter error to the problem details.
    /// </summary>
    /// <param name="details">The invalid parameter details.</param>
    /// <param name="validationError">If the error is a validation error.</param>
    public void AddInvalidParameter(InvalidParameterDetails details)
    {
        invalidParameterErrors ??= [];
        invalidParameterErrors.Add(details);
    }

    /// <summary>
    /// Add a validation error to the problem details.
    /// </summary>
    /// <param name="details">The validation error details.</param>
    public void AddValidationFailed(ErrorDetails details)
    {
        errors ??= [];
        errors.Add(details);
        withValidationErrors = true;
    }

    /// <summary>
    /// Add an error to the problem details.
    /// </summary>
    /// <param name="errorDetails">The error details.</param>
    /// <param name="invalidState">If the error is a invalidState.</param>
    public void AddError(ErrorDetails errorDetails, bool invalidState)
    {
        errors ??= [];
        errors.Add(errorDetails);
        if (invalidState)
            withInvalidStateErrors = true;
    }

    /// <summary>
    /// Add a not allowed error to the problem details.
    /// </summary>
    /// <param name="errorDetails">The error details.</param>
    public void AddNotAllowed(ErrorDetails errorDetails)
    {
        errors ??= [];
        errors.Add(errorDetails);
        withNotAllowedErrors = true;
    }

    /// <summary>
    /// Add an internal error to the problem details.
    /// </summary>
    /// <param name="errorDetails">The error details.</param>
    public void AddInternalError(ErrorDetails errorDetails)
    {
        errors ??= [];
        errors.Add(errorDetails);
        withInternalErrors = true;
    }

    /// <summary>
    /// <para>
    ///     Add a custom problem to the problem details.
    /// </para>
    /// <para>
    ///     A custom problem is a message with a specific Code, and will have a specific Type.
    /// </para>
    /// </summary>
    /// <param name="problem">The custom problem message.</param>
    public void AddCustomProblem(CustomDetails problem)
    {
        customErrors ??= [];
        customErrors.Add(problem);
    }

    /// <summary>
    /// Additional information to be added to the problem details.
    /// </summary>
    /// <param name="additionalInformation">Additional information, in the form of key-value pairs.</param>
    public void AddExtension(IEnumerable<KeyValuePair<string, object>> additionalInformation)
    {
        extensions ??= [];

        foreach (var kvp in additionalInformation)
        {
            extensions[kvp.Key] = kvp.Value;
        }
    }
}
