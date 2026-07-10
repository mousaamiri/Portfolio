using Portfolio.Domain.Common;
using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Entities.UiTranslations;

/// <summary>
/// A single localized UI-chrome string (nav/footer/labels/buttons), keyed by a
/// stable dot-notation <see cref="Key"/> per <see cref="Language"/>. Unlike the
/// content entities this is a flat key→value store (no per-key entity), so it
/// does not follow the <c>ITranslatable</c> pattern. English is the inline
/// default in the views; only non-English values are stored here.
/// </summary>
public class UiTranslation : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public Language Language { get; set; }
    public string Value { get; set; } = string.Empty;
}
