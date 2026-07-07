namespace Portfolio.Web.Models.ViewModels;

/// <summary>
/// One testimonial card in the About "Endorsements" row. English text lives
/// here; QuoteKey/NameKey/RoleKey drive client-side i18n (translations.js).
/// </summary>
public class EndorsementViewModel
{
    public string Quote { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Initials { get; init; } = string.Empty;
    public string AvatarColor { get; init; } = "var(--accent)";
    public string QuoteKey { get; init; } = string.Empty;  // data-i18n key
    public string NameKey { get; init; } = string.Empty;   // data-i18n key
    public string RoleKey { get; init; } = string.Empty;   // data-i18n key
}
