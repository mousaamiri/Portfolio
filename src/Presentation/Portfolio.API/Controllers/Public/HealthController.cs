using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;

namespace Portfolio.API.Controllers.Public;

public class HealthController : PublicControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<object>> Get()
        => Ok(ApiResponse<object>.Ok(new { Status = "Healthy" }));
}
