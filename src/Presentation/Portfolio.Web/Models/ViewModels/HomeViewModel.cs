namespace Portfolio.Web.Models.ViewModels;

public class HomeViewModel
{
    public string FullName { get; init; } = string.Empty;
    public string JobTitle { get; init; } = string.Empty;
    public string Bio { get; init; } = string.Empty;

    // ── Hero config (structured, MockDataService-driven) ──
    // NOTE: hero TEXT (name/role/bio/labels) stays English-in-markup with
    // data-i18n keys; translations.js supplies Farsi at runtime. These props
    // are URLs / copy that the view binds into hrefs and the "learning" popover.
    public string ResumeUrlEn { get; init; } = "/resumes/resume-en.pdf";
    public string ResumeUrlFa { get; init; } = "/resumes/resume-fa.pdf";
    public string GitHubUrl { get; init; } = "#";
    public string InstagramUrl { get; init; } = "#";
    public string LinkedInUrl { get; init; } = "#";
    public string LearningTitle { get; init; } = string.Empty;
    public string LearningDesc { get; init; } = string.Empty;
    public string LearningDate { get; init; } = string.Empty;

    public List<ProjectViewModel> Projects { get; init; } = [];
    public List<SkillViewModel> Skills { get; init; } = [];
    public List<ExperienceViewModel> Experiences { get; init; } = [];
    public List<EducationViewModel> Educations { get; init; } = [];
}
