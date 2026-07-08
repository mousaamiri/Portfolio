using Portfolio.Domain.Entities.Messages;

namespace Portfolio.Application.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<IReadOnlyList<Message>> GetAllOrderedByNewestAsync(CancellationToken cancellationToken = default);
}
