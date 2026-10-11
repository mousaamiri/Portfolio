using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Proficiencies;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/proficiencies")]
[OutputCache(Duration = 300, VaryByQueryKeys = ["lang"], Tags = ["proficiencies", "public"])]
public class ProficienciesController(IProficiencyGroupService service) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProficiencyGroupDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await service.GetPublicAsync(language, ct);
        return result.ToOkResult();
    }
}
