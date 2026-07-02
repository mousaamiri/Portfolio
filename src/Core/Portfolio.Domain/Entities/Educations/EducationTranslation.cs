using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Educations;

public class EducationTranslation : BaseTranslation
{
    public Guid EducationId { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Education Education { get; set; } = null!;
}
