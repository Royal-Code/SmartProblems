using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace OutraLib.FluentProblems.Documentation;

internal static class ProblemDetailsDescriptionPageHtmlRenderer
{
    public static string Render(ProblemDetailsDescriptionPageModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        StringBuilder builder = new();
        builder.AppendLine("<!DOCTYPE html>");
        builder.AppendLine("<html lang=\"en\">");
        builder.AppendLine("<head>");
        builder.AppendLine("<meta charset=\"utf-8\" />");
        builder.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
        builder.Append("<title>")
            .Append(WebUtility.HtmlEncode(model.PageTitle))
            .AppendLine("</title>");
        builder.AppendLine("<style>");
        builder.AppendLine(GetStyles());
        builder.AppendLine("</style>");
        builder.AppendLine("</head>");
        builder.AppendLine("<body class=\"problem-details-page\">\n<div class=\"problem-details-page__app\" data-page=\"problem-details\">");
        builder.AppendLine("<header class=\"problem-details-page__topbar\">\n<div class=\"problem-details-page__topbar-content\">\n<div class=\"problem-details-page__heading\">\n<p class=\"problem-details-page__eyebrow\">RFC 9457</p>");
        builder.Append("<h1 class=\"problem-details-page__title\">")
            .Append(WebUtility.HtmlEncode(model.PageTitle))
            .AppendLine("</h1>");
        builder.Append("<p class=\"problem-details-page__subtitle\">")
            .Append(WebUtility.HtmlEncode(model.PageDescription))
            .AppendLine("</p>\n</div>\n</div>\n</header>");
        builder.AppendLine("<div class=\"problem-details-page__shell\">\n<aside class=\"problem-details-page__sidebar\" data-navigation=\"problem-types\">\n<div class=\"problem-details-page__sidebar-card\">\n<p class=\"problem-details-page__sidebar-title\">Tipos de problema</p>\n<p class=\"problem-details-page__sidebar-description\">Âncoras para os tipos de problem detail configurados.</p>\n<ul class=\"problem-details-page__nav-list\">");

        foreach (var item in model.Items)
        {
            builder.Append("<li class=\"problem-details-page__nav-item\"><a class=\"problem-details-page__nav-link\" href=\"")
                .Append(WebUtility.HtmlEncode(item.NavigationHref))
                .Append("\"><span class=\"problem-details-page__nav-type\">")
                .Append(WebUtility.HtmlEncode(item.TypeId))
                .Append("</span><span class=\"problem-details-page__nav-status\">")
                .Append(WebUtility.HtmlEncode(item.StatusCode.ToString()))
                .AppendLine("</span></a></li>");
        }

        builder.AppendLine("</ul>\n</div>\n</aside>\n<main class=\"problem-details-page__content\" data-content=\"problem-details\">\n<section class=\"problem-details-page__intro\">\n<div class=\"problem-details-page__intro-content\">\n<p class=\"problem-details-page__intro-title\">Endpoint de documentação</p>");
        builder.Append("<p class=\"problem-details-page__intro-text\">Esta página documenta os tipos de problem detail expostos por esta aplicação. As URIs locais geradas ficam alinhadas com a rota desta página e suas âncoras.</p>");
        builder.Append("<p class=\"problem-details-page__intro-uri\"><span class=\"problem-details-page__meta-label\">URI da página</span><span class=\"problem-details-page__meta-value problem-details-page__meta-value--mono\">")
            .Append(WebUtility.HtmlEncode(model.PageUri))
            .AppendLine("</span></p>\n</div>\n<div class=\"problem-details-page__intro-summary\">\n<span class=\"problem-details-page__summary-value\">")
            .Append(model.Items.Count.ToString())
            .AppendLine("</span>\n<span class=\"problem-details-page__summary-label\">tipos documentados</span>\n</div>\n</section>");

        foreach (var item in model.Items)
        {
            builder.Append("<article id=\"")
                .Append(WebUtility.HtmlEncode(item.SectionId))
                .AppendLine("\" class=\"problem-details-page__card\" data-card=\"problem-details-item\">");
            builder.AppendLine("<header class=\"problem-details-page__card-header\">\n<div class=\"problem-details-page__card-heading\">");
            builder.Append("<p class=\"problem-details-page__card-type\">")
                .Append(WebUtility.HtmlEncode(item.TypeId))
                .AppendLine("</p>");
            builder.Append("<h2 class=\"problem-details-page__card-title\">")
                .Append(WebUtility.HtmlEncode(item.Title))
                .AppendLine("</h2>\n</div>");
            builder.Append("<span class=\"problem-details-page__status-badge ")
                .Append(GetStatusClass(item.StatusCode))
                .Append("\">")
                .Append(WebUtility.HtmlEncode(item.StatusDisplay))
                .AppendLine("</span>\n</header>");
            builder.AppendLine("<div class=\"problem-details-page__meta-grid\">\n<div class=\"problem-details-page__meta-item\">\n<span class=\"problem-details-page__meta-label\">TypeId</span>");
            builder.Append("<span class=\"problem-details-page__meta-value problem-details-page__meta-value--mono\">")
                .Append(WebUtility.HtmlEncode(item.TypeId))
                .AppendLine("</span>\n</div>\n<div class=\"problem-details-page__meta-item\">\n<span class=\"problem-details-page__meta-label\">Tipo</span>");
            builder.Append("<span class=\"problem-details-page__meta-value problem-details-page__meta-value--mono\">")
                .Append(WebUtility.HtmlEncode(item.ResolvedTypeUri))
                .AppendLine("</span>\n</div>\n<div class=\"problem-details-page__meta-item\">\n<span class=\"problem-details-page__meta-label\">Status</span>");
            builder.Append("<span class=\"problem-details-page__meta-value\">")
                .Append(WebUtility.HtmlEncode(item.StatusDisplay))
                .AppendLine("</span>\n</div>\n<div class=\"problem-details-page__meta-item\">\n<span class=\"problem-details-page__meta-label\">Origem</span>");
            builder.Append("<span class=\"problem-details-page__meta-value\">")
                .Append(item.UsesGeneratedTypeUri ? "Gerada a partir da rota da página" : "URI explícita configurada")
                .AppendLine("</span>\n</div>\n</div>");
            builder.Append("<div class=\"problem-details-page__description\"><p>")
                .Append(WebUtility.HtmlEncode(item.Description))
                .AppendLine("</p></div>");
            builder.AppendLine("</article>");
        }

        builder.AppendLine("</main>\n</div>\n</div>\n</body>");
        builder.AppendLine("</html>");

        return builder.ToString();
    }

