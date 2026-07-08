using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Testimonials;

public class UpdateTestimonialRequest
{
    public string Initials { get; set; } = string.Empty;
    public string AvatarColor { get; set; } = "var(--accent)";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [MinLength(1)]
    public List<TestimonialTranslationRequest> Translations { get; set; } = [];
}
