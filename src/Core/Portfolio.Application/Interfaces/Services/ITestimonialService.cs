using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Testimonials;

namespace Portfolio.Application.Interfaces.Services;

public interface ITestimonialService
{
    Task<Result<IReadOnlyList<TestimonialDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<TestimonialDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<TestimonialDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateTestimonialRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateTestimonialRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
