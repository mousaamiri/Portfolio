using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Skills;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/skills")]
public class SkillsController(ISkillService skillService) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SkillDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await skillService.GetPublicAsync(language, ct);
        return result.ToOkResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<SkillDto>>> GetById(
        Guid id, [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await skillService.GetByIdAsync(id, language, ct);
        return result.ToOkOrNotFoundResult();
    }
}
