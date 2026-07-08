using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Faqs;

public class Faq : BaseEntity, ITranslatable<FaqTranslation>
{
    public int DisplayOrder { get; set; }

    public ICollection<FaqTranslation> Translations { get; set; } = [];
}
