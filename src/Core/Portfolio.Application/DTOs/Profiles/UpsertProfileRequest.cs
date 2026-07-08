using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Profiles;

public class UpsertProfileRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? ResumeUrlEn { get; set; }
    public string? ResumeUrlFa { get; set; }
    public string? PortraitUrl { get; set; }
    public string? LearningDate { get; set; }

    [Required]
    [MinLength(1)]
    public List<ProfileTranslationRequest> Translations { get; set; } = [];
}
