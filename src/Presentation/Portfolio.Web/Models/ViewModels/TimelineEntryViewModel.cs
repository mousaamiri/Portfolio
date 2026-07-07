namespace Portfolio.Web.Models.ViewModels;

/// <summary>
/// One entry in the About "Journey" timeline. English text lives here; the
/// TitleKey/DescKey drive client-side i18n (translations.js) at runtime.
/// </summary>
public class TimelineEntryViewModel
{
    public string Year { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;      // lucide icon name
    public string TitleKey { get; init; } = string.Empty;  // data-i18n key
    public string DescKey { get; init; } = string.Empty;   // data-i18n key
}
