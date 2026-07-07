namespace Portfolio.Web.Models.ViewModels;

/// <summary>
/// One animated count-up stat. Used by the About "Technical Footprint" grid
/// (and reusable for the Experience "Impact Metrics" section). English label +
/// LabelKey for client-side i18n; CountTarget/Suffix drive the JS count-up.
/// </summary>
public class StatCounterViewModel
{
    public string Icon { get; init; } = string.Empty;     // lucide icon name
    public long CountTarget { get; init; }
    public string? Suffix { get; init; }
    public string Label { get; init; } = string.Empty;
    public string LabelKey { get; init; } = string.Empty; // data-i18n key
    public bool HasTip { get; init; }
    public string? TipText { get; init; }                 // tooltip (attr data-tooltip)
    public string? TipAriaLabel { get; init; }
}
