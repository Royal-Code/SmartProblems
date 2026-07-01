using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace RoyalCode.SmartProblems.Descriptions.Documentation;

internal static class ProblemDetailsDescriptionPageHtmlRenderer
{
    public static string Render(ProblemDetailsDescriptionPageModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        StringBuilder builder = new();

        builder.AppendLine("<!DOCTYPE html>");
        builder.Append("<html lang=\"")
            .Append(Html(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName))
            .AppendLine("\">");
        builder.AppendLine("<head>");
        builder.AppendLine("<meta charset=\"utf-8\" />");
        builder.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
        builder.Append("<title>")
            .Append(Html(model.PageTitle))
            .AppendLine("</title>");
        builder.AppendLine("<style>");
        builder.AppendLine(GetStyles());
        builder.AppendLine("</style>");
        builder.AppendLine("</head>");
        builder.AppendLine("<body>");
        builder.AppendLine("<div class=\"spdoc-page\" data-page=\"smart-problems\">");

        AppendHeader(builder, model);
        AppendShell(builder, model);

        builder.AppendLine("</div>");
        builder.AppendLine("</body>");
        builder.AppendLine("</html>");

        return builder.ToString();
    }

    private static void AppendHeader(StringBuilder builder, ProblemDetailsDescriptionPageModel model)
    {
        builder.AppendLine("<header class=\"spdoc-header\">");
        builder.AppendLine("<div class=\"spdoc-header__inner\">");
        builder.Append("<p class=\"spdoc-kicker\">")
            .Append(Html(R.RfcLabel))
            .AppendLine("</p>");
        builder.Append("<h1 class=\"spdoc-title\">")
            .Append(Html(model.PageTitle))
            .AppendLine("</h1>");
        builder.Append("<p class=\"spdoc-subtitle\">")
            .Append(Html(model.PageDescription))
            .AppendLine("</p>");
        builder.AppendLine("</div>");
        builder.AppendLine("</header>");
    }

    private static void AppendShell(StringBuilder builder, ProblemDetailsDescriptionPageModel model)
    {
        builder.AppendLine("<div class=\"spdoc-shell\">");
        AppendNavigation(builder, model);
        AppendContent(builder, model);
        builder.AppendLine("</div>");
    }

    private static void AppendNavigation(StringBuilder builder, ProblemDetailsDescriptionPageModel model)
    {
        builder.AppendLine("<aside class=\"spdoc-nav\" aria-label=\"Problem detail navigation\">");
        builder.AppendLine("<div class=\"spdoc-nav__panel\">");
        builder.Append("<h2 class=\"spdoc-nav__title\">")
            .Append(Html(R.NavigationTitle))
            .AppendLine("</h2>");
        builder.Append("<p class=\"spdoc-nav__description\">")
            .Append(Html(R.NavigationDescription))
            .AppendLine("</p>");
        builder.AppendLine("<ol class=\"spdoc-nav__list\">");

        foreach (var item in model.Items)
        {
            builder.Append("<li><a class=\"spdoc-nav__link\" href=\"")
                .Append(Html(item.NavigationHref))
                .Append("\"><span class=\"spdoc-nav__name\">")
                .Append(Html(item.TypeId))
                .Append("</span><span class=\"spdoc-nav__code\">")
                .Append(Html(item.StatusCode.ToString(CultureInfo.InvariantCulture)))
                .AppendLine("</span></a></li>");
        }

        builder.AppendLine("</ol>");
        builder.AppendLine("</div>");
        builder.AppendLine("</aside>");
    }

    private static void AppendContent(StringBuilder builder, ProblemDetailsDescriptionPageModel model)
    {
        builder.AppendLine("<main class=\"spdoc-main\">");
        AppendIntro(builder, model);

        foreach (var item in model.Items)
        {
            AppendItem(builder, item);
        }

        builder.AppendLine("</main>");
    }

