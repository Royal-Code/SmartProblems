using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoyalCode.SmartProblems.Convertions;
using RoyalCode.SmartProblems.Metadata;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace RoyalCode.SmartProblems.HttpResults;

/// <summary>
/// <para>
///     Minimal API Result for <see cref="Problems"/>.
/// </para>
/// <para>
///     Used for create a result from the <see cref="Result"/> match for the error case.
/// </para>
/// </summary>
public class MatchErrorResult
    : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<Problems>
{
    /// <summary>
    /// Creates a new <see cref="MatchErrorResult"/> for the <see cref="Problems"/>.
    /// </summary>
    /// <param name="problems">The <see cref="Problems"/> to be converted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MatchErrorResult(Problems problems) => new(problems);

    /// <summary>
    /// Creates a new <see cref="MatchErrorResult"/> for the <see cref="Problem"/>.
    /// </summary>
    /// <param name="problem">The <see cref="Problem"/> to be converted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MatchErrorResult(Problem problem) => new(problem);

    /// <summary>
    /// Creates a new <see cref="MatchErrorResult"/> for the <see cref="Exception"/>.
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> to be converted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator MatchErrorResult(Exception ex) => new(ex);

    private readonly Problems problems;

    /// <summary>
    /// Creates a new instance of <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="problems">The <see cref="Problems"/> to be converted.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="problems"/> is <see langword="null"/>.
    /// </exception>
    public MatchErrorResult(Problems problems)
    {
        this.problems = problems ?? throw new ArgumentNullException(nameof(problems));
    }

    /// <summary>
    /// Creates a new instance of <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="problem">The <see cref="Problem"/> to be converted.</param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="problem"/> is <see langword="null"/>.
    /// </exception>
    public MatchErrorResult(Problem problem) : this([problem]) { }

    /// <summary>
    /// Creates a new instance of <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> to be converted.</param>
    public MatchErrorResult(Exception ex) : this(Problems.InternalError(ex)) { }

    /// <inheritdoc />
    public int? StatusCode { get; private set; }

    /// <inheritdoc />
    public object? Value => problems;

    /// <inheritdoc />
    Problems? IValueHttpResult<Problems>.Value => problems;

    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        var options = httpContext.RequestServices.GetRequiredService<IOptions<Descriptions.ProblemDetailsOptions>>().Value;
        var problemDetails = problems.ToProblemDetails(options);
        JsonSerializerOptions? serializerOptions = null;

        StatusCode = problemDetails.Status;

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest;
        return httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            serializerOptions,
            "application/problem+json",
            httpContext.RequestAborted);
    }

    /// <inheritdoc />
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        var attr = method?.GetCustomAttribute<ProduceProblemsAttribute>();

        Type type;
        string[] content;

        type = typeof(ProblemDetails);
        content = ["application/problem+json"];

        if (attr is not null)
        {
            foreach (var statusCode in attr.GetStatusCodes())
            {
                builder.Metadata.Add(new ResponseTypeMetadata(type, statusCode, content));
            }
        }
        else
        {
            builder.Metadata.Add(
                new ResponseTypeMetadata(type, StatusCodes.Status400BadRequest, content));
            builder.Metadata.Add(
                new ResponseTypeMetadata(type, StatusCodes.Status404NotFound, content));
            builder.Metadata.Add(
                new ResponseTypeMetadata(type, StatusCodes.Status409Conflict, content));
            builder.Metadata.Add(
                new ResponseTypeMetadata(type, StatusCodes.Status422UnprocessableEntity, content));
        }
    }
}
