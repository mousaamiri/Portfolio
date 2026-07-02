using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Skills;

public class Skill : BaseEntity, ITranslatable<SkillTranslation>
{
    public string IconUrl { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<SkillTranslation> Translations { get; set; } = [];
}
