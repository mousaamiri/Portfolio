using Portfolio.Domain.Entities.Testimonials;

namespace Portfolio.Application.Interfaces;

public interface ITestimonialRepository : IRepository<Testimonial>
{
    Task<Testimonial?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Testimonial>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Testimonial>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default);
}
