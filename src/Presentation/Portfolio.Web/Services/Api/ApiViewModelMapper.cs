using Portfolio.Web.Models.ViewModels;

namespace Portfolio.Web.Services.Api;

/// <summary>
/// Maps the API's single-language DTOs onto the Web ViewModels the Razor views
/// already bind to. Bilingual Work fields (<c>NameEn/NameFa</c>) are populated
/// from whichever language was fetched; the Work page (D2) merges both languages.
/// </summary>
public static class ApiViewModelMapper
{
    public static ProjectViewModel ToViewModel(ProjectApiDto dto) => new()
    {
        // Legacy/simple-card fields
        Title = dto.Title,
        Description = dto.Description,
        Technologies = dto.Technologies,
        ImageUrl = dto.ThumbnailUrl,
        DemoUrl = dto.PreviewUrl,
        SourceCodeUrl = dto.SourceCodeUrl,

        // Work master/detail fields (single language for now)
        DisplayId = dto.DisplayOrder,
        NameEn = dto.Title,
        SubtitleEn = dto.ShortDescription,
        DescriptionEn = dto.Description ?? string.Empty,
        GithubUrl = string.IsNullOrWhiteSpace(dto.SourceCodeUrl) ? "#" : dto.SourceCodeUrl,
        Techs = SplitTechnologies(dto.Technologies)
    };

    /// <summary>
    /// Merges the English and Farsi DTOs for the same project (matched by Id) into
    /// the bilingual <see cref="ProjectViewModel"/> the Work master/detail view and
    /// <c>work.js</c> expect. Farsi falls back to English when a translation is missing.
    /// </summary>
    public static ProjectViewModel MergeToWorkViewModel(ProjectApiDto en, ProjectApiDto? fa) => new()
    {
        DisplayId = en.DisplayOrder,
        NameEn = en.Title,
        NameFa = string.IsNullOrWhiteSpace(fa?.Title) ? en.Title : fa!.Title,
        SubtitleEn = en.ShortDescription,
        SubtitleFa = string.IsNullOrWhiteSpace(fa?.ShortDescription) ? en.ShortDescription : fa!.ShortDescription,
        DescriptionEn = en.Description ?? string.Empty,
        DescriptionFa = string.IsNullOrWhiteSpace(fa?.Description) ? en.Description ?? string.Empty : fa!.Description!,
        GithubUrl = string.IsNullOrWhiteSpace(en.SourceCodeUrl) ? "#" : en.SourceCodeUrl,
        Techs = SplitTechnologies(en.Technologies),

        // Legacy single-language fields kept populated for _ProjectCard compatibility.
        Title = en.Title,
        Description = en.Description,
        Technologies = en.Technologies,
        ImageUrl = en.ThumbnailUrl,
        DemoUrl = en.PreviewUrl,
        SourceCodeUrl = en.SourceCodeUrl
    };

    public static SkillViewModel ToViewModel(SkillApiDto dto) => new()
    {
        Name = dto.Name,
        Category = dto.Category,
        Proficiency = dto.Proficiency,
        IconClass = dto.IconUrl
    };

    public static ExperienceViewModel ToViewModel(ExperienceApiDto dto) => new()
    {
        CompanyName = dto.CompanyName,
        JobTitle = dto.JobTitle,
        Description = dto.Description,
        Location = dto.Location,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate
    };

    public static EducationViewModel ToViewModel(EducationApiDto dto) => new()
    {
        InstitutionName = dto.InstitutionName,
        Degree = dto.Degree,
        FieldOfStudy = dto.FieldOfStudy,
        Description = dto.Description,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Gpa = dto.Gpa,
        Score = dto.Gpa?.ToString()
    };

    public static FaqItemViewModel ToViewModel(FaqApiDto dto) => new()
    {
        Question = dto.Question,
        Answer = dto.Answer
    };

    public static TimelineEntryViewModel ToViewModel(TimelineEntryApiDto dto) => new()
    {
        Year = dto.Year,
        Title = dto.Title,
        Description = dto.Description ?? string.Empty,
        Icon = dto.Icon
    };

    public static InterestViewModel ToViewModel(InterestApiDto dto) => new()
    {
        Icon = dto.Icon,
        Label = dto.Label
    };

    public static StatCounterViewModel ToViewModel(StatCounterApiDto dto) => new()
    {
        Icon = dto.Icon,
        CountTarget = dto.CountTarget,
        Suffix = dto.Suffix,
        Label = dto.Label,
        HasTip = !string.IsNullOrWhiteSpace(dto.TipText),
        TipText = dto.TipText,
        TipAriaLabel = dto.TipAriaLabel
    };

    public static ImpactMetricViewModel ToViewModel(ImpactMetricApiDto dto) => new()
    {
        Value = dto.Value,
        Color = dto.Color,
        Tag = dto.Tag,
        Desc = dto.Description
    };

    public static PrincipleViewModel ToViewModel(PrincipleApiDto dto) => new()
    {
        Title = dto.Title,
        Desc = dto.Description
    };

    public static ProficiencyGroupViewModel ToViewModel(ProficiencyGroupApiDto dto) => new()
    {
        Title = dto.Title,
        Color = dto.Color,
        Items = string.IsNullOrWhiteSpace(dto.Items)
            ? []
            : dto.Items.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
    };

    public static EndorsementViewModel ToViewModel(TestimonialApiDto dto) => new()
    {
        Quote = dto.Quote,
        Name = dto.Name,
        Role = dto.Role,
        Initials = dto.Initials,
        AvatarColor = dto.AvatarColor
    };

    public static BlogPostViewModel ToViewModel(ArticleApiDto dto) => new()
    {
        Id = dto.Slug,
        Title = dto.Title,
        Excerpt = dto.Excerpt ?? string.Empty,
        Category = dto.Category ?? string.Empty,
        Date = dto.PublishDate,
        ReadTime = dto.ReadTimeMinutes
    };

    private static List<TechPillViewModel> SplitTechnologies(string? technologies)
    {
        if (string.IsNullOrWhiteSpace(technologies))
            return [];

        return technologies
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(name => new TechPillViewModel { Name = name, Color = TechColor.For(name) })
            .ToList();
    }
}
