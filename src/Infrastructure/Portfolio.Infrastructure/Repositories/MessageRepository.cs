using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Messages;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class MessageRepository(AppDbContext context) : Repository<Message>(context), IMessageRepository
{
    public async Task<IReadOnlyList<Message>> GetAllOrderedByNewestAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