    private static void AppendIntro(StringBuilder builder, ProblemDetailsDescriptionPageModel model)
    {
        builder.AppendLine("<section class=\"spdoc-intro\" aria-labelledby=\"spdoc-intro-title\">");
        builder.AppendLine("<div class=\"spdoc-intro__copy\">");
        builder.Append("<h2 id=\"spdoc-intro-title\" class=\"spdoc-intro__title\">")
            .Append(Html(R.IntroTitle))
            .AppendLine("</h2>");
        builder.Append("<p class=\"spdoc-intro__text\">")
            .Append(Html(R.IntroText))
            .AppendLine("</p>");
        builder.Append("<p class=\"spdoc-intro__uri\"><span>")
            .Append(Html(R.PageUriLabel))
            .Append("</span><code>")
            .Append(Html(model.PageUri))
            .AppendLine("</code></p>");
        builder.AppendLine("</div>");
        builder.AppendLine("<div class=\"spdoc-count\" aria-label=\"Documented problem detail types\">");
        builder.Append("<strong>")
            .Append(Html(model.Items.Count.ToString(CultureInfo.InvariantCulture)))
            .AppendLine("</strong>");
        builder.Append("<span>")
            .Append(Html(R.DocumentedTypesLabel))
            .AppendLine("</span>");
        builder.AppendLine("</div>");
        builder.AppendLine("</section>");
    }

    private static void AppendItem(StringBuilder builder, ProblemDetailsDescriptionPageItem item)
    {
        builder.Append("<article id=\"")
            .Append(Html(item.SectionId))
            .AppendLine("\" class=\"spdoc-card\">");
        builder.AppendLine("<header class=\"spdoc-card__header\">");
        builder.AppendLine("<div class=\"spdoc-card__heading\">");
        builder.Append("<p class=\"spdoc-card__type\">")
            .Append(Html(item.TypeId))
            .AppendLine("</p>");
        builder.Append("<h2 class=\"spdoc-card__title\">")
            .Append(Html(item.Title))
            .AppendLine("</h2>");
        builder.AppendLine("</div>");
        builder.Append("<span class=\"spdoc-status ")
            .Append(GetStatusClass(item.StatusCode))
            .Append("\">")
            .Append(Html(item.StatusDisplay))
            .AppendLine("</span>");
        builder.AppendLine("</header>");

        builder.AppendLine("<dl class=\"spdoc-meta\">");
        AppendMeta(builder, R.TypeIdLabel, item.TypeId, mono: true);
        AppendMeta(builder, R.TypeUriLabel, item.ResolvedTypeUri, mono: true);
        AppendMeta(builder, R.StatusLabel, item.StatusDisplay, mono: false);
        AppendMeta(builder, R.SourceLabel, item.SourceDisplay, mono: false);
        builder.AppendLine("</dl>");

        builder.Append("<p class=\"spdoc-card__description\">")
            .Append(Html(item.Description))
            .AppendLine("</p>");
        builder.AppendLine("</article>");
    }

    private static void AppendMeta(StringBuilder builder, string label, string value, bool mono)
    {
        builder.AppendLine("<div class=\"spdoc-meta__item\">");
        builder.Append("<dt>")
            .Append(Html(label))
            .AppendLine("</dt>");
        builder.Append("<dd");
        if (mono)
            builder.Append(" class=\"spdoc-mono\"");
        builder.Append(">")
            .Append(Html(value))
            .AppendLine("</dd>");
        builder.AppendLine("</div>");
    }

    private static string Html(string value)
    {
        return WebUtility.HtmlEncode(value);
    }

    private static string GetStatusClass(int statusCode)
    {
        return statusCode switch
        {
            >= StatusCodes.Status500InternalServerError => "spdoc-status--danger",
            StatusCodes.Status403Forbidden => "spdoc-status--blocked",
            StatusCodes.Status409Conflict => "spdoc-status--warning",
            StatusCodes.Status422UnprocessableEntity => "spdoc-status--warning",
            >= StatusCodes.Status400BadRequest => "spdoc-status--client",
            >= StatusCodes.Status300MultipleChoices => "spdoc-status--redirect",
            >= StatusCodes.Status200OK => "spdoc-status--success",
            _ => "spdoc-status--unknown",
        };
    }

