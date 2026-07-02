using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Educations;

public class Education : BaseEntity, ITranslatable<EducationTranslation>
{
    public string InstitutionLogo { get; set; } = string.Empty;
    public string InstitutionUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public double? Gpa { get; set; }

    public ICollection<EducationTranslation> Translations { get; set; } = [];
}
