using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Interests;

public class Interest : BaseEntity, ITranslatable<InterestTranslation>
{
    public string Icon { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public ICollection<InterestTranslation> Translations { get; set; } = [];
}
