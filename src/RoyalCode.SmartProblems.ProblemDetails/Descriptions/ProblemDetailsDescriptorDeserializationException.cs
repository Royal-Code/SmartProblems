namespace RoyalCode.SmartProblems.Descriptions;

/// <summary>
/// Exception thrown when a collection of <see cref="ProblemDetailsDescription"/> cannot be deserialized.
/// </summary>
public sealed class ProblemDetailsDescriptorDeserializationException : Exception
{
    /// <summary>
    /// Creates a new instance of the exception.
    /// </summary>
    /// <param name="json">The JSON string used to deserialization.</param>
    /// <param name="innerException">The original exception.</param>
    public ProblemDetailsDescriptorDeserializationException(string json, Exception innerException)
        : base(string.Format(DR.ProblemDetailsExceptionMessagePattern, innerException.GetType().Name, innerException.Message , json), innerException)
    { }
}