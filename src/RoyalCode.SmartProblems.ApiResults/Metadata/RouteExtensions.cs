using Microsoft.AspNetCore.Mvc;
using RoyalCode.SmartProblems;
using RoyalCode.SmartProblems.Metadata;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Provides extension methods for <see cref="RouteHandlerBuilder"/> to populate endpoint metadata
/// based on custom attributes, such as <see cref="ProduceProblemsAttribute"/>.
/// </summary>
public static class RouteExtensions
{
    /// <summary>
    /// <para>
    ///     Populates the endpoint metadata for the specified <see cref="RouteHandlerBuilder"/> using the metadata
    ///     defined on the type parameter <typeparamref name="T"/>.
    /// </para>
    /// <para>
    ///     This is typically used to add response metadata for problem details based 
    ///     on the <see cref="ProduceProblemsAttribute"/> applied to <typeparamref name="T"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type to inspect for metadata attributes.</typeparam>
    /// <param name="builder">The route handler builder to populate metadata for.</param>
    /// <returns>The same <see cref="RouteHandlerBuilder"/> instance for chaining.</returns>
    public static RouteHandlerBuilder PopulateMetadata<T>(this RouteHandlerBuilder builder)
        => PopulateMetadata(builder, typeof(T));

    /// <summary>
    /// <para>
    ///     Populates the endpoint metadata for the specified <see cref="RouteHandlerBuilder"/> using the metadata
    ///     defined on the provided <paramref name="type"/>. 
    /// </para>
    /// <para>
    ///     If the type is decorated with <see cref="ProduceProblemsAttribute"/>,
    ///     it will add <see cref="ResponseTypeMetadata"/> for each status code specified in the attribute.
    /// </para>
    /// </summary>
    /// <param name="builder">The route handler builder to populate metadata for.</param>
    /// <param name="type">The type to inspect for metadata attributes.</param>
    /// <returns>The same <see cref="RouteHandlerBuilder"/> instance for chaining.</returns>
    public static RouteHandlerBuilder PopulateMetadata(this RouteHandlerBuilder builder, Type type)
    {
        var attr = type.GetCustomAttribute<ProduceProblemsAttribute>();
        if (attr is not null)
        {
            Type responseType = typeof(ProblemDetails);
            string[] content = ["application/problem+json"];

            foreach (var statusCode in attr.GetStatusCodes())
            {
                builder.WithMetadata(new ResponseTypeMetadata(responseType, statusCode, content));
            }
        }
        return builder;
    }
}

