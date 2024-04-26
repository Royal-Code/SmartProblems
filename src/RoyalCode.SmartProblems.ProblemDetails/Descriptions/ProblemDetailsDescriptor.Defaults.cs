using RoyalCode.SmartProblems.Convertions;
using System.Net;

namespace RoyalCode.SmartProblems.Descriptions;

public partial class ProblemDetailsDescriptor
{
    /// <summary>
    /// Messages for well-known problem details.
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// The Detail field for the an aggregation of problems.
        /// </summary>
        public static string AggregateMessage { get; set; } = DR.AggregateMessage;

        /// <summary>
        /// The Detail field for the an internal error.
        /// </summary>
        public static string InternalErrorMessage { get; set; } = DR.InternalErrorMessage;

        /// <summary>
        /// The Detail field for the an invalid state.
        /// </summary>
        public static string InvalidStateMessage { get; set; } = DR.InvalidStateMessage;

        /// <summary>
        /// The Detail field for the validation failed.
        /// </summary>
        public static string ValidationFailedMessage { get; set; } = DR.ValidationFailedMessage;

        /// <summary>
        /// The Detail field for the an not allowed operation.
        /// </summary>
        public static string NotAllowedMessage { get; set; } = DR.NotAllowedMessage;

        /// <summary>
        /// The Detail field for the an invalid parameters.
        /// </summary>
        public static string InvalidParametersMessage { get; set; } = DR.InvalidParametersMessage;

        /// <summary>
        /// The Detail field for the an entity not found.
        /// </summary>
        public static string NotFoundMessage { get; set; } = DR.NotFoundMessage;

        /// <summary>
        /// The Detail field for the an undefined error.
        /// </summary>
        public static string DefaultMessage { get; set; } = DR.DefaultMessage;
    }

    /// <summary>
    /// Titles for well-known problem details.
    /// </summary>
    public static class Titles
    {
        /// <summary>
        /// Default title for the problem details of an aggregation of problems.
        /// </summary>
        public static string AggregateProblemsDetailsTitle { get; set; } = DR.AggregateProblemsDetailsTitle;

        /// <summary>
        /// Default title for the problem details of type "about:blank".
        /// </summary>
        public static string AboutBlankTitle { get; set; } = DR.AboutBlankTitle;
    }

    /// <summary>
    /// Constants for the defaults values of the problem details.
    /// </summary>
    public static class Types
    {
        /// <summary>
        /// The "about:blank" URI, when used as a problem type, 
        /// indicates that the problem has no additional semantics beyond that of the HTTP status code.
        /// </summary>
        public const string AboutBlank = "about:blank";
    }

    /// <summary>
    /// Contains the type ids for the generic problems, defined by the <see cref="ProblemCategory"/>.
    /// </summary>
    public static class GenericProblemsTypeIds
    {
        public const string NotFound = "not-found";

        public const string InvalidParameter = "invalid-parameter";

        public const string InvalidState = "invalid-state";

        public const string ValidationFailed = "validation-failed";

        public const string NotAllowed = "not-allowed";

        public const string InternalServerError = "internal-server-error";

        public const string CustomProblem = "problem-occurred";

        /// <summary>
        /// The key for the an aggregation of problems.
        /// </summary>
        public const string AggregateProblemsDetails = "aggregate-problems-details";
    }

    /// <summary>
    /// The factory to create the default descriptions of the problem details for the generic errors.
    /// </summary>
    public static Dictionary<string, ProblemDetailsDescription> DescriptionFactory() => new()
    {
        {
            GenericProblemsTypeIds.AggregateProblemsDetails,
            new ProblemDetailsDescription(
                GenericProblemsTypeIds.AggregateProblemsDetails,
                Titles.AggregateProblemsDetailsTitle,
                DR.AggregateProblemsDetailsDescription,
                HttpStatusCode.BadRequest)
        }
    };

    /// <summary>
    /// The factory to create the default descriptions of the problem details for the generic errors.
    /// </summary>
    public static Dictionary<ProblemCategory, ProblemDetailsDescription> GenericProblemsDescriptionFactory() => new()
    {
        {
            ProblemCategory.NotFound,
            new ProblemDetailsDescription(
                GenericProblemsTypeIds.NotFound,
                Types.AboutBlank,
                ProblemDetailsExtended.Titles.NotFoundTitle,
                DR.NotFoundDescription,
                HttpStatusCode.NotFound)
        },
        {
            ProblemCategory.InvalidParameter,
            new ProblemDetailsDescription(
                GenericProblemsTypeIds.InvalidParameter,
                Types.AboutBlank,
                ProblemDetailsExtended.Titles.InvalidParametersTitle,
                DR.InvalidParameterDescription,
                HttpStatusCode.BadRequest)
        },
        {
            ProblemCategory.InvalidState,
            new ProblemDetailsDescription(
                GenericProblemsTypeIds.InvalidState,
                Types.AboutBlank,
                ProblemDetailsExtended.Titles.InvalidStateTitle,
                DR.InvalidStateDescription,
                HttpStatusCode.Conflict)
        },
        {
            ProblemCategory.ValidationFailed,
            new ProblemDetailsDescription(
                GenericProblemsTypeIds.ValidationFailed,
                Types.AboutBlank,
                ProblemDetailsExtended.Titles.ValidationFailedTitle,
                DR.ValidationFailedDescription,
                HttpStatusCode.UnprocessableEntity)
        },
        {
            ProblemCategory.NotAllowed,
            new ProblemDetailsDescription(
                GenericProblemsTypeIds.NotAllowed,
                Types.AboutBlank,
                ProblemDetailsExtended.Titles.NotAllowedTitle,
                DR.NotAllowedDescription,
                HttpStatusCode.Forbidden)
        },
        {
            ProblemCategory.InternalServerError,
            new ProblemDetailsDescription(
                GenericProblemsTypeIds.InternalServerError,
                Types.AboutBlank,
                ProblemDetailsExtended.Titles.InternalServerErrorTitle,
                DR.InternalServerErrorDescription,
                HttpStatusCode.InternalServerError)
        },
        {
            ProblemCategory.CustomProblem,
            new ProblemDetailsDescription(
                GenericProblemsTypeIds.CustomProblem,
                ProblemDetailsExtended.Titles.DefaultTitle,
                DR.CustomProblemDescription,
                HttpStatusCode.BadRequest)
        }
    };
}
