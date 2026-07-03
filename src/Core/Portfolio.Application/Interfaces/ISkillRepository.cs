using Portfolio.Domain.Entities.Skills;

namespace Portfolio.Application.Interfaces;

public interface ISkillRepository : IRepository<Skill>
{
    Task<Skill?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
}
