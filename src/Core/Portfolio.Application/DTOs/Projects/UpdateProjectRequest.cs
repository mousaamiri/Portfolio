using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Projects;

public class UpdateProjectRequest
{
    [Required]
    public string ImageUrl { get; set; } = string.Empty;
    public string? DemoUrl { get; set; }
    public string? SourceCodeUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [MinLength(1)]
    public List<ProjectTranslationRequest> Translations { get; set; } = [];
}
