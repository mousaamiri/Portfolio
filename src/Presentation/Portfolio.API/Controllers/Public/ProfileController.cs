using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Profiles;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/profile")]
[OutputCache(Duration = 300, VaryByQueryKeys = ["lang"], Tags = ["profile", "public"])]
public class ProfileController(IProfileService profileService) : PublicControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ProfileDto>>> Get(
        [FromQuery] string? lang, CancellationToken ct)
    {
        var language = LanguageResolver.Resolve(lang, Request);
        var result = await profileService.GetPublicAsync(language, ct);
        return result.ToOkOrNotFoundResult();
    }
}
