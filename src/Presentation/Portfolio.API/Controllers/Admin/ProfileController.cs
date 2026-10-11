using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Caching;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Profiles;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Admin;

[Route("api/admin/profile")]
public class ProfileController(IProfileService profileService) : AdminControllerBase
{
    // Single-profile upsert: creates the profile on first call, updates thereafter.
    // The profile is embedded on every page (hero, footer, contact), so a change
    // purges every cached response site-wide rather than one resource bucket.
    [InvalidateCache(InvalidateAll = true)]
    [HttpPut]
    public async Task<ActionResult<ApiResponse<Guid>>> Upsert(
        [FromBody] UpsertProfileRequest request, CancellationToken ct)
    {
        var result = await profileService.UpsertAsync(request, ct);
        return result.ToOkResult();
    }
}
