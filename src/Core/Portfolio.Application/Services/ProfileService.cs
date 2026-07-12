using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Profiles;
using Portfolio.Application.Extensions;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Profiles;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class ProfileService(IUnitOfWork unitOfWork) : IProfileService
{
    public async Task<Result<ProfileDto>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var profile = await unitOfWork.Profiles.GetFirstActiveWithTranslationsAsync(cancellationToken);
        if (profile is null)
            return Result<ProfileDto>.Failure("No profile has been configured yet.");

        var language = ParseLanguage(languageCode);
        return Result<ProfileDto>.Success(MapToDto(profile, language));
    }

    public async Task<Result<Guid>> UpsertAsync(UpsertProfileRequest request, CancellationToken cancellationToken = default)
    {
        // Single-profile model: update the existing active profile if present,
        // otherwise create the first one.
        var profile = await unitOfWork.Profiles.GetFirstActiveWithTranslationsAsync(cancellationToken);
        var isNew = profile is null;

        if (profile is null)
        {
            profile = new Profile { Id = Guid.NewGuid(), IsActive = true, CreatedAt = DateTime.UtcNow };
        }

        profile.Email = request.Email;
        profile.GitHubUrl = request.GitHubUrl;
        profile.LinkedInUrl = request.LinkedInUrl;
        profile.InstagramUrl = request.InstagramUrl;
        profile.TelegramUrl = request.TelegramUrl;
        profile.TwitterUrl = request.TwitterUrl;
        profile.WebsiteUrl = request.WebsiteUrl;
        profile.ResumeUrlEn = request.ResumeUrlEn;
        profile.ResumeUrlFa = request.ResumeUrlFa;
        profile.PortraitUrl = request.PortraitUrl;
        profile.LearningDate = request.LearningDate;
        profile.Phone = request.Phone;
        profile.CountryCode = request.CountryCode;

        if (!isNew)
            profile.UpdatedAt = DateTime.UtcNow;

        profile.Translations.SyncTranslations(
            request.Translations.Select(t => new ProfileTranslation
            {
                ProfileId = profile.Id,
                Language = ParseLanguage(t.LanguageCode),
                FullName = t.FullName,
                JobTitle = t.JobTitle,
                Tagline = t.Tagline,
                Bio = t.Bio,
                LearningTitle = t.LearningTitle,
                LearningDesc = t.LearningDesc,
                RoleBadge = t.RoleBadge,
                ExperienceBadge = t.ExperienceBadge,
                DegreeBadge = t.DegreeBadge,
                PortraitAlt = t.PortraitAlt,
                Location = t.Location,
                Country = t.Country
            }).ToList(),
            (existing, incoming) =>
            {
                existing.FullName = incoming.FullName;
                existing.JobTitle = incoming.JobTitle;
                existing.Tagline = incoming.Tagline;
                existing.Bio = incoming.Bio;
                existing.LearningTitle = incoming.LearningTitle;
                existing.LearningDesc = incoming.LearningDesc;
                existing.RoleBadge = incoming.RoleBadge;
                existing.ExperienceBadge = incoming.ExperienceBadge;
                existing.DegreeBadge = incoming.DegreeBadge;
                existing.PortraitAlt = incoming.PortraitAlt;
                existing.Location = incoming.Location;
                existing.Country = incoming.Country;
            });

        if (isNew)
            await unitOfWork.Profiles.AddAsync(profile, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(profile.Id);
    }

    private static Language ParseLanguage(string languageCode)
    {
        return Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language)
            ? language
            : Language.En;
    }

    private static ProfileDto MapToDto(Profile profile, Language language)
    {
        var t = profile.Translations.FirstOrDefault(x => x.Language == language)
                ?? profile.Translations.FirstOrDefault();

        return new ProfileDto
        {
            Id = profile.Id,
            Email = profile.Email,
            GitHubUrl = profile.GitHubUrl,
            LinkedInUrl = profile.LinkedInUrl,
            InstagramUrl = profile.InstagramUrl,
            TelegramUrl = profile.TelegramUrl,
            TwitterUrl = profile.TwitterUrl,
            WebsiteUrl = profile.WebsiteUrl,
            ResumeUrlEn = profile.ResumeUrlEn,
            ResumeUrlFa = profile.ResumeUrlFa,
            PortraitUrl = profile.PortraitUrl,
            LearningDate = profile.LearningDate,
            Phone = profile.Phone,
            CountryCode = profile.CountryCode,
            IsActive = profile.IsActive,
            CreatedAt = profile.CreatedAt,
            FullName = t?.FullName ?? string.Empty,
            JobTitle = t?.JobTitle ?? string.Empty,
            Tagline = t?.Tagline,
            Bio = t?.Bio,
            LearningTitle = t?.LearningTitle,
            LearningDesc = t?.LearningDesc,
            RoleBadge = t?.RoleBadge,
            ExperienceBadge = t?.ExperienceBadge,
            DegreeBadge = t?.DegreeBadge,
            PortraitAlt = t?.PortraitAlt,
            Location = t?.Location,
            Country = t?.Country
        };
    }
}
