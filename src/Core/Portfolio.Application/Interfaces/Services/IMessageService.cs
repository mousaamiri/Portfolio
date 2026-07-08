using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Messages;

namespace Portfolio.Application.Interfaces.Services;

public interface IMessageService
{
    Task<Result<Guid>> CreateAsync(CreateMessageRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<MessageDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<MessageDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
