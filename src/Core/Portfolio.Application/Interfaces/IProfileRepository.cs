using Portfolio.Domain.Entities.Profiles;

namespace Portfolio.Application.Interfaces;

public interface IProfileRepository : IRepository<Profile>
{
    Task<Profile?> GetFirstActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<Profile?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
}
