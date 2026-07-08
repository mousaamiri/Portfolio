using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Timeline;

namespace Portfolio.Application.Interfaces.Services;

public interface ITimelineEntryService
{
    Task<Result<IReadOnlyList<TimelineEntryDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<TimelineEntryDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<TimelineEntryDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateTimelineEntryRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateTimelineEntryRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
