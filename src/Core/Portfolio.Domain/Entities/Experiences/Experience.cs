using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Experiences;

public class Experience : BaseEntity, ITranslatable<ExperienceTranslation>
{
    public string CompanyLogo { get; set; } = string.Empty;
    public string CompanyUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ICollection<ExperienceTranslation> Translations { get; set; } = [];
}
