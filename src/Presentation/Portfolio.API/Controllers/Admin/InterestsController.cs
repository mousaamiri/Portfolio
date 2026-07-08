using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Interests;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Admin;

[Route("api/admin/interests")]
public class InterestsController(IInterestService interestService) : AdminControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<InterestDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await interestService.GetAllAsync(language, ct);
        return result.ToOkResult();
    }

    [HttpGet("{id:guid}", Name = "GetAdminInterestById")]
    public async Task<ActionResult<ApiResponse<InterestDto>>> GetById(
        Guid id, [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await interestService.GetByIdAsync(id, language, ct);
        return result.ToOkOrNotFoundResult();
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateInterestRequest request, CancellationToken ct)
    {
        var result = await interestService.CreateAsync(request, ct);
        return result.ToCreatedResult("GetAdminInterestById", id => new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        Guid id, [FromBody] UpdateInterestRequest request, CancellationToken ct)
    {
        var result = await interestService.UpdateAsync(id, request, ct);
        return result.ToOkOrNotFoundResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, CancellationToken ct)
    {
        var result = await interestService.DeleteAsync(id, ct);
        return result.ToOkOrNotFoundResult();
    }
}
