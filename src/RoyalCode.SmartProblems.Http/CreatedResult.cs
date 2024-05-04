using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace RoyalCode.SmartProblems.Http;

/// <summary>
/// <para>
///     A result for HTTP calls that indicates that a new resource was created.
/// </para>
/// <para>
///     This result is used when a new resource is created and the server responds with a 201 status code.
/// </para>
/// <para>
///     When the response is not successful, a problem detail is expected to be returned, and the
///     problem details will be converted to <see cref="Problems"/>, like the other results
///     (<see cref="Result"/> and <see cref="Result{TValue}"/>).
/// </para>
/// </summary>
public readonly struct CreatedResult
{
    private readonly Result result;
    private readonly HttpResponseMessage response;
    
    /// <summary>
    /// Creates a new instance of <see cref="CreatedResult"/>.
    /// </summary>
    /// <param name="result">The result of the operation.</param>
    /// <param name="response">The HTTP response.</param>
    public CreatedResult(Result result, HttpResponseMessage response)
    {
        this.result = result;
        this.response = response;
    }

    /// <summary>
    /// Checks if the result is a success.
    /// </summary>
    public bool IsSuccess => result.IsSuccess;

    /// <summary>
    /// Checks if the result is a failure.
    /// </summary>
    public bool IsFailure => result.IsFailure;
    
    /// <summary>
    /// <para>
    ///     Check if the result is a failure and return the problems.
    /// </para>
    /// <para>
    ///     When the result is a success, the problems will be null.
    /// </para>
    /// </summary>
    /// <param name="problems">The problems.</param>
    /// <returns>
    ///     True if the result is a failure, false otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return result.HasProblems(out problems);
    }

    /// <summary>
    /// <para>
    ///     Check if the result is a success and return the problems.
    /// </para>
    /// <para>
    ///     When the result is a success, the problems will be null, otherwise the problems will be returned.
    /// </para>
    /// </summary>
    /// <param name="problems">The problems.</param>
    /// <returns>
    ///     True if the result is a success, false otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSuccessOrGetProblems([NotNullWhen(false)] out Problems? problems)
    {
        return result.IsSuccessOrGetProblems(out problems);
    }

    /// <summary>
    /// <para>
    ///     Check if the result is successful and return the location of the new resource.
    /// </para>
    /// <para>
    ///     When the result is a failure, the location will be null.
    /// </para>
    /// <para>
    ///     If the location is null, the resource was not created.
    /// </para>
    /// </summary>
    /// <param name="location">The location of the new resource.</param>
    /// <returns>True if the result is successful and the location is not null, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasLocation([NotNullWhen(true)] out Uri? location)
    {
        if (result.IsFailure)
        {
            location = null;
            return false;
        }
        
        location = response.Headers.Location;
        return location is not null;
    }
    
    /// <summary>
    /// Try to get the resource from the location header.
    /// </summary>
    /// <param name="client">The HTTP client to make the request.</param>
    /// <param name="options">Optional, the JSON serializer options.</param>
    /// <typeparam name="TResource">The type of the resource.</typeparam>
    /// <returns>A <see cref="Result{TResource}"/>.</returns>
    public async Task<Result<TResource>> TryGetResourceAsync<TResource>(HttpClient client, JsonSerializerOptions? options = null)
    {
        if (result.HasProblems(out var problems))
            return problems;
        
        if (!HasLocation(out var location))
            return Problems.NotFound("Resource not found, location header is not present.");
        
        var locateResponse = await client.GetAsync(location);
        return await locateResponse.ToResultAsync<TResource>();
    }
}