using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Projects;

public class UpdateProjectRequest
{
    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }

    public string? PreviewUrl { get; set; }
    public string? SourceCodeUrl { get; set; }

    public bool IsSourcePrivate { get; set; }
    public bool IsClientProject { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [MinLength(1)]
    public List<ProjectTranslationRequest> Translations { get; set; } = [];
}
