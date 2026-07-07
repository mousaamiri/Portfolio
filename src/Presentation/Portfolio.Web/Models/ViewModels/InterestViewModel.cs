namespace Portfolio.Web.Models.ViewModels;

/// <summary>
/// One card in the About "Interests" grid. English label + a lucide icon name;
/// LabelKey drives client-side i18n (translations.js).
/// </summary>
public class InterestViewModel
{
    public string Icon { get; init; } = string.Empty;      // lucide icon name
    public string Label { get; init; } = string.Empty;
    public string LabelKey { get; init; } = string.Empty;  // data-i18n key
}
