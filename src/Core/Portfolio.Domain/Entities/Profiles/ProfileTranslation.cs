using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Profiles;

public class ProfileTranslation : BaseTranslation
{
    public Guid ProfileId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? Tagline { get; set; }
    public string? Bio { get; set; }
    public string? LearningTitle { get; set; }
    public string? LearningDesc { get; set; }

    // About-page hero badges (localized copy, e.g. "Software Engineer" / "3+ Years").
    public string? RoleBadge { get; set; }
    public string? ExperienceBadge { get; set; }
    public string? DegreeBadge { get; set; }
    public string? PortraitAlt { get; set; }

    // Contact-page localized location labels (e.g. "Tehran" / "Iran").
    public string? Location { get; set; }
    public string? Country { get; set; }

    public Profile Profile { get; set; } = null!;
}
