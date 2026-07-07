using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Experiences;

namespace Portfolio.Application.Interfaces.Services;

public interface IExperienceService
{
    Task<Result<IReadOnlyList<ExperienceDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ExperienceDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<ExperienceDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateExperienceRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateExperienceRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}