namespace Portfolio.Web.Models.ViewModels;

public class HomeViewModel
{
    public string FullName { get; init; } = string.Empty;
    public string JobTitle { get; init; } = string.Empty;
    public string Bio { get; init; } = string.Empty;
    public List<ProjectViewModel> Projects { get; init; } = [];
    public List<SkillViewModel> Skills { get; init; } = [];
    public List<ExperienceViewModel> Experiences { get; init; } = [];
    public List<EducationViewModel> Educations { get; init; } = [];
}
