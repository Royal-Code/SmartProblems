using OutraLib.FluentProblems.Descriptions;
using System.Net;
using System.Text;

namespace OutraLib.FluentProblems.Documentation;

internal sealed class ProblemDetailsDescriptionPageModel
{
    public required string PageTitle { get; init; }

    public required string PageDescription { get; init; }

    public required string PageUri { get; init; }

    public required IReadOnlyList<ProblemDetailsDescriptionPageItem> Items { get; init; }

    public static ProblemDetailsDescriptionPageModel Create(ProblemDetailsOptions options, string pageUri)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(pageUri))
            throw new ArgumentException("The page URI cannot be null or empty.", nameof(pageUri));

        string effectiveBaseAddress = GetEffectiveBaseAddress(options, pageUri);
        ProblemDetailsDescriptionPageItem[] items = options.Descriptor
            .GetAllDescriptions()
            .Select(description => CreateItem(description, effectiveBaseAddress, options.TypeComplement))
            .OrderBy(item => item.TypeId, StringComparer.Ordinal)
            .ToArray();

        return new ProblemDetailsDescriptionPageModel
        {
            PageTitle = "Detalhes de Problemas",
            PageDescription = "Página de referência para os tipos de problema RFC 9457 configurados nesta aplicação.",
            PageUri = pageUri,
            Items = items,
        };
    }

    private static ProblemDetailsDescriptionPageItem CreateItem(
        ProblemDetailsDescription description,
        string baseAddress,
        string typeComplement)
    {
        string sectionId = description.TypeId;
        string resolvedTypeUri = string.IsNullOrWhiteSpace(description.Type)
            ? $"{baseAddress}{typeComplement}{description.TypeId}"
            : description.Type;

        return new ProblemDetailsDescriptionPageItem
        {
            TypeId = description.TypeId,
            Title = description.Title,
            Description = description.Description,
            StatusCode = (int)description.Status,
            StatusName = FormatStatusName(description.Status),
            StatusDisplay = $"{(int)description.Status} {FormatStatusName(description.Status)}",
            SectionId = sectionId,
            NavigationHref = $"#{sectionId}",
            ResolvedTypeUri = resolvedTypeUri,
            UsesGeneratedTypeUri = string.IsNullOrWhiteSpace(description.Type),
        };
    }

    private static string GetEffectiveBaseAddress(ProblemDetailsOptions options, string pageUri)
    {
        if (string.IsNullOrWhiteSpace(options.BaseAddress)
            || string.Equals(options.BaseAddress, ProblemDetailsOptions.DefaultBaseAddress, StringComparison.Ordinal))
        {
            return pageUri;
        }

        return options.BaseAddress;
    }

    private static string FormatStatusName(HttpStatusCode status)
    {
        ReadOnlySpan<char> value = status.ToString().AsSpan();
        StringBuilder builder = new(value.Length + 8);

        for (int i = 0; i < value.Length; i++)
        {
            char current = value[i];
            if (i > 0 && char.IsUpper(current) && !char.IsUpper(value[i - 1]))
            {
                builder.Append(' ');
            }

            builder.Append(current);
        }

        return builder.ToString();
    }
}

internal sealed class ProblemDetailsDescriptionPageItem
{
    public required string TypeId { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required int StatusCode { get; init; }

    public required string StatusName { get; init; }

    public required string StatusDisplay { get; init; }

    public required string SectionId { get; init; }

    public required string NavigationHref { get; init; }

    public required string ResolvedTypeUri { get; init; }

    public required bool UsesGeneratedTypeUri { get; init; }
}