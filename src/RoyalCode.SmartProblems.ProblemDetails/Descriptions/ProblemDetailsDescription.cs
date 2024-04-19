﻿using System.Net;
using System.Text.Json.Serialization;

namespace RoyalCode.SmartProblems.Descriptions;

/// <summary>
/// A descriptions of a specific type of problem details.
/// </summary>
public class ProblemDetailsDescription
{
    /// <summary>
    /// Creates a new instance of <see cref="ProblemDetailsDescription"/>.
    /// </summary>
    /// <param name="typeId">The Id of the problem type, related to <see cref="Problem.TypeId"/>.</param>
    /// <param name="title">The title of the problem detail.</param>
    /// <param name="description">
    ///     A text that describes and explains what the problem is, 
    ///     when it occurs, why it occurs, what can be done about it.
    /// </param>
    /// <param name="status">Http Status Code of the problem type.</param>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="typeId"/>, <paramref name="title"/> or <paramref name="description"/> is null.
    /// </exception>
    public ProblemDetailsDescription(string typeId, string title, string description, HttpStatusCode status)
    {
        TypeId = typeId ?? throw new ArgumentNullException(nameof(typeId));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Status = status;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ProblemDetailsDescription"/>.
    /// </summary>
    /// <param name="typeId">The Id of the problem type, related to <see cref="Problem.TypeId"/>.</param>
    /// <param name="type">The type of the problem.</param>
    /// <param name="title">The title of the problem detail.</param>
    /// <param name="description">
    ///     A text that describes and explains what the problem is, 
    ///     when it occurs, why it occurs, what can be done about it.
    /// </param>
    /// <param name="status">Http Status Code of the problem type.</param>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="typeId"/>, <paramref name="title"/> or <paramref name="description"/> is null.
    /// </exception>
    [JsonConstructor]
    public ProblemDetailsDescription(string typeId, string? type, string title, string description, HttpStatusCode status)
    {
        TypeId = typeId ?? throw new ArgumentNullException(nameof(typeId));
        Type = type;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Status = status;
    }

    /// <summary>
    /// <para>
    ///     The Id of the problem type, related to <see cref="Problem.TypeId"/>.
    /// </para>
    /// <para>
    ///     It is used to define the problem details type, where the code is the last part of the URI.
    ///     It can be a path or an ID.
    /// </para>
    /// </summary>
    public string TypeId { get; }

    /// <summary>
    /// <para>
    ///     A specific type for the problem. It must be a URI.
    /// </para>
    /// </summary>
    public string? Type { get; internal set; }

    /// <summary>
    /// <para>
    ///     The title of the problem detail.
    /// </para>
    /// </summary>
    public string Title { get; internal set; }

    /// <summary>
    /// A text that describes and explains what the problem is,
    /// when it occurs, why it occurs, what can be done about it. 
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Optional, Http Status Code of the problem type.
    /// </summary>
    public HttpStatusCode Status { get; }
}
