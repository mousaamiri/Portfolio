using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Proficiencies;

public class ProficiencyGroup : BaseEntity, ITranslatable<ProficiencyGroupTranslation>
{
    public string Color { get; set; } = "amber";   // amber | pink | purple
    public int DisplayOrder { get; set; }

    public ICollection<ProficiencyGroupTranslation> Translations { get; set; } = [];
}
