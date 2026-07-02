using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize]
public abstract class AdminControllerBase : ControllerBase;
