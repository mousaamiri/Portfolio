using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class WorkController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        // Language is resolved server-side (cookie); the page renders in that one
        // language — no client-side EN/FA swap anymore.
        var language = WebLanguage.ResolveFromRequest(HttpContext, lang);
        var projects = await api.GetProjectsAsync(language, cancellationToken);

        var model = projects
            .Select(ApiViewModelMapper.ToViewModel)
            .ToList();

        return View(model);
    }
}
