using System.Text.Json;
using System.Text.Json.Serialization;
using RoyalCode.SmartProblems.Conversions.Internals;

namespace RoyalCode.SmartProblems.Conversions;

/// <summary>
/// <para>
///     A Problem Details with extensions. See <see cref="ProblemDetails"/> for more information.
/// </para>
/// <para>
///     Additional properties are added to this class to support the extensions, and they are:
/// </para>
/// <list type="bullet">
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
        public const string Aggregate = "inner_details";

        /// <summary>
        /// Extension field for the <see cref="ProblemDetails"/> that contains generic errors or application errors.
        /// </summary>
        public const string Errors = "errors";

        /// <summary>
        /// Extension field for the <see cref="ProblemDetails"/> that contains the not found details.
        /// </summary>
        public const string NotFound = "not_found";

        /// <summary>
        ///  Extension field for the <see cref="ProblemDetails"/> that contains the pointer to the property,
        ///  in format of a JSON pointer.
        /// </summary>
        public const string Pointer = "pointer";
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
        /// The default title for internal errors.
        /// </summary>
        public static string InternalServerErrorTitle { get; set; } = R.InternalServerErrorTitle;

        /// <summary>
        /// The default title for the custom problems that not have a description.
        /// </summary>
        public static string DefaultTitle { get; set; } = R.DefaultTitle;
    }

    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Not found errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(Fields.NotFound)]
    public IEnumerable<ErrorDetails>? NotFoundDetails { get; set; }

    /// <summary>
    /// <para>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </para>
    /// <para>
    ///     Internal errors are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(Fields.Errors)]
    public IEnumerable<ErrorDetails>? Errors { get; set; }

    /// <summary>
    /// <para>
    ///     Inner problem details. When a problem details is wrapped by another problem details,
    ///     the inner problem details are added to this property.
    /// </para>
    /// </summary>
    [JsonPropertyName(Fields.Aggregate)]
    public IEnumerable<ProblemDetails>? InnerProblemDetails { get; set; }

    /// <summary>
    /// <para>
    ///     Convert the <see cref="ProblemDetailsExtended"/> to a <see cref="Result"/>.
    /// </para>
    /// </summary>
    /// <returns>
    ///     A new instance of <see cref="Result"/> with the <see cref="Problems"/> created from the <see cref="ProblemDetailsExtended"/>.
    /// </returns>
    public Problems ToProblems()
    {
        Problems problems = [];

        bool ignoreDetails = false;

        if (NotFoundDetails is not null)
        {
            foreach (var errorDetails in NotFoundDetails)
            {
                var problem = Problems.NotFound(errorDetails.Detail, errorDetails.Pointer.PointerToProperty());
                if (errorDetails.Extensions is not null)
                    foreach (var extension in errorDetails.Extensions)
                        problem.With(extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);

                problems += problem;
            }

            ignoreDetails = Title == Titles.NotFoundTitle;
        }

        if (Errors is not null)
        {
            foreach (var errorDetails in Errors)
            {
                string? property = errorDetails.Pointer.PointerToProperty();
                int status = GetStatusForError(errorDetails.Extensions, Status ?? default);

                var problem = status switch
                {
                    500 => Problems.InternalError(errorDetails.Detail, property: property),
                    409 => Problems.InvalidState(errorDetails.Detail, property),
                    403 => Problems.NotAllowed(errorDetails.Detail, property),
                    422 => Problems.ValidationFailed(errorDetails.Detail, property),
                    400 => Problems.InvalidParameter(errorDetails.Detail, property),
                    _ => Problems.Custom(errorDetails.Detail, GetTypeId(Type), property)
                };
                
                if (errorDetails.Extensions is not null)
                    foreach (var extension in errorDetails.Extensions)
                        problem.With(extension.Key, ReadJsonValue(extension.Value));

                problems += problem;
            }

            ignoreDetails = Type == "about:blank"
                && (Title == Titles.InvalidParametersTitle
                    || Title == Titles.ValidationFailedTitle
                    || Title == Titles.NotAllowedTitle
                    || Title == Titles.InvalidStateTitle
                    || Title == Titles.InternalServerErrorTitle);
        }

        if (InnerProblemDetails is not null)
        {
            problems = InnerProblemDetails.Aggregate(
                problems, 
                (current, innerProblemDetail) => current + ToProblem(innerProblemDetail));

            ignoreDetails = true;
        }

        if (ignoreDetails)
        {
            if (Extensions.Count is 0) 
                return problems;
            
            var problem = problems[0];
            foreach (var extension in Extensions)
                problem.With(extension.Key, ReadJsonValue(extension.Value) ?? string.Empty);
        }
        else
        {
            problems += ToProblem(this);
        }

        return problems;
    }
    
    private static Problem ToProblem(ProblemDetails details)
    {
        // try to get the pointer from the extensions and convert to a property
        string? property = TryGetProperty(details.Extensions);

        // get the category of the problem and the type id
        ProblemCategory category;
        string? typeId = null;
        if (details.Type == "about:blank")
        {
            category = details.Status switch
            {
                404 => ProblemCategory.NotFound,
                400 => ProblemCategory.InvalidParameter,
                422 => ProblemCategory.ValidationFailed,
                403 => ProblemCategory.NotAllowed,
                409 => ProblemCategory.InvalidState,
                500 => ProblemCategory.InternalServerError,
                _ => ProblemCategory.CustomProblem,
            };
        }
        else
        {
            typeId = GetTypeId(details.Type);
            category = ProblemCategory.CustomProblem;
        }

        // creates the result message
        var problem = new Problem()
        {
            Category = category,
            Detail = details.Detail ?? R.DefaultDetail,
            Property = property,
            TypeId = typeId
        };

        // add the additional information
        foreach (var extension in details.Extensions)
            problem.With(extension.Key, ReadJsonValue(extension.Value));
         
        return problem;
    }

    private static object? ReadJsonValue(object? obj)
    {
        return obj switch
        {
            null => null,
            JsonElement jsonElement => ReadJsonElement(jsonElement),
            _ => obj
        };
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

    private static Dictionary<string, object?> ReadJsonObject(JsonElement jsonElement)
    {
        // convert the json element to a object of the correct type
        var obj = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var property in jsonElement.EnumerateObject())
        {
            obj.Add(property.Name, ReadJsonElement(property.Value));
        }

        return obj;
    }

    private static string? TryGetProperty(IDictionary<string, object?>? extensions)
    {
        if (extensions?.TryGetValue(Fields.Pointer, out var propertyValue) ?? false)
            return (ReadJsonValue(propertyValue) as string).PointerToProperty();

        return null;
    }

    private static int GetStatusForError(IDictionary<string, object?>? extensions, int responseStatus)
    {
        // try to get the status from the extensions
        if (!(extensions?.TryGetValue("status", out var statusValue) ?? false)) 
            return responseStatus;

        return statusValue switch
        {
            int status => status,
            JsonElement element => element.GetInt32(),
            _ => responseStatus
        };
    }
    
    private static string GetTypeId(string? type)
    {
        if (type is null)
            return "about:blank";
        
        // if contains #, then the typeid is the part after the #
        var lastIndexOfHash = type.LastIndexOf('#');
        if (lastIndexOfHash > 0 && type.Length > lastIndexOfHash + 1)
            return type[(lastIndexOfHash + 1)..];
        
        // if contains /, then the typeid is the part after the /
        var lastIndexOfSlash = type.LastIndexOf('/');
        if (lastIndexOfSlash > 0 && type.Length > lastIndexOfSlash + 1)
            return type[(lastIndexOfSlash + 1)..];
        
        // otherwise, return the type
        return type;
    }
}
