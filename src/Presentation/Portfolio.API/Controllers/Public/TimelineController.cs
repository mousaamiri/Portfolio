using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Timeline;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/timeline")]
public class TimelineController(ITimelineEntryService timelineService) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TimelineEntryDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await timelineService.GetPublicAsync(language, ct);
        return result.ToOkResult();
    }
}
