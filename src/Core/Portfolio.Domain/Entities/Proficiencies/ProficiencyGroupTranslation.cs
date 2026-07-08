using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Proficiencies;

public class ProficiencyGroupTranslation : BaseTranslation
{
    public Guid ProficiencyGroupId { get; set; }
    public string Title { get; set; } = string.Empty;

    // Comma-separated skill names (e.g. "Java 21, Spring Boot, MySQL"). Kept as a
    // single localizable string, mirroring Project.Technologies.
    public string Items { get; set; } = string.Empty;

    public ProficiencyGroup ProficiencyGroup { get; set; } = null!;
}
