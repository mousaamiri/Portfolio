using Portfolio.Domain.Entities.Educations;

namespace Portfolio.Application.Interfaces;

public interface IEducationRepository : IRepository<Education>
{
    Task<Education?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
}
