using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Profiles;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Admin;

[Route("api/admin/profile")]
public class ProfileController(IProfileService profileService) : AdminControllerBase
{
    // Single-profile upsert: creates the profile on first call, updates thereafter.
    [HttpPut]
    public async Task<ActionResult<ApiResponse<Guid>>> Upsert(
        [FromBody] UpsertProfileRequest request, CancellationToken ct)
    {
        var result = await profileService.UpsertAsync(request, ct);
        return result.ToOkResult();
    }
}
