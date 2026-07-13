using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Messages;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/messages")]
public class MessagesController(IMessageService messageService) : PublicControllerBase
{
    // Anonymous contact-form submission. Read side is admin-only (no public GET).
    // Rate-limited per IP ("messages" policy) as defense-in-depth; the primary
    // per-visitor limit lives in the Web layer which sees the real client IP.
    [HttpPost]
    [EnableRateLimiting("messages")]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateMessageRequest request, CancellationToken ct)
    {
        var result = await messageService.CreateAsync(request, ct);
        return result.ToOkResult();
    }
}
