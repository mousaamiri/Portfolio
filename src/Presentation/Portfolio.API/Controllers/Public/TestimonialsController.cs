using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Testimonials;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/testimonials")]
[OutputCache(Duration = 300, VaryByQueryKeys = ["lang"], Tags = ["testimonials", "public"])]
public class TestimonialsController(ITestimonialService service) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TestimonialDto>>>> GetAll(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await service.GetPublicAsync(language, ct);
        return result.ToOkResult();
    }
}
