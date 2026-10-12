using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class ExperienceController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.ResolveFromRequest(HttpContext, lang);

        // Impact metrics, core principles, proficiency matrix and professional history
        // all come from Portfolio.API; the Summary name/text come from the Profile
        // entity. Sections with no real data stay empty and are hidden by the view.
        var model = new ExperiencePageViewModel();

        // All calls are independent, so they run concurrently (one round-trip, not five).
        var profileTask = api.GetProfileAsync(language, cancellationToken);
        var experiencesTask = api.GetExperiencesAsync(language, cancellationToken);
        var metricsTask = api.GetImpactMetricsAsync(language, cancellationToken);
        var principlesTask = api.GetPrinciplesAsync(language, cancellationToken);
        var proficiencyTask = api.GetProficienciesAsync(language, cancellationToken);
        await Task.WhenAll(profileTask, experiencesTask, metricsTask, principlesTask, proficiencyTask);

        var profile = profileTask.Result;
        var experiences = experiencesTask.Result;
        var metrics = metricsTask.Result;
        var principles = principlesTask.Result;
        var proficiency = proficiencyTask.Result;

        if (profile is not null)
        {
            model.SummaryName = profile.FullName;
            model.SummaryText = profile.Bio ?? string.Empty;
        }

        model.ProfessionalHistory = experiences.Select(ApiViewModelMapper.ToViewModel).ToList();
        model.Metrics = metrics.Select(ApiViewModelMapper.ToViewModel).ToList();
        model.Principles = principles.Select(ApiViewModelMapper.ToViewModel).ToList();
        model.Proficiency = proficiency.Select(ApiViewModelMapper.ToViewModel).ToList();

        return View(model);
    }
}
