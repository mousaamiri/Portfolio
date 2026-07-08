using Portfolio.Domain.Entities.Proficiencies;

namespace Portfolio.Application.Interfaces;

public interface IProficiencyGroupRepository : IRepository<ProficiencyGroup>
{
    Task<ProficiencyGroup?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProficiencyGroup>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProficiencyGroup>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
