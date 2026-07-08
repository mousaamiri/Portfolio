using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Testimonials;

public class TestimonialTranslation : BaseTranslation
{
    public Guid TestimonialId { get; set; }
    public string Quote { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public Testimonial Testimonial { get; set; } = null!;
}
