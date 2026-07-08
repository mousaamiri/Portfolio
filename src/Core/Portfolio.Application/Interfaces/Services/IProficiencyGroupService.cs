using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Proficiencies;

namespace Portfolio.Application.Interfaces.Services;

public interface IProficiencyGroupService
{
    Task<Result<IReadOnlyList<ProficiencyGroupDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ProficiencyGroupDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<ProficiencyGroupDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateProficiencyGroupRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateProficiencyGroupRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
