using Portfolio.Domain.Admins;

namespace Portfolio.Application.Interfaces;

public interface IAdminRepository
{
    Task<Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task AddAsync(Admin admin, CancellationToken cancellationToken = default);
    Task UpdateAsync(Admin admin, CancellationToken cancellationToken = default);
}
