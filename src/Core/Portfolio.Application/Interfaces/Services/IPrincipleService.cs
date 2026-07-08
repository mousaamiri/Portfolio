using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Principles;

namespace Portfolio.Application.Interfaces.Services;

public interface IPrincipleService
{
    Task<Result<IReadOnlyList<PrincipleDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<PrincipleDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<PrincipleDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreatePrincipleRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdatePrincipleRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
