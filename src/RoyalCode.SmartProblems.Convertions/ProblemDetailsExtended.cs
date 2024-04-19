using RoyalCode.SmartProblems.Convertions.Internals;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoyalCode.SmartProblems.Convertions;

#pragma warning disable S3358 // Ternary operators should not be nested

/// <summary>
/// <para>
///     A Problem Details with extensions. See <see cref="ProblemDetails"/> for more information.
/// </para>
/// <para>
///     Additional properties are added to this class to support the extensions, and they are:
/// </para>
/// <list type="bullet">
///     <item>
///         <term><see cref="InvalidParameters"/></term> that contains the invalid parameters details, 
///         like the parameter name, the error message (reason) and additional information.
///     </item>
///     <item>
///         <term><see cref="NotFoundDetails"/></term> that contains the not found details,
///         like the property name and the error message and additional information.
///     </item>
///     <item>
///         <term><see cref="Errors"/></term> that contains messages for generic errors or application errors.
///     </item>
///     <item>
///         <term><see cref="InnerProblemDetails"/></term> that contains inner <see cref="ProblemDetails"/>
///         for aggregate exceptions.
///     </item>
/// </list>
/// </summary>
public class ProblemDetailsExtended : ProblemDetails
{
    /// <summary>
    /// Contains the constants for the extension fields used by the convertion of the <see cref="ProblemDetailsExtended"/>.
    /// </summary>
    public static class Fields
    {
        /// <summary>
        /// Extension field for the <see cref="ProblemDetails"/> that contains the aggregate details.
        /// </summary>
        public const string AggregateExtensionField = "inner_details";

        /// <summary>
        /// Extension field for the <see cref="ProblemDetails"/> that contains the invalid parameters details.
        /// </summary>
        public const string InvalidParametersExtensionField = "invalid_params";

        /// <summary>
        /// Extension field for the <see cref="ProblemDetails"/> that contains generic errors or application errors.
        /// </summary>
        public const string ErrorsExtensionField = "errors";

        /// <summary>
        /// Extension field for the <see cref="ProblemDetails"/> that contains the not found details.
        /// </summary>
        public const string NotFoundExtensionField = "not_found";
    }

    /// <summary>
    /// Contains the constants for the default titles used to the problem details.
    /// </summary>
    public static class Titles
    {
        /// <summary>
        /// The default title for the not found errors.
        /// </summary>
        public static string NotFoundTitle { get; set; } = R.NotFoundTitle;

        /// <summary>
        /// The default title for the invalid parameters errors.
        /// </summary>
        public static string InvalidParametersTitle { get; set; } = R.InvalidParametersTitle;

        /// <summary>
        /// The default title for the validation errors.
        /// </summary>
        public static string ValidationFailedTitle { get; set; } = R.InvalidParametersTitle;

        /// <summary>
        /// The default title for the invalid operation errors.
        /// </summary>
        public static string InvalidStateTitle { get; set; } = R.InvalidStateTitle;

        /// <summary>
        /// The default title for the not allowed errors.
        /// </summary>
        public static string NotAllowedTitle { get; set; } = R.NotAllowedTitle;

