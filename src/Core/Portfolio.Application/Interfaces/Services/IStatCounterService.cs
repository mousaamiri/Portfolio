using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Stats;

namespace Portfolio.Application.Interfaces.Services;

public interface IStatCounterService
{
    Task<Result<IReadOnlyList<StatCounterDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<StatCounterDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<StatCounterDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateStatCounterRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateStatCounterRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
