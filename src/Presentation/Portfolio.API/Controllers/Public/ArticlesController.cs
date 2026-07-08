using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Articles;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/articles")]
public class ArticlesController(IArticleService articleService) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ArticleDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await articleService.GetPublicAsync(language, ct);
        return result.ToOkResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> GetById(
        Guid id, [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await articleService.GetByIdAsync(id, language, ct);
        return result.ToOkOrNotFoundResult();
    }
}
