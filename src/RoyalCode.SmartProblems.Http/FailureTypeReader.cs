namespace RoyalCode.SmartProblems.Http;

/// <summary>
/// A component to read the failure type from a <see cref="HttpResponseMessage"/>
/// when the status code is not success and the content is not a problem details.
/// </summary>
public abstract class FailureTypeReader
{
    /// <summary>
    /// Try to read the failure type from the <paramref name="response"/>.
    /// </summary>
    /// <param name="response">The response to read.</param>
    /// <returns>
    ///     A <see cref="ReadResult"/> with the problems read from the response, or <see cref="ReadResult.HasBeenRead"/> equals to false.
    /// </returns>
    public abstract Task<ReadResult> TryReadAsync(HttpResponseMessage response);
}