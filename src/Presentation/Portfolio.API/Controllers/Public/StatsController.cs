using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Stats;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/stats")]
[OutputCache(Duration = 300, VaryByQueryKeys = ["lang"], Tags = ["stats", "public"])]
public class StatsController(IStatCounterService statService) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StatCounterDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await statService.GetPublicAsync(language, ct);
        return result.ToOkResult();
    }
}
