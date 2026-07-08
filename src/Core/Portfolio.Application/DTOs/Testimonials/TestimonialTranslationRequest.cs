using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Testimonials;

public class TestimonialTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string Quote { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
