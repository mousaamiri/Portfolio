using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Testimonials;

public class CreateTestimonialRequest
{
    public string Initials { get; set; } = string.Empty;
    public string AvatarColor { get; set; } = "var(--accent)";
    public int DisplayOrder { get; set; }

    [Required]
    [MinLength(1)]
    public List<TestimonialTranslationRequest> Translations { get; set; } = [];
}
