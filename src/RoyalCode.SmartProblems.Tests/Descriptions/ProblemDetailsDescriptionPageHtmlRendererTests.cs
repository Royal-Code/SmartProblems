using System.Globalization;
using System.Net;
using RoyalCode.SmartProblems.Descriptions;
using RoyalCode.SmartProblems.Descriptions.Documentation;

namespace RoyalCode.SmartProblems.Tests.Descriptions;

public class ProblemDetailsDescriptionPageHtmlRendererTests
{
    [Fact]
    public void Render_EncodesDescriptionValues()
    {
        // Arrange
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription(
            "unsafe",
            "<Title>",
            "<script>alert(1)</script>",
            HttpStatusCode.BadRequest));

        var model = ProblemDetailsDescriptionPageModel.Create(options, "https://example.test/.problems");

        // Act
        var html = ProblemDetailsDescriptionPageHtmlRenderer.Render(model);

        // Assert
        Assert.Contains("&lt;Title&gt;", html);
        Assert.Contains("&lt;script&gt;alert(1)&lt;/script&gt;", html);
        Assert.DoesNotContain("<script>alert(1)</script>", html);
    }

    [Fact]
    public void Render_UsesCurrentUiCultureResources()
    {
        // Arrange
        var previousCulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
            var options = new ProblemDetailsOptions();
            var model = ProblemDetailsDescriptionPageModel.Create(options, "https://example.test/.problems");

            // Act
            var html = ProblemDetailsDescriptionPageHtmlRenderer.Render(model);

            // Assert
            Assert.Contains("Detalhes de problemas", html);
            Assert.Contains("tipos documentados", html);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }
}
