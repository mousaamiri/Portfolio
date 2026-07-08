using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Testimonials;

public class Testimonial : BaseEntity, ITranslatable<TestimonialTranslation>
{
    public string Initials { get; set; } = string.Empty;
    public string AvatarColor { get; set; } = "var(--accent)";
    public int DisplayOrder { get; set; }

    public ICollection<TestimonialTranslation> Translations { get; set; } = [];
}
