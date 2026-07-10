using Microsoft.AspNetCore.Mvc;
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
        // entity. The static site's fabricated CV-education / single-role / Stack are
        // intentionally left empty (no real equivalent) and hidden by the view.
        var model = MockDataService.GetExperiencePageViewModel();

        var profile = await api.GetProfileAsync(language, cancellationToken);
        var experiences = await api.GetExperiencesAsync(language, cancellationToken);
        var metrics = await api.GetImpactMetricsAsync(language, cancellationToken);
        var principles = await api.GetPrinciplesAsync(language, cancellationToken);
        var proficiency = await api.GetProficienciesAsync(language, cancellationToken);

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
