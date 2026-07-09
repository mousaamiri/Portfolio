using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Admins;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class AdminRepository(AppDbContext context) : IAdminRepository
{
    public async Task<Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await context.Admins
            .FirstOrDefaultAsync(a => a.Username == username, cancellationToken);
    }

    public async Task AddAsync(Admin admin, CancellationToken cancellationToken = default)
    {
        await context.Admins.AddAsync(admin, cancellationToken);
    }

    public async Task UpdateAsync(Admin admin, CancellationToken cancellationToken = default)
    {
        context.Admins.Update(admin);
        await context.SaveChangesAsync(cancellationToken);
    }
}
