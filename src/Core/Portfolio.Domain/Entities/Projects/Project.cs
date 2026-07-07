using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Projects;

public class Project : BaseEntity, ITranslatable<ProjectTranslation>
{
    public string Slug { get; set; } = string.Empty;

    // Images
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }

    // Links
    public string? PreviewUrl { get; set; }
    public string? SourceCodeUrl { get; set; }

    // Source / project type flags
    public bool IsSourcePrivate { get; set; }
    public bool IsClientProject { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }

    // Dates
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public int DisplayOrder { get; set; }

    public ICollection<ProjectTranslation> Translations { get; set; } = [];
}
