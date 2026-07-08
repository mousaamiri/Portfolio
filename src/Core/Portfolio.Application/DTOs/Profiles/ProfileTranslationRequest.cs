using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Profiles;

public class ProfileTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string JobTitle { get; set; } = string.Empty;
    public string? Tagline { get; set; }
    public string? Bio { get; set; }
    public string? LearningTitle { get; set; }
    public string? LearningDesc { get; set; }
}
