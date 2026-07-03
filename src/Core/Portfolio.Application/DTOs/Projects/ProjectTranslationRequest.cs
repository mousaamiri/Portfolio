using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Projects;

public class ProjectTranslationRequest
{
    [Required]
    public string LanguageCode { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Technologies { get; set; }
}
