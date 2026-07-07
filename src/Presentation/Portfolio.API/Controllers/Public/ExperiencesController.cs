using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Experiences;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/experiences")]
public class ExperiencesController(IExperienceService experienceService) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ExperienceDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await experienceService.GetPublicAsync(language, ct);
        return result.ToOkResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ExperienceDto>>> GetById(
        Guid id, [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await experienceService.GetByIdAsync(id, language, ct);
        return result.ToOkOrNotFoundResult();
    }
}
