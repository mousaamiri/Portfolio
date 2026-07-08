using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Messages;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Public;

[Route("api/public/messages")]
public class MessagesController(IMessageService messageService) : PublicControllerBase
{
    // Anonymous contact-form submission. Read side is admin-only (no public GET).
    // TODO: add rate limiting to guard against spam before production.
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateMessageRequest request, CancellationToken ct)
    {
        var result = await messageService.CreateAsync(request, ct);
        return result.ToOkResult();
    }
}
