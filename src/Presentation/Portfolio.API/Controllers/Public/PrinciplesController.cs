using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Principles;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/principles")]
[OutputCache(Duration = 300, VaryByQueryKeys = ["lang"], Tags = ["principles", "public"])]
public class PrinciplesController(IPrincipleService service) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PrincipleDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await service.GetPublicAsync(language, ct);
        return result.ToOkResult();
    }
}
