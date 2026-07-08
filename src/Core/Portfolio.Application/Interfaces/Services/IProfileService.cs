using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Profiles;

namespace Portfolio.Application.Interfaces.Services;

public interface IProfileService
{
    Task<Result<ProfileDto>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default);
    Task<Result<Guid>> UpsertAsync(UpsertProfileRequest request, CancellationToken cancellationToken = default);
}
