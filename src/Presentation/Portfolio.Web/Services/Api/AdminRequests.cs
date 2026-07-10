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

// ── Skill ──
public class SkillTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
}

public class SkillApiRequest
{
    public string IconUrl { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<SkillTranslationApiRequest> Translations { get; set; } = [];
}

// ── Faq ──
public class FaqTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

public class FaqApiRequest
{
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<FaqTranslationApiRequest> Translations { get; set; } = [];
}

// ── Interest ──
public class InterestTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class InterestApiRequest
{
    public string Icon { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<InterestTranslationApiRequest> Translations { get; set; } = [];
}

// ── TimelineEntry ──
public class TimelineTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class TimelineApiRequest
{
    public string Year { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<TimelineTranslationApiRequest> Translations { get; set; } = [];
}

// ── StatCounter ──
public class StatTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? TipText { get; set; }
    public string? TipAriaLabel { get; set; }
}

public class StatApiRequest
{
    public string Icon { get; set; } = string.Empty;
    public long CountTarget { get; set; }
    public string? Suffix { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<StatTranslationApiRequest> Translations { get; set; } = [];
}

// ── Experience ──
public class ExperienceTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
}

public class ExperienceApiRequest
{
    public string CompanyLogo { get; set; } = string.Empty;
    public string CompanyUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public List<ExperienceTranslationApiRequest> Translations { get; set; } = [];
}

// ── Education ──
public class EducationTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class EducationApiRequest
{
    public string InstitutionLogo { get; set; } = string.Empty;
    public string InstitutionUrl { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public double? Gpa { get; set; }
    public bool IsActive { get; set; } = true;
    public List<EducationTranslationApiRequest> Translations { get; set; } = [];
}

// ── Article ──
public class ArticleTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? Body { get; set; }
}

public class ArticleApiRequest
{
    public string Slug { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime PublishDate { get; set; }
    public int ReadTimeMinutes { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<ArticleTranslationApiRequest> Translations { get; set; } = [];
}

// ── Testimonial ──
public class TestimonialTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class TestimonialApiRequest
{
    public string Initials { get; set; } = string.Empty;
    public string AvatarColor { get; set; } = "var(--accent)";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<TestimonialTranslationApiRequest> Translations { get; set; } = [];
}

// ── ImpactMetric ──
public class ImpactMetricTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ImpactMetricApiRequest
{
    public string Value { get; set; } = string.Empty;
    public string Color { get; set; } = "amber";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<ImpactMetricTranslationApiRequest> Translations { get; set; } = [];
}

// ── Principle ──
public class PrincipleTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class PrincipleApiRequest
{
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<PrincipleTranslationApiRequest> Translations { get; set; } = [];
}

// ── ProficiencyGroup ──
public class ProficiencyTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Items { get; set; } = string.Empty;
}

public class ProficiencyApiRequest
{
    public string Color { get; set; } = "amber";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<ProficiencyTranslationApiRequest> Translations { get; set; } = [];
}

// ── Profile (single upsert) ──
public class ProfileTranslationApiRequest
{
    public string LanguageCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? Tagline { get; set; }
    public string? Bio { get; set; }
    public string? LearningTitle { get; set; }
    public string? LearningDesc { get; set; }
}

public class UpsertProfileApiRequest
{
    public string Email { get; set; } = string.Empty;
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TelegramUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? ResumeUrlEn { get; set; }
    public string? ResumeUrlFa { get; set; }
    public string? PortraitUrl { get; set; }
    public string? LearningDate { get; set; }
    public List<ProfileTranslationApiRequest> Translations { get; set; } = [];
}
