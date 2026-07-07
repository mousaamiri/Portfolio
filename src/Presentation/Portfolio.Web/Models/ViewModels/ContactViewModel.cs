namespace Portfolio.Web.Models.ViewModels;

/// <summary>
/// Contact page model. Structured config (email/phone/location/socials) is
/// ViewModel-driven; text stays English-in-markup with data-i18n keys for
/// client-side Farsi. The contact FORM is a pure client-side simulation
/// (contact.js) — there is intentionally NO server POST action or binding here.
/// The admin-only Messages inbox is a separate concern (Step 10), not exposed here.
/// </summary>
public class ContactViewModel
{
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;      // raw value copied by the copy-button
    public string GitHubUrl { get; init; } = "#";
    public string LinkedInUrl { get; init; } = "#";
    public string InstagramUrl { get; init; } = "#";
    public List<FaqItemViewModel> Faqs { get; init; } = [];
}
