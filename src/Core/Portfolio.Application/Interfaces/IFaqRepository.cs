using Portfolio.Domain.Entities.Faqs;

namespace Portfolio.Application.Interfaces;

public interface IFaqRepository : IRepository<Faq>
{
    Task<Faq?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Faq>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Faq>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
