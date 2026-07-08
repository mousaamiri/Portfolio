using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Interests;

namespace Portfolio.Application.Interfaces.Services;

public interface IInterestService
{
    Task<Result<IReadOnlyList<InterestDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<InterestDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<InterestDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateInterestRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateInterestRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
