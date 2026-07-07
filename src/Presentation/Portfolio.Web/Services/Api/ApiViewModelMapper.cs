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

    private static List<TechPillViewModel> SplitTechnologies(string? technologies)
    {
        if (string.IsNullOrWhiteSpace(technologies))
            return [];

        return technologies
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(name => new TechPillViewModel { Name = name })
            .ToList();
    }
}
