using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Educations;

namespace Portfolio.Application.Interfaces.Services;

public interface IEducationService
{
    Task<Result<IReadOnlyList<EducationDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<EducationDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateEducationRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateEducationRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}