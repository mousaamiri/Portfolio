using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Principles;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/principles")]
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
