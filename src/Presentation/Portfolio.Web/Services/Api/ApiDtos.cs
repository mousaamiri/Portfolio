namespace Portfolio.Web.Services.Api;

// Web-local mirrors of the Portfolio.API public DTO JSON shapes. Kept here (not
// referenced from the Application layer) so Portfolio.Web stays standalone and
// only depends on the API over HTTP. Each already carries a single resolved
// language (the API flattens translations via ?lang).

public class ProjectApiDto
{
    public Guid Id { get; set; }
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
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Technologies { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class SkillApiDto
{
    public Guid Id { get; set; }
    public string IconUrl { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
}

public class ExperienceApiDto
{
    public Guid Id { get; set; }
    public string CompanyLogo { get; set; } = string.Empty;
    public string CompanyUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
}

public class EducationApiDto
{
    public Guid Id { get; set; }
    public string InstitutionLogo { get; set; } = string.Empty;
    public string InstitutionUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public double? Gpa { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ArticleApiDto
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime PublishDate { get; set; }
    public int ReadTimeMinutes { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? Body { get; set; }
}