        /// <summary>
        /// Mensagem padrão para erros internos.
        /// </summary>
        public static string InternalServerErrorTitle { get; set; } = R.InternalServerErrorTitle;
    }

    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Invalid parameters errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(Fields.InvalidParametersExtensionField)]
    public IEnumerable<InvalidParameterDetails>? InvalidParameters { get; set; }

    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Not found errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(Fields.NotFoundExtensionField)]
    public IEnumerable<NotFoundDetails>? NotFoundDetails { get; set; }

    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Internal errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(Fields.ErrorsExtensionField)]
    public IEnumerable<ErrorDetails>? Errors { get; set; }

    /// <summary>
    /// <para>
    ///     Inner problem details. When a problem details is wrapped by another problem details,
    ///     the inner problem details are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(Fields.AggregateExtensionField)]
    public IEnumerable<ProblemDetails>? InnerProblemDetails { get; set; }

    /// <summary>
    /// <para>
    ///     Convert the <see cref="ProblemDetailsExtended"/> to a <see cref="Result"/>.
    /// </para>
    /// </summary>
    /// <returns>
    ///     A new instance of <see cref="Result"/> with the <see cref="Problems"/> created from the <see cref="ProblemDetailsExtended"/>.
    /// </returns>
    public Result ToResult()
    {
        Result result = new();

        bool ignoreDetails = false;

        if (InvalidParameters is not null)
        {
            // if status is 400 then use invalid parameters, otherwise use validation failed
            var useValidation = Status != 400;

            foreach (var invalidParameter in InvalidParameters)
            {
                var problem = useValidation
                    ? Problems.ValidationFailed(invalidParameter.Reason, invalidParameter.Name ?? string.Empty)
                    : Problems.InvalidParameter(invalidParameter.Reason, invalidParameter.Name ?? string.Empty);
                if (invalidParameter.Extensions is not null)
                    foreach (var extension in invalidParameter.Extensions)
                        problem.With(extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);

                result += problem;
            }

            if (Title == Titles.InvalidParametersTitle || Title == Titles.ValidationFailedTitle)
            {
                ignoreDetails = true;
            }
        }

        if (NotFoundDetails is not null)
        {
            foreach (var notFoundDetail in NotFoundDetails)
            {
                var problem = Problems.NotFound(notFoundDetail.Message, notFoundDetail.Property ?? string.Empty);
                if (notFoundDetail.Extensions is not null)
                    foreach (var extension in notFoundDetail.Extensions)
                        problem.With(extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);

                result += problem;
            }

            if (Title == Titles.NotFoundTitle)
                ignoreDetails = true;
        }

        if (Errors is not null)
        {
            bool isInternalError = Status == 500;
            bool isConflict = Status == 409;
            bool isNotAllowed = Status == 403;

            foreach (var errorDetails in Errors)
            {
                var property = errorDetails.GetProperty();

                var problem = isInternalError
                    ? Problems.InternalError(errorDetails.Detail, property)
                    : isConflict
                        ? Problems.InvalidState(errorDetails.Detail, property)
                        : isNotAllowed
                            ? Problems.NotAllowed(errorDetails.Detail, property)
                            : Problems.ValidationFailed(errorDetails.Detail, property);
                
                if (errorDetails.Extensions is not null)
                    foreach (var extension in errorDetails.Extensions)
                        AddExtension(problem, extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);

                result += problem;
            }

            if (Title == Titles.InvalidStateTitle
                || Title == Titles.ValidationFailedTitle
                || Title == Titles.InternalServerErrorTitle
                || Title == Titles.NotAllowedTitle)
            {
                ignoreDetails = true;
            }
        }

        if (InnerProblemDetails is not null)
        {
            foreach (var innerProblemDetail in InnerProblemDetails)
            {
                result += ToProblem(innerProblemDetail);
            }

            ignoreDetails = true;
        }

        if (ignoreDetails)
        {
            if (Extensions?.Count > 0)
            {
                var problem = result[0];
                foreach (var extension in Extensions)
                    problem.With(extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);
            }
        }
        else
        {
            result += ToProblem(this);
        }

        return result;
    }

    private static Problem ToProblem(ProblemDetails details)
    {
        // obtém a propriedade do problema
        string? property = null;
        if (details.Extensions is not null && details.Extensions.TryGetValue("property", out var propertyValue))
            property = ReadJsonValue(propertyValue) as string;

        // obtém a categoria do problema
        ProblemCategory category;
        if (details.Type == "about:blank")
        {
            category = details.Status switch
            {
                404 => ProblemCategory.NotFound,
                400 => ProblemCategory.InvalidParameter,
                422 => ProblemCategory.ValidationFailed,
                409 => ProblemCategory.InvalidState,
                500 => ProblemCategory.InvalidState,
                _ => ProblemCategory.CustomProblem,
            };
        }
        else
        {
            category = ProblemCategory.CustomProblem;
        }

        // creates the result message
        var message = new Problem()
        {
            Category = category,
            Detail = details.Detail ?? string.Empty,
            Property = property,
        };

        // add the additional information
        if (details.Extensions is not null)
            foreach (var extension in details.Extensions)
            {
                if (extension.Key == "property")
                    continue;
                message.With(extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);
            }

        return message;
    }

    private static object? ReadJsonValue(object? obj)
    {
        if (obj is null)
            return null;

        if (obj is JsonElement jsonElement)
            return ReadJsonElement(jsonElement);

        return obj;
    }

    private static object? ReadJsonElement(JsonElement jsonElement)
    {
        // convert the json element to a object of the correct type
        return jsonElement.ValueKind switch
        {
            JsonValueKind.String => jsonElement.GetString(),
            JsonValueKind.Number => jsonElement.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Array => jsonElement.EnumerateArray().Select(ReadJsonElement).ToArray(),
            JsonValueKind.Object => ReadJsonObject(jsonElement),
            _ => jsonElement.GetRawText(),
        };
    }

    private static object ReadJsonObject(JsonElement jsonElement)
    {
        // convert the json element to a object of the correct type
        var obj = new Dictionary<string, object?>();
        foreach (var property in jsonElement.EnumerateObject())
        {
            obj.Add(property.Name, ReadJsonElement(property.Value));
        }

        return obj;
    }

    private static void AddExtension(Problem problem, string key, object value)
    {
        if (key == "property")
            return;

        problem.With(key, value);
    }
}
