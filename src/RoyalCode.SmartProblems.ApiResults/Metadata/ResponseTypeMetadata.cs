using Microsoft.AspNetCore.Http.Metadata;
using System.Net.Mime;

namespace RoyalCode.SmartProblems.Metadata;

/// <summary>
/// Provides response metadata for an endpoint, including the CLR response <see cref="Type"/>, 
/// HTTP <see cref="StatusCode"/>, and the allowed <see cref="ContentTypes"/>.
/// </summary>
/// <remarks>
/// This type is intended to be added to endpoint metadata (e.g. via <c>WithMetadata</c>) so that
/// tools (OpenAPI/Swagger, clients, analyzers) can infer the produced response shapes.
/// When no content types are specified, <c>application/json</c> is assumed by default.
/// </remarks>
/// <example>
/// Adding explicit metadata to an endpoint:
/// <code>
/// app.MapGet("/items/{id:int}", (int id) => Results.Ok(new ItemDto(id)))
///    .WithMetadata(new ResponseTypeMetadata(typeof(ItemDto), 200));
///
/// app.MapPost("/items", (CreateItemDto dto) => Results.Created($"/items/1", null))
///    .WithMetadata(new ResponseTypeMetadata(201));
/// </code>
/// </example>
public sealed class ResponseTypeMetadata : IProducesResponseTypeMetadata
{
    /// <summary>
    /// Initializes a new instance specifying the response <paramref name="type"/>, 
    /// HTTP <paramref name="statusCode"/>, and optional <paramref name="contentTypes"/>.
    /// </summary>
    /// <param name="type">The CLR type returned in the response body (nullable for empty responses).</param>
    /// <param name="statusCode">The HTTP status code produced by the endpoint.</param>
    /// <param name="contentTypes">
    /// The content types the endpoint can produce. Defaults to <c>application/json</c> when omitted.
    /// </param>
    public ResponseTypeMetadata(Type? type, int statusCode, params string[]? contentTypes)
    {
        Type = type;
        StatusCode = statusCode;
        ContentTypes = contentTypes ?? [MediaTypeNames.Application.Json];
    }

    /// <summary>
    /// Initializes a new instance specifying only the HTTP <paramref name="statusCode"/> and optional
    /// <paramref name="contentTypes"/> for responses where the body type is not declared or is empty.
    /// </summary>
    /// <param name="statusCode">The HTTP status code produced by the endpoint.</param>
    /// <param name="contentTypes">
    /// The content types the endpoint can produce. Defaults to <c>application/json</c> when omitted.
    /// </param>
    public ResponseTypeMetadata(int statusCode, params string[]? contentTypes)
    {
        StatusCode = statusCode;
        ContentTypes = contentTypes ?? [MediaTypeNames.Application.Json];
    }

    /// <summary>
    /// Gets the CLR type returned in the response body (if any).
    /// </summary>
    public Type? Type { get; }

    /// <summary>
    /// Gets the HTTP status code produced by the endpoint.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the sequence of content types the endpoint can produce.
    /// </summary>
    public IEnumerable<string> ContentTypes { get; }
}