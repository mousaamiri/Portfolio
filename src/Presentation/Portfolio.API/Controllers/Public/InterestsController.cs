using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Interests;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/interests")]
[OutputCache(Duration = 300, VaryByQueryKeys = ["lang"], Tags = ["interests", "public"])]
public class InterestsController(IInterestService interestService) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<InterestDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await interestService.GetPublicAsync(language, ct);
        return result.ToOkResult();
    }
}
