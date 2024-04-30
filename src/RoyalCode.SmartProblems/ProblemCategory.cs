namespace RoyalCode.SmartProblems;

/// <summary>
/// The problem categories that the system can handle.
/// </summary>
public enum ProblemCategory
{
    /// <summary>
    /// <para>
    ///     Define a custom problem for the system.
    /// </para>
    /// <para>
    ///     This kind of problem is used when the system needs to define a custom problem
    ///     that the defaults problems does not have the necessary information to represent.
    /// </para>
    /// <para>
    ///     Problems of this category will require the TypeId and a ProblemDetailsDescription for convert
    ///     to ProblemDetails.
    /// </para>
    /// </summary>
    CustomProblem,
    
    /// <summary>
    /// <para>
    ///     Define a problem for a resource that was not found.
    /// </para>
    /// <para>
    ///     This is equivalent to the HTTP status code 404 (Not Found).
    /// </para>
    /// </summary>
    NotFound,
    
    /// <summary>
    /// <para>
    ///     Define a problem for an invalid input parameter.
    /// </para>
    /// <para>
    ///    This is equivalent to the HTTP status code 400 (Bad Request).
    /// </para>
    /// </summary>
    InvalidParameter,
    
    /// <summary>
    /// <para>
    ///     Define a problem for when a business rule is violated.
    /// </para>
    /// <para>
    ///     This is equivalent to the HTTP status code 422 (Unprocessable Entity).
    /// </para>
    /// </summary>
    ValidationFailed,
    
    /// <summary>
    /// <para>
    ///     Define a problem for an invalid state of the system or entity.
    /// </para>
    /// <para>
    ///     This is equivalent to the HTTP status code 409 (Conflict).
    /// </para>
    /// </summary>
    InvalidState,

    /// <summary>
    /// <para>
    ///     Define a problem for a request that is not allowed to be executed.
    ///     <br/>
    ///     In these problems there is some reason why execution is not allowed (forbidden) 
    ///     and it is important to clearly state the reason, 
    ///     possibly by providing extra data and creating an error type.
    /// </para>
    /// <para>
    ///     This is equivalent to the HTTP status code 403 (Forbidden).
    /// </para>
    /// </summary>
    NotAllowed,

    /// <summary>
    /// <para>
    ///     Defines a problem for when an unexpected exception/error occurs in the system.
    /// </para>
    /// <para>
    ///     This is equivalent to the HTTP status code 500 (Internal Server Error).
    /// </para>
    /// </summary>
    InternalServerError
}
