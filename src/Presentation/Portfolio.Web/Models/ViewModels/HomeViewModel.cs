namespace Portfolio.Web.Models.ViewModels;

public class HomeViewModel
{
    public string FullName { get; init; } = string.Empty;
    public string JobTitle { get; init; } = string.Empty;
    public string Bio { get; init; } = string.Empty;

    // ── Hero config (populated from the Profile entity via Portfolio.API) ──
    // URLs / copy the view binds into hrefs and the "learning" popover. Social
    // URLs default empty; the view hides icons whose URL is blank.
    public string ResumeUrlEn { get; init; } = string.Empty;
    public string ResumeUrlFa { get; init; } = string.Empty;
    public string GitHubUrl { get; init; } = string.Empty;
    public string InstagramUrl { get; init; } = string.Empty;
    public string LinkedInUrl { get; init; } = string.Empty;
    public string LearningTitle { get; init; } = string.Empty;
    public string LearningDesc { get; init; } = string.Empty;
    public string LearningDate { get; init; } = string.Empty;

    public List<ProjectViewModel> Projects { get; init; } = [];
    public List<SkillViewModel> Skills { get; init; } = [];
    public List<ExperienceViewModel> Experiences { get; init; } = [];
    public List<EducationViewModel> Educations { get; init; } = [];
}
