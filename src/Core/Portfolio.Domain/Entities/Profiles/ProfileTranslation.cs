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

    public Profile Profile { get; set; } = null!;
}
