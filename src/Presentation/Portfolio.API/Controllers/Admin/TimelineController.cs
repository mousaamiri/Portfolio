using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Timeline;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Admin;

[Route("api/admin/timeline")]
public class TimelineController(ITimelineEntryService timelineService) : AdminControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TimelineEntryDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await timelineService.GetAllAsync(language, ct);
        return result.ToOkResult();
    }

    [HttpGet("{id:guid}", Name = "GetAdminTimelineEntryById")]
    public async Task<ActionResult<ApiResponse<TimelineEntryDto>>> GetById(
        Guid id, [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await timelineService.GetByIdAsync(id, language, ct);
        return result.ToOkOrNotFoundResult();
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateTimelineEntryRequest request, CancellationToken ct)
    {
        var result = await timelineService.CreateAsync(request, ct);
        return result.ToCreatedResult("GetAdminTimelineEntryById", id => new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        Guid id, [FromBody] UpdateTimelineEntryRequest request, CancellationToken ct)
    {
        var result = await timelineService.UpdateAsync(id, request, ct);
        return result.ToOkOrNotFoundResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, CancellationToken ct)
    {
        var result = await timelineService.DeleteAsync(id, ct);
        return result.ToOkOrNotFoundResult();
    }
}
