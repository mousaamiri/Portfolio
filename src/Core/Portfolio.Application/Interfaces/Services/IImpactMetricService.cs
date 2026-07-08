using Portfolio.Application.Common;
using Portfolio.Application.DTOs.ImpactMetrics;

namespace Portfolio.Application.Interfaces.Services;

public interface IImpactMetricService
{
    Task<Result<IReadOnlyList<ImpactMetricDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ImpactMetricDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<ImpactMetricDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateImpactMetricRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateImpactMetricRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
