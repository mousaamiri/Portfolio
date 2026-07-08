using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.ImpactMetrics;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Admin;

[Route("api/admin/impact-metrics")]
public class ImpactMetricsController(IImpactMetricService service) : AdminControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ImpactMetricDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await service.GetAllAsync(language, ct);
        return result.ToOkResult();
    }

    [HttpGet("{id:guid}", Name = "GetAdminImpactMetricById")]
    public async Task<ActionResult<ApiResponse<ImpactMetricDto>>> GetById(
        Guid id, [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await service.GetByIdAsync(id, language, ct);
        return result.ToOkOrNotFoundResult();
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateImpactMetricRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return result.ToCreatedResult("GetAdminImpactMetricById", id => new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        Guid id, [FromBody] UpdateImpactMetricRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return result.ToOkOrNotFoundResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, ct);
        return result.ToOkOrNotFoundResult();
    }
}
