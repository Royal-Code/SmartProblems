using RoyalCode.SmartProblems.HttpResults;

namespace RoyalCode.SmartProblems.TestsApi.Apis;

public static class ProblemsApis
{

    public static void MapProblems(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("api/problems")
            .WithDescription("Apis for problems results.")
            .WithName("ProblemsResults")
            .WithDisplayName("Problems Results API");



        group.MapGet("not-found", NotFound)
            .WithDescription("Returns a not found problem.")
            .WithName("NotFound")
            .WithDisplayName("NotFound");

        group.MapGet("invalid-parameter", InvalidParameter)
            .WithDescription("Returns an invalid parameter problem.")
            .WithName("InvalidParameter")
            .WithDisplayName("Invalid Parameter");

        group.MapGet("validation-failed", ValidationFailed)
            .WithDescription("Returns a validation failed problem.")
            .WithName("ValidationFailed")
            .WithDisplayName("Validation Failed");

        group.MapGet("invalid-state", InvalidState)
            .WithDescription("Returns an invalid state problem.")
            .WithName("InvalidState")
            .WithDisplayName("Invalid State");

        group.MapGet("not-allowed", NotAllowed)
            .WithDescription("Returns a not allowed problem.")
            .WithName("NotAllowed")
            .WithDisplayName("Not Allowed");

        group.MapGet("internal-server-error", InternalServerError)
            .WithDescription("Returns an internal server error problem.")
            .WithName("InternalServerError")
            .WithDisplayName("Internal Server Error");

        group.MapGet("custom-problem", CustomProblem)
            .WithDescription("Returns a custom problem.")
            .WithName("CustomProblem")
            .WithDisplayName("Custom Problem");



        group.MapGet("many-not-found", ManyNotFound)
            .WithDescription("Returns many not found problems.")
            .WithName("ManyNotFound")
            .WithDisplayName("Many NotFound");

        group.MapGet("many-invalid-parameter", ManyInvalidParameter)
            .WithDescription("Returns many invalid parameter problems.")
            .WithName("ManyInvalidParameter")
            .WithDisplayName("Many Invalid Parameter");

        group.MapGet("many-validation-failed", ManyValidationFailed)
            .WithDescription("Returns many validation failed problems.")
            .WithName("ManyValidationFailed")
            .WithDisplayName("Many Validation Failed");

        group.MapGet("many-invalid-state", ManyInvalidState)
            .WithDescription("Returns many invalid state problems.")
            .WithName("ManyInvalidState")
            .WithDisplayName("Many Invalid State");

        group.MapGet("many-not-allowed", ManyNotAllowed)
            .WithDescription("Returns many not allowed problems.")
            .WithName("ManyNotAllowed")
            .WithDisplayName("Many Not Allowed");

        group.MapGet("many-internal-server-error", ManyInternalServerError)
            .WithDescription("Returns many internal server error problems.")
            .WithName("ManyInternalServerError")
            .WithDisplayName("Many Internal Server Error");

        group.MapGet("many-custom-problem", ManyCustomProblem)
            .WithDescription("Returns many custom problems.")
            .WithName("ManyCustomProblem")
            .WithDisplayName("Many Custom Problem");



        group.MapGet("not-found-invalid-parameter", NotFound_InvalidParameter)
            .WithDescription("Returns a not found and an invalid parameter problems.")
            .WithName("NotFoundInvalidParameter")
            .WithDisplayName("NotFound Invalid Parameter");

        group.MapGet("not-found-invalid-parameter-validation-failed", NotFound_InvalidParameter_ValidationFailed)
            .WithDescription("Returns a not found, an invalid parameter and a validation failed problems.")
            .WithName("NotFoundInvalidParameterValidationFailed")
            .WithDisplayName("NotFound Invalid Parameter Validation Failed");

        group.MapGet("not-found-invalid-parameter-validation-failed-invalid-state",
                NotFound_InvalidParameter_ValidationFailed_InvalidState)
            .WithDescription("Returns a not found, an invalid parameter, a validation failed and an invalid state problems.")
            .WithName("NotFoundInvalidParameterValidationFailedInvalidState")
            .WithDisplayName("NotFound Invalid Parameter Validation Failed Invalid State");

        group.MapGet("not-found-invalid-parameter-validation-failed-invalid-state-not-allowed", 
                NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed)
            .WithDescription("Returns a not found, an invalid parameter, a validation failed, an invalid state and a not allowed problems.")
            .WithName("NotFoundInvalidParameterValidationFailedInvalidStateNotAllowed")
            .WithDisplayName("NotFound Invalid Parameter Validation Failed Invalid State Not Allowed");

        group.MapGet("not-found-invalid-parameter-validation-failed-invalid-state-not-allowed-internal-server-error", 
                NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed_InternalServerError)
            .WithDescription("Returns a not found, an invalid parameter, a validation failed, an invalid state, a not allowed and an internal server error problems.")
            .WithName("NotFoundInvalidParameterValidationFailedInvalidStateNotAllowedInternalServerError")
            .WithDisplayName("NotFound Invalid Parameter Validation Failed Invalid State Not Allowed Internal Server Error");

        group.MapGet("not-found-invalid-parameter-validation-failed-invalid-state-not-allowed-internal-server-error-custom-problem",
                NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed_InternalServerError_CustomProblem)
            .WithDescription("Returns a not found, an invalid parameter, a validation failed, an invalid state, a not allowed, an internal server error and a custom problem problems.")
            .WithName("NotFoundInvalidParameterValidationFailedInvalidStateNotAllowedInternalServerErrorCustomProblem")
            .WithDisplayName("NotFound Invalid Parameter Validation Failed Invalid State Not Allowed Internal Server Error Custom Problem");

        group.MapGet("not-found-invalid-parameter-validation-failed-invalid-state-not-allowed-internal-server-error-custom-problem-many", 
                NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed_InternalServerError_CustomProblem_Many)
            .WithDescription("Returns a not found, an invalid parameter, a validation failed, an invalid state, a not allowed, an internal server error, a custom problem and many custom problems problems.")
            .WithName("NotFoundInvalidParameterValidationFailedInvalidStateNotAllowedInternalServerErrorCustomProblemMany")
            .WithDisplayName("NotFound Invalid Parameter Validation Failed Invalid State Not Allowed Internal Server Error Custom Problem Many");
    }

