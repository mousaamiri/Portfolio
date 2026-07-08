using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class BlogController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = Services.WebLanguage.Resolve(lang);

        // Article metadata (server-rendered grid, for SEO/no-JS) now comes from
        // Portfolio.API. NOTE: blog.js still owns the client-side grid re-render and
        // the reading modal from its own hardcoded post array (full HTML bodies).
        // TODO(blog-js): refactor blog.js to fetch the API instead of its own array
        // so the visible grid + modal reflect real articles.
        var articles = await api.GetArticlesAsync(language, cancellationToken);
        var model = articles.Select(ApiViewModelMapper.ToViewModel).ToList();

        return View(model);
    }
}
