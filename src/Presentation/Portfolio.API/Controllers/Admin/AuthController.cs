using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.API.Common;
using Portfolio.Application.DTOs;
using Portfolio.Application.Services;

namespace Portfolio.API.Controllers.Admin;

[Route("api/admin/auth")]
public class AuthController(AuthService authService) : AdminControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        if (result is null)
            return Unauthorized(ApiResponse.Fail("Invalid username or password."));

        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    [HttpGet("me")]
    public ActionResult<ApiResponse<object>> Me()
    {
        return Ok(ApiResponse<object>.Ok(new { Username = User.Identity?.Name }));
    }
}
