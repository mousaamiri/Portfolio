using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Services.Admin;

/// <summary>Maps between the admin Project form/list ViewModels and the API DTOs/requests.</summary>
public static class AdminProjectMapper
{
    public static ProjectListItem ToListItem(ProjectApiDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        Slug = dto.Slug,
        Technologies = dto.Technologies,
        IsPublished = dto.IsPublished,
        IsActive = dto.IsActive
    };

    /// <summary>Builds the bilingual edit form from the EN + FA fetches of a project.</summary>
    public static ProjectFormModel ToFormModel(ProjectApiDto en, ProjectApiDto? fa) => new()
    {
        Id = en.Id,
        Slug = en.Slug,
        ThumbnailUrl = en.ThumbnailUrl,
        CoverImageUrl = en.CoverImageUrl,
        PreviewUrl = en.PreviewUrl,
        SourceCodeUrl = en.SourceCodeUrl,
        IsSourcePrivate = en.IsSourcePrivate,
        IsClientProject = en.IsClientProject,
        IsFeatured = en.IsFeatured,
        IsPublished = en.IsPublished,
        IsActive = en.IsActive,
        StartedAt = en.StartedAt,
        CompletedAt = en.CompletedAt,
        DisplayOrder = en.DisplayOrder,
        TitleEn = en.Title,
        ShortDescriptionEn = en.ShortDescription,
        DescriptionEn = en.Description,
        TechnologiesEn = en.Technologies,
        TitleFa = fa?.Title ?? string.Empty,
        ShortDescriptionFa = fa?.ShortDescription ?? string.Empty,
        DescriptionFa = fa?.Description,
        TechnologiesFa = fa?.Technologies
    };

    public static CreateProjectApiRequest ToCreateRequest(ProjectFormModel m) => new()
    {
        Slug = m.Slug,
        ThumbnailUrl = m.ThumbnailUrl,
        CoverImageUrl = m.CoverImageUrl,
        PreviewUrl = m.PreviewUrl,
        SourceCodeUrl = m.SourceCodeUrl,
        IsSourcePrivate = m.IsSourcePrivate,
        IsClientProject = m.IsClientProject,
        IsFeatured = m.IsFeatured,
        IsPublished = m.IsPublished,
        StartedAt = m.StartedAt,
        CompletedAt = m.CompletedAt,
        DisplayOrder = m.DisplayOrder,
        Translations = BuildTranslations(m)
    };

    public static UpdateProjectApiRequest ToUpdateRequest(ProjectFormModel m) => new()
    {
        Slug = m.Slug,
        ThumbnailUrl = m.ThumbnailUrl,
        CoverImageUrl = m.CoverImageUrl,
        PreviewUrl = m.PreviewUrl,
        SourceCodeUrl = m.SourceCodeUrl,
        IsSourcePrivate = m.IsSourcePrivate,
        IsClientProject = m.IsClientProject,
        IsFeatured = m.IsFeatured,
        IsPublished = m.IsPublished,
        IsActive = m.IsActive,
        StartedAt = m.StartedAt,
        CompletedAt = m.CompletedAt,
        DisplayOrder = m.DisplayOrder,
        Translations = BuildTranslations(m)
    };

    private static List<ProjectTranslationApiRequest> BuildTranslations(ProjectFormModel m)
    {
        var translations = new List<ProjectTranslationApiRequest>
        {
            new()
            {
                LanguageCode = "en",
                Title = m.TitleEn,
                ShortDescription = m.ShortDescriptionEn,
                Description = m.DescriptionEn,
                Technologies = m.TechnologiesEn
            }
        };

        // Only include the Persian translation if it was actually filled in.
        if (!string.IsNullOrWhiteSpace(m.TitleFa))
        {
            translations.Add(new ProjectTranslationApiRequest
            {
                LanguageCode = "fa",
                Title = m.TitleFa,
                ShortDescription = m.ShortDescriptionFa,
                Description = m.DescriptionFa,
                Technologies = m.TechnologiesFa
            });
        }

        return translations;
    }
}
