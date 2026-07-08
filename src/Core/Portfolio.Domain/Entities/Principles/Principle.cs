using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Principles;

public class Principle : BaseEntity, ITranslatable<PrincipleTranslation>
{
    public int DisplayOrder { get; set; }

    public ICollection<PrincipleTranslation> Translations { get; set; } = [];
}
