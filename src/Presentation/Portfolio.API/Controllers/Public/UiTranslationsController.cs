using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Portfolio.API.Common;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/ui-translations")]
[OutputCache(Duration = 300, VaryByQueryKeys = ["lang"], Tags = ["ui-translations", "public"])]
public class UiTranslationsController(IUiTranslationService uiTranslationService) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyDictionary<string, string>>>> GetMap(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await uiTranslationService.GetMapAsync(language, ct);
        return result.ToOkResult();
    }
}
