using Microsoft.AspNetCore.Mvc;

namespace Portfolio.API.Controllers.Public;

public class HealthController : PublicControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { Status = "Healthy" });
}
