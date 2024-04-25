using RoyalCode.SmartProblems;
using RoyalCode.SmartProblems.Convertions;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace System.Net.Http;

/// <summary>
/// <para>
///     Extension methods for deserialize <see cref="Result" /> from <see cref="HttpResponseMessage"/>.
/// </para>
/// </summary>
public static class HttpResultExtensions
{
    /// <summary>
    /// <para>
    ///     Get <see cref="Result" /> from <see cref="HttpResponseMessage"/>.
    /// </para>
    /// </summary>
    /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
    /// <param name="token">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Result"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result> ToResultAsync(
        this HttpResponseMessage response, CancellationToken token = default)
    {
        return ToResultAsync(response, null, token);
    }
    
    /// <summary>
    /// <para>
    ///     Get <see cref="Result" /> from <see cref="HttpResponseMessage"/>.
    /// </para>
    /// </summary>
    /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
    /// <param name="failureTypeReader">A <see cref="FailureTypeReader"/> to read the error content when the status code is not success and the content is not a problem details.</param>
    /// <param name="token">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Result"/>.</returns>
    public static async Task<Result> ToResultAsync(
        this HttpResponseMessage response, FailureTypeReader? failureTypeReader, CancellationToken token = default)
    {
        if (response.IsSuccessStatusCode)
        {
            // on success, when there is no value,
            // returns a success OperationResult with no message
            return Result.Ok();
        }

        return await response.ReadErrorStatus(failureTypeReader, token);
    }

    /// <summary>
    /// <para>
    ///     Get <see cref="Result{TValue}" /> from <see cref="HttpResponseMessage"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
    /// <param name="options">
    ///     The <see cref="JsonSerializerOptions"/> for the <typeparamref name="TValue"/>, 
    ///     used when status code is success.
    /// </param>
    /// <param name="token">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Result{TValue}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TValue>> ToResultAsync<TValue>(
        this HttpResponseMessage response, JsonSerializerOptions? options = null, CancellationToken token = default)
    {
        return ToResultAsync<TValue>(response, null, options, token);
    }

    /// <summary>
    /// <para>
    ///     Get <see cref="Result{TValue}" /> from <see cref="HttpResponseMessage"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
    /// <param name="failureTypeReader">A <see cref="FailureTypeReader"/> to read the error content when the status code is not success and the content is not a problem details.</param>
    /// <param name="options">
    ///     The <see cref="JsonSerializerOptions"/> for the <typeparamref name="TValue"/>, 
    ///     used when status code is success.
    /// </param>
    /// <param name="token">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Result{TValue}"/>.</returns>
    public static async Task<Result<TValue>> ToResultAsync<TValue>(
        this HttpResponseMessage response,
        FailureTypeReader? failureTypeReader, 
        JsonSerializerOptions? options = null,
        CancellationToken token = default)
    {
        // on error, read the content as a problem details or text.
        if (!response.IsSuccessStatusCode) 
            return await response.ReadErrorStatus(failureTypeReader, token);
        
        // on success, with value, the value must be deserialized
        var value = await response.Content.ReadFromJsonAsync<TValue>(options, token);

        return value!;
    }

    private static Task<Problems> ReadErrorStatus(
        this HttpResponseMessage response, FailureTypeReader? failureTypeReader, CancellationToken token)
    {
        var mediaType = response.Content.Headers.ContentType?.MediaType;
        return mediaType switch
        {
            // check the content
            "application/problem+json" => response.ReadProblemDetails(token),
            _ => response.ReadNonProblemDetailsContent(failureTypeReader, token)
        };
    }
    
    private static async Task<Problems> ReadNonProblemDetailsContent(
        this HttpResponseMessage response, FailureTypeReader? failureTypeReader, CancellationToken token = default)
    {
        // if reader is not null, try reads the content as a failure type
        if (failureTypeReader is not null)
        {
            var result = await failureTypeReader.TryReadAsync(response);
            if (result)
                return result;
        }

        // when is not problems, try reads the content as string, if the response has content
        string? detail = null;
        if (response.Content.Headers.ContentLength > 0)
        {
            detail = await response.Content.ReadAsStringAsync(token);
        }

        detail ??= response.ReasonPhrase ?? response.StatusCode.ToString();
        
        // create a message with the status code and the content as message
        return new Problem()
        {
            Detail = detail,
            Category = response.StatusCode.ToCategory()
        };
    }

    private static async Task<Problems> ReadProblemDetails(
        this HttpResponseMessage response, CancellationToken token)
    {
        var problemDetails = await response.Content.ReadFromJsonAsync(
            ProblemDetailsSerializer.DefaultProblemDetailsExtended,
            token);

        return problemDetails!.ToProblems();
    }
}
