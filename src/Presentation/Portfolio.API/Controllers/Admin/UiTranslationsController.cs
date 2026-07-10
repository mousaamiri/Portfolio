using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.UiTranslations;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Admin;

[Route("api/admin/ui-translations")]
public class UiTranslationsController(IUiTranslationService uiTranslationService) : AdminControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UiTranslationDto>>>> GetAll(CancellationToken ct)
    {
        var result = await uiTranslationService.GetAllAsync(ct);
        return result.ToOkResult();
    }

    [HttpGet("{id:guid}", Name = "GetAdminUiTranslationById")]
    public async Task<ActionResult<ApiResponse<UiTranslationDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await uiTranslationService.GetByIdAsync(id, ct);
        return result.ToOkOrNotFoundResult();
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] UpsertUiTranslationRequest request, CancellationToken ct)
    {
        var result = await uiTranslationService.CreateAsync(request, ct);
        return result.ToCreatedResult("GetAdminUiTranslationById", id => new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        Guid id, [FromBody] UpsertUiTranslationRequest request, CancellationToken ct)
    {
        var result = await uiTranslationService.UpdateAsync(id, request, ct);
        return result.ToOkOrNotFoundResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, CancellationToken ct)
    {
        var result = await uiTranslationService.DeleteAsync(id, ct);
        return result.ToOkOrNotFoundResult();
    }
}
