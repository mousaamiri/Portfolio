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
        var profile = await api.GetProfileAsync(language, cancellationToken);
        var projects = await api.GetProjectsAsync(language, cancellationToken);
        var skills = await api.GetSkillsAsync(language, cancellationToken);
        var experiences = await api.GetExperiencesAsync(language, cancellationToken);
        var educations = await api.GetEducationsAsync(language, cancellationToken);

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
