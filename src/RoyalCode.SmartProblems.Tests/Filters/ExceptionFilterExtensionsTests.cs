using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoyalCode.SmartProblems.HttpResults;

namespace RoyalCode.SmartProblems.Tests.Filters;

public class ExceptionFilterExtensionsTests
{
    [Fact]
    public void WithExceptionFilter_ReturnsSameBuilder()
    {
        // Arrange
        var builder = new TestEndpointConventionBuilder();

        // Act
        var result = builder.WithExceptionFilter();

        // Assert
        Assert.Same(builder, result);
        Assert.Single(builder.Conventions);
    }

    [Fact]
    public async Task WithExceptionFilter_OnGroup_CatchesExceptionAndReturnsProblemDetails()
    {
        // Arrange
        using var loggerProvider = new TestLoggerProvider();
        await using var app = await CreateAppAsync(loggerProvider, group =>
        {
            group.MapGet("/produto", () =>
            {
                throw new InvalidOperationException("Unexpected product failure");
            });
        });

        var client = app.GetTestClient();

        // Act
        var response = await client.GetAsync("/api/produto");
        var problemDetails = await ReadProblemDetailsAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Contains("Unexpected product failure", problemDetails.Detail);

        Assert.Contains(loggerProvider.Entries, entry =>
            entry.LogLevel == LogLevel.Error
            && entry.Exception is InvalidOperationException
            && entry.Message.Contains(
                "An exception occurred during the execution of an API endpoint",
                StringComparison.Ordinal));
    }

    [Fact]
    public async Task WithExceptionFilter_LogsNestedMatchError_WhenLogLevelIsProvided()
    {
        // Arrange
        using var loggerProvider = new TestLoggerProvider();
        await using var app = await CreateAppAsync(
            loggerProvider,
            group =>
            {
                group.MapGet("/produto", static () =>
                {
                    OkMatch<string> result = Problems.InvalidParameter("Produto invalido", "produto");
                    return result;
                });
            },
            LogLevel.Warning,
            typeof(LoggerCategory));

        var client = app.GetTestClient();

        // Act
        var response = await client.GetAsync("/api/produto");
        var problemDetails = await ReadProblemDetailsAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal("Produto invalido", problemDetails.Detail);

        Assert.Contains(loggerProvider.Entries, entry =>
            entry.CategoryName.EndsWith($".{nameof(LoggerCategory)}", StringComparison.Ordinal)
            && entry.LogLevel == LogLevel.Warning
            && entry.Exception is null
            && entry.Message.Contains("The endpoint result", StringComparison.Ordinal));
    }

    [Fact]
    public async Task WithExceptionFilter_DoesNotLogNestedMatchError_WhenLogLevelIsNone()
    {
        // Arrange
        using var loggerProvider = new TestLoggerProvider();
        await using var app = await CreateAppAsync(loggerProvider, group =>
        {
            group.MapGet("/produto", static () =>
            {
                OkMatch<string> result = Problems.InvalidParameter("Produto invalido", "produto");
                return result;
            });
        });

        var client = app.GetTestClient();

        // Act
        var response = await client.GetAsync("/api/produto");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.DoesNotContain(loggerProvider.Entries, entry =>
            entry.Message.Contains("The endpoint result", StringComparison.Ordinal));
    }

    private static async Task<WebApplication> CreateAppAsync(
        TestLoggerProvider loggerProvider,
        Action<RouteGroupBuilder> configureGroup,
        LogLevel logLevel = LogLevel.None,
        Type? loggerType = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddProblemDetailsDescriptions();
        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddProvider(loggerProvider);
            logging.SetMinimumLevel(LogLevel.Trace);
        });

        var app = builder.Build();

        var group = app.MapGroup("/api")
            .WithExceptionFilter(logLevel, loggerType);

        configureGroup(group);

        await app.StartAsync();
        return app;
    }

    private static async Task<ProblemDetails> ReadProblemDetailsAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(
            json,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        return Assert.IsType<ProblemDetails>(problemDetails);
    }

    private sealed class TestEndpointConventionBuilder : IEndpointConventionBuilder
    {
        public List<Action<EndpointBuilder>> Conventions { get; } = [];

        public void Add(Action<EndpointBuilder> convention)
        {
            Conventions.Add(convention);
        }
    }

    private sealed class TestLoggerProvider : ILoggerProvider
    {
        public ConcurrentQueue<LogEntry> Entries { get; } = new();

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(categoryName, Entries);
        }

        public void Dispose() { }
    }

    private sealed class TestLogger(
        string categoryName,
        ConcurrentQueue<LogEntry> entries) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            entries.Enqueue(new LogEntry(
                categoryName,
                logLevel,
                exception,
                formatter(state, exception)));
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        public void Dispose() { }
    }

    private sealed record LogEntry(
        string CategoryName,
        LogLevel LogLevel,
        Exception? Exception,
        string Message);

    private sealed class LoggerCategory;
}
