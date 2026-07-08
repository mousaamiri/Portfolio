using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class ExperienceController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.Resolve(lang);

        // Professional history comes from Portfolio.API. Everything else on this
        // page (impact metrics, core principles, summary, CV-style education,
        // proficiency matrix, and the fallback single-role block) has no backend
        // entity yet and stays mocked. TODO(E-phase): model these if needed.
        var model = MockDataService.GetExperiencePageViewModel();

        var experiences = await api.GetExperiencesAsync(language, cancellationToken);
        model.ProfessionalHistory = experiences.Select(ApiViewModelMapper.ToViewModel).ToList();

        return View(model);
    }
}
