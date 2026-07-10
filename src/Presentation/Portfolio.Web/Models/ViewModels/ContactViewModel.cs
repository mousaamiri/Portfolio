namespace Portfolio.Web.Models.ViewModels;

/// <summary>
/// Contact page model. Structured config (email/phone/location/socials) is
/// ViewModel-driven; text stays English-in-markup with data-i18n keys for
/// client-side Farsi. The contact FORM posts for real (ContactController.Submit →
/// Portfolio.API Message). The admin-only Messages inbox is a separate concern.
/// </summary>
public class ContactViewModel
{
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;      // raw value copied by the copy-button
    public string Location { get; init; } = string.Empty;   // city / region, e.g. "Tehran"
    public string Country { get; init; } = string.Empty;    // e.g. "Iran"
    public string CountryCode { get; init; } = string.Empty; // 2-letter, e.g. "IR"
    public string GitHubUrl { get; init; } = "#";
    public string LinkedInUrl { get; init; } = "#";
    public string InstagramUrl { get; init; } = "#";
    public string TelegramUrl { get; init; } = "#";
    public string TwitterUrl { get; init; } = "#";
    public string WebsiteUrl { get; init; } = "#";
    public List<FaqItemViewModel> Faqs { get; init; } = [];
}
