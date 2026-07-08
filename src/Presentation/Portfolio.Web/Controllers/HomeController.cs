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

        // Hero now comes from the Profile entity (Portfolio.API). Until a profile is
        // seeded, fall back to the mock hero so the page still renders (retired once
        // real content is seeded in F1).
        var profile = await api.GetProfileAsync(language, cancellationToken);
        var hero = MockDataService.GetHomeViewModel();

        var projects = await api.GetProjectsAsync(language, cancellationToken);
        var skills = await api.GetSkillsAsync(language, cancellationToken);
        var experiences = await api.GetExperiencesAsync(language, cancellationToken);
        var educations = await api.GetEducationsAsync(language, cancellationToken);

        var model = new HomeViewModel
        {
            FullName = profile?.FullName ?? hero.FullName,
            JobTitle = profile?.JobTitle ?? hero.JobTitle,
            Bio = profile?.Bio ?? hero.Bio,
            ResumeUrlEn = profile?.ResumeUrlEn ?? hero.ResumeUrlEn,
            ResumeUrlFa = profile?.ResumeUrlFa ?? hero.ResumeUrlFa,
            GitHubUrl = profile?.GitHubUrl ?? hero.GitHubUrl,
            InstagramUrl = profile?.InstagramUrl ?? hero.InstagramUrl,
            LinkedInUrl = profile?.LinkedInUrl ?? hero.LinkedInUrl,
            LearningTitle = profile?.LearningTitle ?? hero.LearningTitle,
            LearningDesc = profile?.LearningDesc ?? hero.LearningDesc,
            LearningDate = profile?.LearningDate ?? hero.LearningDate,
            Projects = projects.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Skills = skills.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Experiences = experiences.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Educations = educations.Select(ApiViewModelMapper.ToViewModel).ToList()
        };

        return View(model);
    }
}
