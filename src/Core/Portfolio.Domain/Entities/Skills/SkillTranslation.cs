using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Skills;

public class SkillTranslation : BaseTranslation
{
    public Guid SkillId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }

    public Skill Skill { get; set; } = null!;
}
