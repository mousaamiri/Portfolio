using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Faqs;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/faqs")]
[OutputCache(Duration = 300, VaryByQueryKeys = ["lang"], Tags = ["faqs", "public"])]
public class FaqsController(IFaqService faqService) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<FaqDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await faqService.GetPublicAsync(language, ct);
        return result.ToOkResult();
    }
}
