using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class ExperienceController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.Resolve(lang);

        // Professional history, impact metrics, core principles and the proficiency
        // matrix all come from Portfolio.API. The remaining page content (summary,
        // CV-style education, single-role fallback block) has no backend entity yet
        // and stays mocked. TODO: model those if the owner wants them dynamic.
        var model = MockDataService.GetExperiencePageViewModel();

        var experiences = await api.GetExperiencesAsync(language, cancellationToken);
        var metrics = await api.GetImpactMetricsAsync(language, cancellationToken);
        var principles = await api.GetPrinciplesAsync(language, cancellationToken);
        var proficiency = await api.GetProficienciesAsync(language, cancellationToken);

        model.ProfessionalHistory = experiences.Select(ApiViewModelMapper.ToViewModel).ToList();
        model.Metrics = metrics.Select(ApiViewModelMapper.ToViewModel).ToList();
        model.Principles = principles.Select(ApiViewModelMapper.ToViewModel).ToList();
        model.Proficiency = proficiency.Select(ApiViewModelMapper.ToViewModel).ToList();

        return View(model);
    }
}
