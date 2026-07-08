using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Testimonials;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class TestimonialRepository(AppDbContext context) : Repository<Testimonial>(context), ITestimonialRepository
{
    public async Task<Testimonial?> GetByIdWithTranslationsAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.Include(x => x.Translations).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Testimonial>> GetAllWithTranslationsAsync(CancellationToken cancellationToken = default)
        => await DbSet.Include(x => x.Translations).OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Testimonial>> GetActiveWithTranslationsAsync(CancellationToken cancellationToken = default)
        => await DbSet.Where(x => x.IsActive).Include(x => x.Translations).OrderBy(x => x.DisplayOrder).ToListAsync(cancellationToken);
}
