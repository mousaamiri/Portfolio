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
            FullNameEn = en.FullName,
            JobTitleEn = en.JobTitle,
            TaglineEn = en.Tagline,
            BioEn = en.Bio,
            LearningTitleEn = en.LearningTitle,
            LearningDescEn = en.LearningDesc,
            FullNameFa = fa?.FullName ?? string.Empty,
            JobTitleFa = fa?.JobTitle ?? string.Empty,
            TaglineFa = fa?.Tagline,
            BioFa = fa?.Bio,
            LearningTitleFa = fa?.LearningTitle,
            LearningDescFa = fa?.LearningDesc
        };
    }

    public static UpsertProfileApiRequest ToRequest(ProfileFormModel m)
    {
        var list = new List<ProfileTranslationApiRequest>
        {
            new()
            {
                LanguageCode = "en", FullName = m.FullNameEn, JobTitle = m.JobTitleEn,
                Tagline = m.TaglineEn, Bio = m.BioEn, LearningTitle = m.LearningTitleEn, LearningDesc = m.LearningDescEn
            }
        };
        if (!string.IsNullOrWhiteSpace(m.FullNameFa))
        {
            list.Add(new()
            {
                LanguageCode = "fa", FullName = m.FullNameFa, JobTitle = m.JobTitleFa,
                Tagline = m.TaglineFa, Bio = m.BioFa, LearningTitle = m.LearningTitleFa, LearningDesc = m.LearningDescFa
            });
        }

        return new UpsertProfileApiRequest
        {
            Email = m.Email, GitHubUrl = m.GitHubUrl, LinkedInUrl = m.LinkedInUrl, InstagramUrl = m.InstagramUrl,
            TelegramUrl = m.TelegramUrl, TwitterUrl = m.TwitterUrl,
            WebsiteUrl = m.WebsiteUrl, ResumeUrlEn = m.ResumeUrlEn, ResumeUrlFa = m.ResumeUrlFa,
            PortraitUrl = m.PortraitUrl, LearningDate = m.LearningDate, Translations = list
        };
    }
}
