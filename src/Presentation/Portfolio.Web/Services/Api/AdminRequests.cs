namespace Portfolio.Web.Services.Api;

// Web-local mirrors of the Portfolio.API admin write-request shapes. Posted as JSON
// to api/admin/*; kept here so Portfolio.Web stays standalone (HTTP only).

public class ProjectTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Technologies { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class CreateProjectApiRequest
{
    public string Slug { get; set; } = string.Empty;
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
    public List<ProjectTranslationApiRequest> Translations { get; set; } = [];
}

public class UpdateProjectApiRequest : CreateProjectApiRequest
{
    public bool IsActive { get; set; }
}
