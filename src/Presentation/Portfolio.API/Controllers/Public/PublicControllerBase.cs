using Microsoft.AspNetCore.Mvc;

namespace Portfolio.API.Controllers.Public;

[ApiController]
[Route("api/public/[controller]")]
public abstract class PublicControllerBase : ControllerBase;