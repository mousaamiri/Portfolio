using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class WorkController(IPortfolioApiClient api, ILogger<WorkController> logger) : Controller
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

        // A 4xx now escalates to the error page, so a blank shell should be
        // unreachable — but a legitimately empty list would still render one. Log it
        // so any future blank render stays greppable in production.
        if (model.Count == 0)
            logger.LogWarning("Work page rendered with no projects for language '{Language}'.", language);

        return View(model);
    }
}
