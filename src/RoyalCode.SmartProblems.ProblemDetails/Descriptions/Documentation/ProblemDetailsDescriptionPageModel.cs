using System.Globalization;
using System.Net;
using System.Text;

namespace RoyalCode.SmartProblems.Descriptions.Documentation;

internal sealed class ProblemDetailsDescriptionPageModel
{
    public required string PageTitle { get; init; }

    public required string PageDescription { get; init; }

    public required string PageUri { get; init; }

    public required IReadOnlyList<ProblemDetailsDescriptionPageItem> Items { get; init; }

    public static ProblemDetailsDescriptionPageModel Create(
        ProblemDetailsOptions options,
        string pageUri)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(pageUri))
            throw new ArgumentException("The page URI cannot be null or empty.", nameof(pageUri));

        var baseAddress = GetEffectiveBaseAddress(options, pageUri);
        var typeComplement = options.TypeComplement ?? string.Empty;
        Dictionary<string, int> sectionIds = new(StringComparer.Ordinal);

        var items = options.Descriptor
            .GetAllDescriptions()
            .OrderBy(static description => description.TypeId, StringComparer.Ordinal)
            .Select(description => CreateItem(description, baseAddress, typeComplement, sectionIds))
            .ToArray();

        return new ProblemDetailsDescriptionPageModel
        {
            PageTitle = R.PageTitle,
            PageDescription = R.PageDescription,
            PageUri = pageUri,
            Items = items,
        };
    }

    private static ProblemDetailsDescriptionPageItem CreateItem(
        ProblemDetailsDescription description,
        string baseAddress,
        string typeComplement,
        Dictionary<string, int> sectionIds)
    {
        var usesGeneratedTypeUri = string.IsNullOrWhiteSpace(description.Type);
        var resolvedTypeUri = usesGeneratedTypeUri
            ? $"{baseAddress}{typeComplement}{description.TypeId}"
            : description.Type!;

        var sectionId = CreateSectionId(description.TypeId, sectionIds);

        return new ProblemDetailsDescriptionPageItem
        {
            TypeId = description.TypeId,
            Title = description.Title,
            Description = description.Description,
            StatusCode = (int)description.Status,
            StatusName = FormatStatusName(description.Status),
            StatusDisplay = FormatStatusDisplay(description.Status),
            SectionId = sectionId,
            NavigationHref = $"#{sectionId}",
            ResolvedTypeUri = resolvedTypeUri,
            UsesGeneratedTypeUri = usesGeneratedTypeUri,
            SourceDisplay = usesGeneratedTypeUri
                ? R.ProblemTypeSourceGenerated
                : R.ProblemTypeSourceExplicit,
        };
    }

    private static string GetEffectiveBaseAddress(ProblemDetailsOptions options, string pageUri)
    {
        if (IsDefaultBaseAddress(options.BaseAddress, pageUri))
        {
            return pageUri;
        }

        return options.BaseAddress;
    }

    private static bool IsDefaultBaseAddress(string? baseAddress, string pageUri)
    {
        if (string.IsNullOrWhiteSpace(baseAddress))
            return true;

        if (string.Equals(baseAddress, ProblemDetailsOptions.DefaultBaseAddress, StringComparison.Ordinal))
            return true;

        if (!Uri.TryCreate(baseAddress, UriKind.Absolute, out var baseUri)
            || !Uri.TryCreate(pageUri, UriKind.Absolute, out var page))
        {
            return false;
        }

        return string.Equals(baseUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            && string.Equals(baseUri.Authority, page.Authority, StringComparison.OrdinalIgnoreCase)
            && string.Equals(baseUri.AbsolutePath.TrimEnd('/'), "/.problems", StringComparison.OrdinalIgnoreCase)
            && string.IsNullOrEmpty(baseUri.Query)
            && string.IsNullOrEmpty(baseUri.Fragment);
    }

    private static string FormatStatusDisplay(HttpStatusCode status)
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{(int)status} {FormatStatusName(status)}");
    }

    private static string FormatStatusName(HttpStatusCode status)
    {
        if ((int)status <= 0)
            return R.StatusUnknown;

        var value = status.ToString();
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            return R.StatusUnknown;

        StringBuilder builder = new(value.Length + 8);

        for (var i = 0; i < value.Length; i++)
        {
            var current = value[i];
            if (i > 0 && char.IsUpper(current) && !char.IsUpper(value[i - 1]))
                builder.Append(' ');

            builder.Append(current);
        }

        return builder.ToString();
    }

    private static string CreateSectionId(string typeId, Dictionary<string, int> sectionIds)
    {
        var baseId = SanitizeSectionId(typeId);

        if (!sectionIds.TryGetValue(baseId, out var count))
        {
            sectionIds[baseId] = 1;
            return baseId;
        }

        count++;
        sectionIds[baseId] = count;
        return string.Create(CultureInfo.InvariantCulture, $"{baseId}-{count}");
    }

    private static string SanitizeSectionId(string typeId)
    {
        const string prefix = "problem-";

        StringBuilder builder = new(typeId.Length + prefix.Length);
        builder.Append(prefix);

        var previousWasSeparator = false;
        foreach (var c in typeId)
        {
            if (IsAsciiLetterOrDigit(c))
            {
                builder.Append(char.ToLowerInvariant(c));
                previousWasSeparator = false;
            }
            else if (!previousWasSeparator)
            {
                builder.Append('-');
                previousWasSeparator = true;
            }
        }

        while (builder.Length > prefix.Length && builder[^1] == '-')
            builder.Length--;

        if (builder.Length == prefix.Length)
            builder.Append("type");

        return builder.ToString();
    }

    private static bool IsAsciiLetterOrDigit(char c)
    {
        return c is >= 'a' and <= 'z'
            or >= 'A' and <= 'Z'
            or >= '0' and <= '9';
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

    public required string SourceDisplay { get; init; }
}
