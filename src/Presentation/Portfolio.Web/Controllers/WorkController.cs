using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class WorkController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        // Work keeps a client-side EN/FA switch (work.js), so fetch both languages
        // and merge them into the bilingual ProjectViewModel the view serializes.
        var enProjects = await api.GetProjectsAsync("en", cancellationToken);
        var faProjects = await api.GetProjectsAsync("fa", cancellationToken);

        var faById = faProjects.ToDictionary(p => p.Id);

        var model = enProjects
            .Select(en => ApiViewModelMapper.MergeToWorkViewModel(
                en, faById.GetValueOrDefault(en.Id)))
            .ToList();

        return View(model);
    }
}
