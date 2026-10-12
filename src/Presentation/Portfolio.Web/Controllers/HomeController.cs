using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class HomeController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.ResolveFromRequest(HttpContext, lang);

        // All content comes from Portfolio.API (backed by the database). If the API
        // is unreachable, PortfolioApiClient throws and the global error page renders.
        // All calls are independent, so they run concurrently (one round-trip, not five).
        var profileTask = api.GetProfileAsync(language, cancellationToken);
        var projectsTask = api.GetProjectsAsync(language, cancellationToken);
        var skillsTask = api.GetSkillsAsync(language, cancellationToken);
        var experiencesTask = api.GetExperiencesAsync(language, cancellationToken);
        var educationsTask = api.GetEducationsAsync(language, cancellationToken);
        await Task.WhenAll(profileTask, projectsTask, skillsTask, experiencesTask, educationsTask);

        var profile = profileTask.Result;
        var projects = projectsTask.Result;
        var skills = skillsTask.Result;
        var experiences = experiencesTask.Result;
        var educations = educationsTask.Result;

        var model = new HomeViewModel
        {
            FullName = profile?.FullName ?? string.Empty,
            JobTitle = profile?.JobTitle ?? string.Empty,
            Bio = profile?.Bio ?? string.Empty,
            ResumeUrlEn = profile?.ResumeUrlEn ?? string.Empty,
            ResumeUrlFa = profile?.ResumeUrlFa ?? string.Empty,
            GitHubUrl = profile?.GitHubUrl ?? string.Empty,
            InstagramUrl = profile?.InstagramUrl ?? string.Empty,
            LinkedInUrl = profile?.LinkedInUrl ?? string.Empty,
            TelegramUrl = profile?.TelegramUrl ?? string.Empty,
            LearningTitle = profile?.LearningTitle ?? string.Empty,
            LearningDesc = profile?.LearningDesc ?? string.Empty,
            LearningDate = profile?.LearningDate ?? string.Empty,
            Projects = projects.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Skills = skills.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Experiences = experiences.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Educations = educations.Select(ApiViewModelMapper.ToViewModel).ToList()
        };

        return View(model);
    }
}