    private static NoContentMatch NotFound()
    {
        return Problems.NotFound("Not Found");
    }

    private static NoContentMatch InvalidParameter()
    {
        return Problems.InvalidParameter("Invalid Parameter", "MyProperty");
    }

    private static NoContentMatch ValidationFailed()
    {
        return Problems.ValidationFailed("Validation Failed", "MyProperty");
    }

    private static NoContentMatch InvalidState()
    {
        return Problems.InvalidState("Invalid State");
    }

    private static NoContentMatch NotAllowed()
    {
        return Problems.NotAllowed("Not Allowed");
    }

    private static NoContentMatch InternalServerError()
    {
        return Problems.InternalError(new Exception("Internal Server Error"));
    }

    private static NoContentMatch CustomProblem()
    {
        return Problems.Custom("Custom Problem", "my-custom-type");
    }

    private static NoContentMatch ManyNotFound()
    {
        return Problems.NotFound("Not Found 1")
            + Problems.NotFound("Not Found 2")
            + Problems.NotFound("Not Found 3");
    }

    private static NoContentMatch ManyInvalidParameter()
    {
        return Problems.InvalidParameter("Invalid Parameter 1", "MyProperty1")
            + Problems.InvalidParameter("Invalid Parameter 2", "MyProperty2")
            + Problems.InvalidParameter("Invalid Parameter 3", "MyProperty3");
    }

    private static NoContentMatch ManyValidationFailed()
    {
        return Problems.ValidationFailed("Validation Failed 1", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed 2", "MyProperty2")
            + Problems.ValidationFailed("Validation Failed 3", "MyProperty3");
    }

    private static NoContentMatch ManyInvalidState()
    {
        return Problems.InvalidState("Invalid State 1")
            + Problems.InvalidState("Invalid State 2")
            + Problems.InvalidState("Invalid State 3");
    }

    private static NoContentMatch ManyNotAllowed()
    {
        return Problems.NotAllowed("Not Allowed 1")
            + Problems.NotAllowed("Not Allowed 2")
            + Problems.NotAllowed("Not Allowed 3");
    }

    private static NoContentMatch ManyInternalServerError()
    {
        return Problems.InternalError(new Exception("Internal Server Error 1"))
            + Problems.InternalError(new Exception("Internal Server Error 2"))
            + Problems.InternalError(new Exception("Internal Server Error 3"));
    }

    private static NoContentMatch ManyCustomProblem()
    {
        return Problems.Custom("Custom Problem 1", "my-custom-type")
            + Problems.Custom("Custom Problem 2", "my-custom-type")
            + Problems.Custom("Custom Problem 3", "my-custom-type");
    }

    private static NoContentMatch NotFound_InvalidParameter()
    {
        return Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty");
    }

    private static NoContentMatch NotFound_InvalidParameter_ValidationFailed()
    {
        return Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2");
    }

    private static NoContentMatch NotFound_InvalidParameter_ValidationFailed_InvalidState()
    {
        return Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State");
    }

    private static NoContentMatch NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed()
    {
        return Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State")
            + Problems.NotAllowed("Not Allowed");
    }

    private static NoContentMatch NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed_InternalServerError()
    {
        return Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State")
            + Problems.NotAllowed("Not Allowed")
            + Problems.InternalError(new Exception("Internal Server Error"));
    }

    private static NoContentMatch NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed_InternalServerError_CustomProblem()
    {
        return Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State")
            + Problems.NotAllowed("Not Allowed")
            + Problems.InternalError(new Exception("Internal Server Error"))
            + Problems.Custom("Custom Problem", "my-custom-type");
    }

    private static NoContentMatch NotFound_InvalidParameter_ValidationFailed_InvalidState_NotAllowed_InternalServerError_CustomProblem_Many()
    {
        return Problems.NotFound("Not Found")
            + Problems.InvalidParameter("Invalid Parameter", "MyProperty1")
            + Problems.ValidationFailed("Validation Failed", "MyProperty2")
            + Problems.InvalidState("Invalid State")
            + Problems.NotAllowed("Not Allowed")
            + Problems.InternalError(new Exception("Internal Server Error"))
            + Problems.Custom("Custom Problem 1", "my-custom-type")
            + Problems.Custom("Custom Problem 2", "my-custom-type")
            + Problems.Custom("Custom Problem 3", "my-custom-type");
    }
}
