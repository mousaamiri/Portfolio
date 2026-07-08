using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Faqs;

namespace Portfolio.Application.Interfaces.Services;

public interface IFaqService
{
    Task<Result<IReadOnlyList<FaqDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<FaqDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<FaqDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateFaqRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateFaqRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
