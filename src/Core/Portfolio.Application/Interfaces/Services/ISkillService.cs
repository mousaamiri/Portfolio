using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Skills;

namespace Portfolio.Application.Interfaces.Services;

public interface ISkillService
{
    Task<Result<IReadOnlyList<SkillDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<SkillDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateSkillRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateSkillRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
