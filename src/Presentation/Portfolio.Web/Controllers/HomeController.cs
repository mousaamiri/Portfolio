using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class HomeController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.Resolve(lang);

        // TODO(E4): hero copy (name/role/bio/socials/resume/learning) still comes
        // from MockDataService until the Profile entity exists on the backend.
        var hero = MockDataService.GetHomeViewModel();

        var projects = await api.GetProjectsAsync(language, cancellationToken);
        var skills = await api.GetSkillsAsync(language, cancellationToken);
        var experiences = await api.GetExperiencesAsync(language, cancellationToken);
        var educations = await api.GetEducationsAsync(language, cancellationToken);

        var model = new HomeViewModel
        {
            FullName = hero.FullName,
            JobTitle = hero.JobTitle,
            Bio = hero.Bio,
            ResumeUrlEn = hero.ResumeUrlEn,
            ResumeUrlFa = hero.ResumeUrlFa,
            GitHubUrl = hero.GitHubUrl,
            InstagramUrl = hero.InstagramUrl,
            LinkedInUrl = hero.LinkedInUrl,
            LearningTitle = hero.LearningTitle,
            LearningDesc = hero.LearningDesc,
            LearningDate = hero.LearningDate,
            Projects = projects.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Skills = skills.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Experiences = experiences.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Educations = educations.Select(ApiViewModelMapper.ToViewModel).ToList()
        };

        return View(model);
    }
}
