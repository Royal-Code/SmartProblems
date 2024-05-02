using System.Net.Http.Json;

namespace RoyalCode.SmartProblems.Http;

/// <summary>
/// A component to read the failure type from a <see cref="HttpResponseMessage"/> when the content is a json.
/// </summary>
/// <typeparam name="TResponseType">The type of the response content.</typeparam>
public abstract class JsonFailureTypeReader<TResponseType> : FailureTypeReader
{
    /// <summary>
    /// Map the response type to <see cref="Problems"/>.
    /// </summary>
    /// <param name="response">The response readed from the content.</param>
    /// <returns>A <see cref="Problems"/>.</returns>
    protected abstract Problems Map(TResponseType response);

    /// <inheritdoc />
    public override async Task<ReadResult> TryReadAsync(HttpResponseMessage response)
    {
        // check if content is application/json
        if (!response.Content.Headers.ContentType?.MediaType?.Equals("application/json", StringComparison.OrdinalIgnoreCase) ?? true)
        {
            return new();
        }
        
        // read the content
        var content = await response.Content.ReadFromJsonAsync<TResponseType>();
        
        // map the response to problems
        var problems = Map(content!);
        
        return new ReadResult
        {
            HasBeenRead = true,
            Problems = problems
        };
    }
}