using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace RoyalCode.SmartProblems.Tests.Http;

public class HttpResultExtensionsTests
{
    [Fact]
    public async Task ToResultAsync_NonProblemDetails_ReadsBody_WhenContentLengthIsUnknown()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new UnknownLengthStringContent("external validation failed")
        };

        // Act
        var result = await response.ToResultAsync();

        // Assert
        Assert.True(result.HasProblems(out var problems));
        Assert.Single(problems);
        Assert.Equal("external validation failed", problems[0].Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problems[0].Category);
    }

    private sealed class UnknownLengthStringContent : HttpContent
    {
        private readonly byte[] content;

        public UnknownLengthStringContent(string content)
        {
            this.content = Encoding.UTF8.GetBytes(content);
            Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            return stream.WriteAsync(content, 0, content.Length);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
