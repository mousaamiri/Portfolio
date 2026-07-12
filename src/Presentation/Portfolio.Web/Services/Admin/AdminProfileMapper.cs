using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Services.Admin;

/// <summary>
/// Profile admin mapping. Hand-written (not Mapperly) because the bilingual
/// merge/flatten dominates and the neutral fields are few.
/// </summary>
public static class AdminProfileMapper
{
    public static ProfileFormModel ToFormModel(ProfileApiDto? en, ProfileApiDto? fa)
    {
        if (en is null) return new ProfileFormModel();
        return new ProfileFormModel
        {
            Email = en.Email,
            GitHubUrl = en.GitHubUrl,
            LinkedInUrl = en.LinkedInUrl,
            InstagramUrl = en.InstagramUrl,
            TelegramUrl = en.TelegramUrl,
            TwitterUrl = en.TwitterUrl,
            WebsiteUrl = en.WebsiteUrl,
            ResumeUrlEn = en.ResumeUrlEn,
            ResumeUrlFa = en.ResumeUrlFa,
            PortraitUrl = en.PortraitUrl,
            LearningDate = en.LearningDate,
            Phone = en.Phone,
            CountryCode = en.CountryCode,
            FullNameEn = en.FullName,
            JobTitleEn = en.JobTitle,
            TaglineEn = en.Tagline,
            BioEn = en.Bio,
            LearningTitleEn = en.LearningTitle,
            LearningDescEn = en.LearningDesc,
            RoleBadgeEn = en.RoleBadge,
            ExperienceBadgeEn = en.ExperienceBadge,
            DegreeBadgeEn = en.DegreeBadge,
            PortraitAltEn = en.PortraitAlt,
            LocationEn = en.Location,
            CountryEn = en.Country,
            FullNameFa = fa?.FullName ?? string.Empty,
            JobTitleFa = fa?.JobTitle ?? string.Empty,
            TaglineFa = fa?.Tagline,
            BioFa = fa?.Bio,
            LearningTitleFa = fa?.LearningTitle,
            LearningDescFa = fa?.LearningDesc,
            RoleBadgeFa = fa?.RoleBadge,
            ExperienceBadgeFa = fa?.ExperienceBadge,
            DegreeBadgeFa = fa?.DegreeBadge,
            PortraitAltFa = fa?.PortraitAlt,
            LocationFa = fa?.Location,
            CountryFa = fa?.Country
        };
    }

    public static UpsertProfileApiRequest ToRequest(ProfileFormModel m)
    {
        var list = new List<ProfileTranslationApiRequest>
        {
            new()
            {
                LanguageCode = "en", FullName = m.FullNameEn, JobTitle = m.JobTitleEn,
                Tagline = m.TaglineEn, Bio = m.BioEn, LearningTitle = m.LearningTitleEn, LearningDesc = m.LearningDescEn,
                RoleBadge = m.RoleBadgeEn, ExperienceBadge = m.ExperienceBadgeEn, DegreeBadge = m.DegreeBadgeEn,
                PortraitAlt = m.PortraitAltEn, Location = m.LocationEn, Country = m.CountryEn
            }
        };
        if (!string.IsNullOrWhiteSpace(m.FullNameFa))
        {
            list.Add(new()
            {
                LanguageCode = "fa", FullName = m.FullNameFa, JobTitle = m.JobTitleFa,
                Tagline = m.TaglineFa, Bio = m.BioFa, LearningTitle = m.LearningTitleFa, LearningDesc = m.LearningDescFa,
                RoleBadge = m.RoleBadgeFa, ExperienceBadge = m.ExperienceBadgeFa, DegreeBadge = m.DegreeBadgeFa,
                PortraitAlt = m.PortraitAltFa, Location = m.LocationFa, Country = m.CountryFa
            });
        }

        return new UpsertProfileApiRequest
        {
            Email = m.Email, GitHubUrl = m.GitHubUrl, LinkedInUrl = m.LinkedInUrl, InstagramUrl = m.InstagramUrl,
            TelegramUrl = m.TelegramUrl, TwitterUrl = m.TwitterUrl,
            WebsiteUrl = m.WebsiteUrl, ResumeUrlEn = m.ResumeUrlEn, ResumeUrlFa = m.ResumeUrlFa,
            PortraitUrl = m.PortraitUrl, LearningDate = m.LearningDate, Phone = m.Phone, CountryCode = m.CountryCode,
            Translations = list
        };
    }
}
