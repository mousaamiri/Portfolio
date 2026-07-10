using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Profiles;

/// <summary>
/// Singleton-style profile powering the Home hero and About bio. Language-neutral
/// contact/links live here; localized name/role/bio live on <see cref="ProfileTranslation"/>.
/// The public API returns the first active profile.
/// </summary>
public class Profile : BaseEntity, ITranslatable<ProfileTranslation>
{
    public string Email { get; set; } = string.Empty;
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TelegramUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? ResumeUrlEn { get; set; }
    public string? ResumeUrlFa { get; set; }
    public string? PortraitUrl { get; set; }
    public string? LearningDate { get; set; }

    public ICollection<ProfileTranslation> Translations { get; set; } = [];
}
