namespace RoyalCode.SmartProblems;

public sealed class ExceptionOptions
{
    /// <summary>
    /// <para>
    ///     Determines if the exception type name should be included in the problem as an extension.
    /// </para>
    /// <para>
    ///     The default value is <c>false</c>.
    /// </para>
    /// </summary>
    public bool IncludeExceptionTypeName { get; set; }
    
    /// <summary>
    /// <para>
    ///     Determines if the exception stack trace should be included in the problem as an extension.
    /// </para>
    /// <para>
    ///     The default value is <c>false</c>.
    /// </para>
    /// </summary>
    public bool IncludeStackTrace { get; set; }

    /// <summary>
    /// <para>
    ///     Determines if the exception message should be included in the problem as a detail.
    /// </para>
    /// <para>
    ///     When <c>false</c>, <see cref="DefaultExceptionMessage"/> will be used.
    /// </para>
    /// <para>
    ///     The default value is <c>true</c>.
    /// </para>
    /// </summary>
    public bool UseExceptionMessageAsDetail { get; set; } = true;
    
    /// <summary>
    /// <para>
    ///     The default message to be used when the exception message is empty
    ///     or when the <see cref="UseExceptionMessageAsDetail"/> is <c>false</c>.
    /// </para>
    /// </summary>
    public string DefaultExceptionMessage { get; set; } = R.InternalServerErrorMessage;
}