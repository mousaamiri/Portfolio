using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Experiences;

public class ExperienceTranslation : BaseTranslation
{
    public Guid ExperienceId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }

    public Experience Experience { get; set; } = null!;
}
