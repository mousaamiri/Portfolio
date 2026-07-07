namespace Portfolio.Web.Models.ViewModels;

/// <summary>One "Impact Metric" card on the Experience page. Value is static
/// text (e.g. "99.9%"); Tag/Desc carry data-i18n keys for client-side Farsi.</summary>
public class ImpactMetricViewModel
{
    public string Value { get; init; } = string.Empty;
    public string Color { get; init; } = "amber";   // pink | amber | green
    public string TagKey { get; init; } = string.Empty;
    public string Tag { get; init; } = string.Empty;
    public string DescKey { get; init; } = string.Empty;
    public string Desc { get; init; } = string.Empty;
}
