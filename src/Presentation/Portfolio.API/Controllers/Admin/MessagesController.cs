using Microsoft.AspNetCore.Mvc;
using Portfolio.API.Common;
using Portfolio.Application.DTOs.Messages;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Controllers.Admin;

[Route("api/admin/messages")]
public class MessagesController(IMessageService messageService) : AdminControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MessageDto>>>> GetAll(CancellationToken ct)
    {
        var result = await messageService.GetAllAsync(ct);
        return result.ToOkResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<MessageDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await messageService.GetByIdAsync(id, ct);
        return result.ToOkOrNotFoundResult();
    }

    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(Guid id, CancellationToken ct)
    {
        var result = await messageService.MarkAsReadAsync(id, ct);
        return result.ToOkOrNotFoundResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, CancellationToken ct)
    {
        var result = await messageService.DeleteAsync(id, ct);
        return result.ToOkOrNotFoundResult();
    }
}
