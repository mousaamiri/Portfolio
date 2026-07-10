using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class BlogController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = Services.WebLanguage.ResolveFromRequest(HttpContext, lang);

        // Articles come from Portfolio.API. The server-rendered grid (SEO/no-JS) and
        // the JSON island the view emits both use this data; blog.js reads that island
        // for its client-side grid re-render and reading modal, so the whole page now
        // reflects real articles (empty until the owner writes any).
        var articles = await api.GetArticlesAsync(language, cancellationToken);
        var model = articles.Select(ApiViewModelMapper.ToViewModel).ToList();

        return View(model);
    }
}