    private static string GetStatusClass(int statusCode)
    {
        return statusCode switch
        {
            >= 500 => "problem-details-page__status-badge--danger",
            StatusCodes.Status403Forbidden => "problem-details-page__status-badge--severe-warning",
            StatusCodes.Status422UnprocessableEntity => "problem-details-page__status-badge--warning",
            StatusCodes.Status404NotFound => "problem-details-page__status-badge--info",
            _ => "problem-details-page__status-badge--info",
        };
    }

    private static string GetStyles()
    {
        return """
:root {
    color-scheme: light;
    --pd-color-brand-100: #e8f1ff;
    --pd-color-brand-200: #c5dbff;
    --pd-color-brand-300: #0d69fd;
    --pd-color-brand-400: #0b4bb1;
    --pd-color-neutral-100: #ffffff;
    --pd-color-neutral-200: #f0f0f0;
    --pd-color-neutral-300: #bdbdbd;
    --pd-color-text-primary: #212121;
    --pd-color-text-secondary: #666666;
    --pd-color-success-100: #e5ffeb;
    --pd-color-success-200: #bdf2c8;
    --pd-color-success-300: #188931;
    --pd-color-warning-100: #fdf9e6;
    --pd-color-warning-200: #fff3b5;
    --pd-color-warning-300: #ecbe23;
    --pd-color-severe-warning-100: #ffefe5;
    --pd-color-severe-warning-200: #ffceb0;
    --pd-color-severe-warning-300: #ed7124;
    --pd-color-danger-100: #ffdee4;
    --pd-color-danger-200: #ffbdc8;
    --pd-color-danger-300: #da2140;
    --pd-radius-sm: 8px;
    --pd-radius-md: 12px;
    --pd-shadow-card: 0 4px 12px 0 #BDBDBD;
    --pd-space-2: 16px;
    --pd-space-3: 24px;
    --pd-space-4: 32px;
}

* {
    box-sizing: border-box;
}

html {
    scroll-behavior: smooth;
}

body {
    margin: 0;
    background: var(--pd-color-neutral-100);
    color: var(--pd-color-text-primary);
    font-family: Roboto, Arial, Helvetica, sans-serif;
    font-size: 16px;
    font-weight: 400;
    line-height: 24px;
}

a {
    color: var(--pd-color-brand-300);
}

.problem-details-page__app {
    min-height: 100vh;
    background: var(--pd-color-neutral-100);
}

.problem-details-page__topbar {
    position: sticky;
    top: 0;
    z-index: 2;
    background: var(--pd-color-neutral-100);
    border-bottom: 1px solid var(--pd-color-neutral-300);
}

.problem-details-page__topbar-content {
    max-width: 1440px;
    margin: 0 auto;
    padding: 16px 24px;
}

.problem-details-page__heading {
    display: flex;
    flex-direction: column;
    gap: 2px;
}

.problem-details-page__eyebrow {
    margin: 0;
    color: var(--pd-color-brand-300);
    font-size: 12px;
    line-height: 16px;
    font-weight: 400;
    letter-spacing: 0.04em;
    text-transform: uppercase;
}

.problem-details-page__title {
    margin: 0;
    color: var(--pd-color-text-primary);
    font-size: 24px;
    line-height: 32px;
    font-weight: 500;
}

.problem-details-page__subtitle {
    margin: 0;
    color: var(--pd-color-text-secondary);
    font-size: 14px;
    line-height: 16px;
    font-weight: 400;
}

.problem-details-page__shell {
    display: grid;
    grid-template-columns: minmax(240px, 320px) minmax(0, 1fr);
    gap: var(--pd-space-3);
    max-width: 1440px;
    margin: 0 auto;
    padding: var(--pd-space-3);
}

.problem-details-page__sidebar {
    min-width: 0;
}

.problem-details-page__sidebar-card,
.problem-details-page__intro,
.problem-details-page__card {
    background: var(--pd-color-neutral-100);
    border: 1px solid var(--pd-color-neutral-300);
    border-radius: var(--pd-radius-md);
    box-shadow: var(--pd-shadow-card);
}

.problem-details-page__sidebar-card {
    position: sticky;
    top: 89px;
    padding: var(--pd-space-2);
    display: flex;
    flex-direction: column;
    gap: var(--pd-space-2);
    max-height: calc(100vh - 89px - var(--pd-space-3));
    overflow: hidden;
}

.problem-details-page__sidebar-title,
.problem-details-page__intro-title {
    margin: 0;
    color: var(--pd-color-text-primary);
    font-size: 16px;
    line-height: 24px;
    font-weight: 500;
}

.problem-details-page__sidebar-description,
.problem-details-page__intro-text,
.problem-details-page__description p {
    margin: 0;
    color: var(--pd-color-text-secondary);
    font-size: 14px;
    line-height: 16px;
    font-weight: 400;
}

.problem-details-page__nav-list {
    list-style: none;
    margin: 0;
    padding: 0;
    display: flex;
    flex-direction: column;
    gap: 8px;
    min-height: 0;
    overflow-y: auto;
    overflow-x: hidden;
    padding-right: 4px;
    scrollbar-gutter: stable;
}

.problem-details-page__nav-item {
    margin: 0;
}

.problem-details-page__nav-link {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 8px;
    width: 100%;
    padding: 10px 12px;
    border-radius: var(--pd-radius-sm);
    color: var(--pd-color-text-primary);
    text-decoration: none;
    transition: background-color 200ms ease-out, color 200ms ease-out, border-color 200ms ease-out;
}

.problem-details-page__nav-link:hover,
.problem-details-page__nav-link:focus-visible {
    background: var(--pd-color-brand-100);
    color: var(--pd-color-brand-400);
    outline: none;
}

.problem-details-page__nav-type {
    min-width: 0;
    overflow-wrap: anywhere;
    font-size: 14px;
    line-height: 16px;
    font-weight: 400;
}

.problem-details-page__nav-status {
    flex-shrink: 0;
    min-width: 40px;
    padding: 2px 8px;
    border: 1px solid var(--pd-color-brand-200);
    border-radius: var(--pd-radius-sm);
    background: var(--pd-color-brand-100);
    color: var(--pd-color-brand-400);
    font-size: 12px;
    line-height: 16px;
    font-weight: 400;
    text-align: center;
}

.problem-details-page__content {
    min-width: 0;
    display: flex;
    flex-direction: column;
    gap: var(--pd-space-3);
}

.problem-details-page__intro {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    gap: var(--pd-space-2);
    padding: var(--pd-space-3);
}

.problem-details-page__intro-content {
    min-width: 0;
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.problem-details-page__intro-uri {
    display: flex;
    flex-direction: column;
    gap: 8px;
    margin: 0;
}

.problem-details-page__intro-summary {
    flex-shrink: 0;
    display: flex;
    flex-direction: column;
    align-items: flex-end;
    justify-content: center;
    min-width: 120px;
    padding: 16px;
    border: 1px solid var(--pd-color-brand-200);
    border-radius: var(--pd-radius-md);
    background: var(--pd-color-brand-100);
}

.problem-details-page__summary-value {
    color: var(--pd-color-text-primary);
    font-size: 24px;
    line-height: 32px;
    font-weight: 500;
}

.problem-details-page__summary-label {
    color: var(--pd-color-text-secondary);
    font-size: 12px;
    line-height: 16px;
    font-weight: 400;
    letter-spacing: 0.04em;
    text-transform: uppercase;
}

.problem-details-page__card {
    scroll-margin-top: 104px;
    padding: var(--pd-space-3);
    display: flex;
    flex-direction: column;
    gap: var(--pd-space-2);
}

.problem-details-page__card:target {
    border-color: var(--pd-color-brand-200);
    box-shadow: 0 0 0 2px var(--pd-color-brand-100), var(--pd-shadow-card);
}

.problem-details-page__card-header {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    gap: var(--pd-space-2);
}

.problem-details-page__card-heading {
    min-width: 0;
    display: flex;
    flex-direction: column;
    gap: 4px;
}

.problem-details-page__card-type {
    margin: 0;
    color: var(--pd-color-brand-300);
    font-size: 12px;
    line-height: 16px;
    font-weight: 400;
    letter-spacing: 0.04em;
    overflow-wrap: anywhere;
}

.problem-details-page__card-title {
    margin: 0;
    color: var(--pd-color-text-primary);
    font-size: 16px;
    line-height: 24px;
    font-weight: 500;
}

.problem-details-page__status-badge {
    flex-shrink: 0;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    padding: 6px 10px;
    border-radius: var(--pd-radius-sm);
    border: 1px solid transparent;
    font-size: 12px;
    line-height: 16px;
    font-weight: 400;
    text-align: center;
}

.problem-details-page__status-badge--info {
    background: var(--pd-color-brand-100);
    border-color: var(--pd-color-brand-200);
    color: var(--pd-color-brand-400);
}

.problem-details-page__status-badge--warning {
    background: var(--pd-color-warning-100);
    border-color: var(--pd-color-warning-200);
    color: var(--pd-color-text-primary);
}

.problem-details-page__status-badge--severe-warning {
    background: var(--pd-color-severe-warning-100);
    border-color: var(--pd-color-severe-warning-200);
    color: var(--pd-color-text-primary);
}

.problem-details-page__status-badge--danger {
    background: var(--pd-color-danger-100);
    border-color: var(--pd-color-danger-200);
    color: var(--pd-color-danger-300);
}

.problem-details-page__meta-grid {
    display: grid;
    grid-template-columns: repeat(2, minmax(0, 1fr));
    gap: var(--pd-space-2);
}

.problem-details-page__meta-item {
    min-width: 0;
    display: flex;
    flex-direction: column;
    gap: 8px;
    padding: 12px 16px;
    border: 1px solid var(--pd-color-neutral-200);
    border-radius: var(--pd-radius-sm);
    background: var(--pd-color-neutral-100);
}

.problem-details-page__meta-label {
    color: var(--pd-color-text-secondary);
    font-size: 12px;
    line-height: 16px;
    font-weight: 400;
    letter-spacing: 0.04em;
    text-transform: uppercase;
}

.problem-details-page__meta-value {
    color: var(--pd-color-text-primary);
    font-size: 14px;
    line-height: 16px;
    font-weight: 400;
    overflow-wrap: anywhere;
}

.problem-details-page__meta-value--mono {
    font-family: "Roboto Mono", Consolas, "Courier New", monospace;
}

.problem-details-page__description {
    display: flex;
    flex-direction: column;
    gap: 8px;
}

@media (max-width: 991px) {
    .problem-details-page__shell {
        grid-template-columns: minmax(0, 1fr);
    }

    .problem-details-page__sidebar-card {
        position: static;
        max-height: none;
        overflow: visible;
    }

    .problem-details-page__intro {
        flex-direction: column;
    }

    .problem-details-page__intro-summary {
        align-items: flex-start;
        min-width: 0;
        width: 100%;
    }
}

@media (max-width: 767px) {
    .problem-details-page__topbar-content,
    .problem-details-page__shell {
        padding: var(--pd-space-2);
    }

    .problem-details-page__card,
    .problem-details-page__intro {
        padding: var(--pd-space-2);
    }

    .problem-details-page__card-header {
        flex-direction: column;
    }

    .problem-details-page__meta-grid {
        grid-template-columns: minmax(0, 1fr);
    }

    .problem-details-page__nav-link {
        align-items: flex-start;
    }
}
""";
    }
}