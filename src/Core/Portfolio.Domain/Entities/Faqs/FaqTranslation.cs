using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Faqs;

public class FaqTranslation : BaseTranslation
{
    public Guid FaqId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;

    public Faq Faq { get; set; } = null!;
}