    private static string GetStyles()
    {
        return """
:root {
    color-scheme: light;
    --spdoc-bg: #f7f6f2;
    --spdoc-bg-soft: #fbfaf7;
    --spdoc-surface: #ffffff;
    --spdoc-surface-alt: #f2f7f6;
    --spdoc-border: #dedbd2;
    --spdoc-border-strong: #c5c0b5;
    --spdoc-text: #20231f;
    --spdoc-text-muted: #646861;
    --spdoc-accent: #117c72;
    --spdoc-accent-dark: #075b54;
    --spdoc-accent-soft: #dff1ee;
    --spdoc-blue-soft: #e5eefb;
    --spdoc-blue: #295c9f;
    --spdoc-yellow-soft: #fff2c6;
    --spdoc-yellow: #85630b;
    --spdoc-orange-soft: #ffe5d3;
    --spdoc-orange: #9a4917;
    --spdoc-red-soft: #ffe0e4;
    --spdoc-red: #a82338;
    --spdoc-green-soft: #def3e4;
    --spdoc-green: #276b38;
    --spdoc-shadow: 0 18px 48px rgba(36, 35, 31, 0.08);
    --spdoc-radius: 8px;
}

* {
    box-sizing: border-box;
}

html {
    scroll-behavior: smooth;
}

body {
    margin: 0;
    color: var(--spdoc-text);
    background:
        radial-gradient(circle at top left, rgba(17, 124, 114, 0.10), transparent 34rem),
        linear-gradient(180deg, var(--spdoc-bg-soft), var(--spdoc-bg));
    font-family: "Segoe UI Variable", "Segoe UI", "Noto Sans", Arial, sans-serif;
    font-size: 16px;
    line-height: 1.5;
}

a {
    color: inherit;
}

.spdoc-page {
    min-height: 100vh;
}

.spdoc-header {
    border-bottom: 1px solid var(--spdoc-border);
    background: rgba(251, 250, 247, 0.92);
    backdrop-filter: blur(14px);
}

.spdoc-header__inner {
    width: min(1180px, calc(100% - 32px));
    margin: 0 auto;
    padding: 34px 0 28px;
}

.spdoc-kicker,
.spdoc-card__type {
    margin: 0;
    color: var(--spdoc-accent-dark);
    font-size: 0.76rem;
    font-weight: 700;
    letter-spacing: 0.08em;
    text-transform: uppercase;
}

.spdoc-title {
    margin: 6px 0 0;
    max-width: 860px;
    font-size: clamp(2rem, 4vw, 3.2rem);
    line-height: 1.05;
    font-weight: 650;
    letter-spacing: 0;
}

.spdoc-subtitle {
    margin: 14px 0 0;
    max-width: 760px;
    color: var(--spdoc-text-muted);
    font-size: 1.04rem;
}

.spdoc-shell {
    width: min(1180px, calc(100% - 32px));
    margin: 0 auto;
    padding: 28px 0 56px;
    display: grid;
    grid-template-columns: minmax(220px, 280px) minmax(0, 1fr);
    gap: 28px;
    align-items: start;
}

.spdoc-nav {
    position: sticky;
    top: 20px;
}

.spdoc-nav__panel,
.spdoc-intro,
.spdoc-card {
    border: 1px solid var(--spdoc-border);
    border-radius: var(--spdoc-radius);
    background: rgba(255, 255, 255, 0.88);
    box-shadow: var(--spdoc-shadow);
}

.spdoc-nav__panel {
    padding: 18px;
    max-height: calc(100vh - 40px);
    overflow: hidden;
}

.spdoc-nav__title,
.spdoc-intro__title {
    margin: 0;
    font-size: 1rem;
    font-weight: 700;
}

.spdoc-nav__description,
.spdoc-intro__text,
.spdoc-card__description {
    margin: 8px 0 0;
    color: var(--spdoc-text-muted);
}

.spdoc-nav__list {
    list-style: none;
    margin: 18px 0 0;
    padding: 0;
    display: grid;
    gap: 6px;
    max-height: calc(100vh - 168px);
    overflow-y: auto;
}

.spdoc-nav__link {
    display: grid;
    grid-template-columns: minmax(0, 1fr) auto;
    gap: 10px;
    align-items: center;
    padding: 9px 10px;
    border-radius: 7px;
    color: var(--spdoc-text);
    text-decoration: none;
}

.spdoc-nav__link:hover,
.spdoc-nav__link:focus-visible {
    color: var(--spdoc-accent-dark);
    background: var(--spdoc-accent-soft);
    outline: 2px solid transparent;
}

.spdoc-nav__name {
    overflow-wrap: anywhere;
    font-size: 0.92rem;
}

.spdoc-nav__code {
    min-width: 2.6rem;
    padding: 1px 7px;
    border: 1px solid var(--spdoc-border);
    border-radius: 999px;
    color: var(--spdoc-text-muted);
    font-family: "Cascadia Code", Consolas, monospace;
    font-size: 0.78rem;
    text-align: center;
}

.spdoc-main {
    min-width: 0;
    display: grid;
    gap: 18px;
}

.spdoc-intro {
    padding: 22px;
    display: flex;
    justify-content: space-between;
    gap: 20px;
}

.spdoc-intro__copy {
    min-width: 0;
}

.spdoc-intro__uri {
    margin: 16px 0 0;
    display: grid;
    gap: 5px;
}

.spdoc-intro__uri span,
.spdoc-meta dt {
    color: var(--spdoc-text-muted);
    font-size: 0.72rem;
    font-weight: 700;
    letter-spacing: 0.08em;
    text-transform: uppercase;
}

.spdoc-intro__uri code,
.spdoc-mono {
    font-family: "Cascadia Code", Consolas, "Liberation Mono", monospace;
}

.spdoc-intro__uri code {
    overflow-wrap: anywhere;
    color: var(--spdoc-accent-dark);
}

.spdoc-count {
    min-width: 140px;
    align-self: stretch;
    padding: 16px;
    border: 1px solid var(--spdoc-accent-soft);
    border-radius: var(--spdoc-radius);
    background: var(--spdoc-surface-alt);
    display: grid;
    align-content: center;
    justify-items: end;
}

.spdoc-count strong {
    font-size: 2rem;
    line-height: 1;
}

.spdoc-count span {
    margin-top: 5px;
    color: var(--spdoc-text-muted);
    font-size: 0.76rem;
    font-weight: 700;
    letter-spacing: 0.08em;
    text-transform: uppercase;
}

.spdoc-card {
    scroll-margin-top: 24px;
    padding: 22px;
}

.spdoc-card:target {
    border-color: var(--spdoc-accent);
    box-shadow: 0 0 0 3px var(--spdoc-accent-soft), var(--spdoc-shadow);
}

.spdoc-card__header {
    display: flex;
    justify-content: space-between;
    gap: 18px;
    align-items: flex-start;
}

.spdoc-card__heading {
    min-width: 0;
}

.spdoc-card__title {
    margin: 4px 0 0;
    font-size: 1.35rem;
    line-height: 1.2;
    font-weight: 700;
    overflow-wrap: anywhere;
}

.spdoc-status {
    flex: 0 0 auto;
    padding: 6px 10px;
    border-radius: 999px;
    border: 1px solid transparent;
    font-family: "Cascadia Code", Consolas, monospace;
    font-size: 0.78rem;
    font-weight: 700;
    white-space: nowrap;
}

.spdoc-status--success {
    color: var(--spdoc-green);
    background: var(--spdoc-green-soft);
}

.spdoc-status--redirect,
.spdoc-status--client {
    color: var(--spdoc-blue);
    background: var(--spdoc-blue-soft);
}

.spdoc-status--warning,
.spdoc-status--blocked {
    color: var(--spdoc-yellow);
    background: var(--spdoc-yellow-soft);
}

.spdoc-status--danger {
    color: var(--spdoc-red);
    background: var(--spdoc-red-soft);
}

.spdoc-status--unknown {
    color: var(--spdoc-orange);
    background: var(--spdoc-orange-soft);
}

.spdoc-meta {
    margin: 20px 0 0;
    display: grid;
    grid-template-columns: repeat(2, minmax(0, 1fr));
    gap: 10px;
}

.spdoc-meta__item {
    min-width: 0;
    padding: 12px;
    border: 1px solid var(--spdoc-border);
    border-radius: 7px;
    background: var(--spdoc-bg-soft);
}

.spdoc-meta dt,
.spdoc-meta dd {
    margin: 0;
}

.spdoc-meta dd {
    margin-top: 5px;
    overflow-wrap: anywhere;
}

@media (max-width: 900px) {
    .spdoc-shell {
        grid-template-columns: minmax(0, 1fr);
    }

    .spdoc-nav {
        position: static;
    }

    .spdoc-nav__panel {
        max-height: none;
    }

    .spdoc-nav__list {
        max-height: none;
    }
}

@media (max-width: 640px) {
    .spdoc-header__inner,
    .spdoc-shell {
        width: min(100% - 24px, 1180px);
    }

    .spdoc-intro,
    .spdoc-card__header {
        flex-direction: column;
    }

    .spdoc-count {
        width: 100%;
        justify-items: start;
    }

    .spdoc-meta {
        grid-template-columns: minmax(0, 1fr);
    }
}
""";
    }
}
