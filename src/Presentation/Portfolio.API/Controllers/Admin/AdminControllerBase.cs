using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;

namespace Portfolio.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
public abstract class AdminControllerBase : ControllerBase;