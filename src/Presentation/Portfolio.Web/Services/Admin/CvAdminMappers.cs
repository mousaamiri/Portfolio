using Riok.Mapperly.Abstractions;
using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Services.Admin;

[Mapper]
public static partial class AdminExperienceMapper
{
    public static AdminListRow ToRow(ExperienceApiDto d) => new()
    {
        Id = d.Id,
        Cells = [d.JobTitle, d.CompanyName, $"{d.StartDate:yyyy} – {(d.EndDate is null ? "Present" : d.EndDate.Value.ToString("yyyy"))}"],
        Active = d.IsActive
    };

    [MapProperty(nameof(ExperienceApiDto.CompanyName), nameof(ExperienceFormModel.CompanyNameEn))]
    [MapProperty(nameof(ExperienceApiDto.JobTitle), nameof(ExperienceFormModel.JobTitleEn))]
    [MapProperty(nameof(ExperienceApiDto.Description), nameof(ExperienceFormModel.DescriptionEn))]
    [MapProperty(nameof(ExperienceApiDto.Location), nameof(ExperienceFormModel.LocationEn))]
    private static partial ExperienceFormModel ToFormEn(ExperienceApiDto en);

    public static ExperienceFormModel ToFormModel(ExperienceApiDto en, ExperienceApiDto? fa)
    {
        var m = ToFormEn(en);
        m.CompanyNameFa = fa?.CompanyName ?? string.Empty;
        m.JobTitleFa = fa?.JobTitle ?? string.Empty;
        m.DescriptionFa = fa?.Description;
        m.LocationFa = fa?.Location;
        return m;
    }

    public static ExperienceApiRequest ToRequest(ExperienceFormModel m)
    {
        var list = new List<ExperienceTranslationApiRequest>
        {
            new() { LanguageCode = "en", CompanyName = m.CompanyNameEn, JobTitle = m.JobTitleEn, Description = m.DescriptionEn, Location = m.LocationEn }
        };
        if (!string.IsNullOrWhiteSpace(m.CompanyNameFa))
            list.Add(new() { LanguageCode = "fa", CompanyName = m.CompanyNameFa, JobTitle = m.JobTitleFa, Description = m.DescriptionFa, Location = m.LocationFa });
        return new ExperienceApiRequest
        {
            CompanyLogo = m.CompanyLogo, CompanyUrl = m.CompanyUrl,
            StartDate = m.StartDate, EndDate = m.EndDate, IsActive = m.IsActive, Translations = list
        };
    }
}

[Mapper]
public static partial class AdminEducationMapper
{
    public static AdminListRow ToRow(EducationApiDto d) => new()
    {
        Id = d.Id,
        Cells = [d.Degree, d.InstitutionName, $"{d.StartDate:yyyy} – {(d.EndDate is null ? "Present" : d.EndDate.Value.ToString("yyyy"))}"],
        Active = d.IsActive
    };

    [MapProperty(nameof(EducationApiDto.InstitutionName), nameof(EducationFormModel.InstitutionNameEn))]
    [MapProperty(nameof(EducationApiDto.Degree), nameof(EducationFormModel.DegreeEn))]
    [MapProperty(nameof(EducationApiDto.FieldOfStudy), nameof(EducationFormModel.FieldOfStudyEn))]
    [MapProperty(nameof(EducationApiDto.Description), nameof(EducationFormModel.DescriptionEn))]
    private static partial EducationFormModel ToFormEn(EducationApiDto en);

    public static EducationFormModel ToFormModel(EducationApiDto en, EducationApiDto? fa)
    {
        var m = ToFormEn(en);
        m.InstitutionNameFa = fa?.InstitutionName ?? string.Empty;
        m.DegreeFa = fa?.Degree ?? string.Empty;
        m.FieldOfStudyFa = fa?.FieldOfStudy ?? string.Empty;
        m.DescriptionFa = fa?.Description;
        return m;
    }

    public static EducationApiRequest ToRequest(EducationFormModel m)
    {
        var list = new List<EducationTranslationApiRequest>
        {
            new() { LanguageCode = "en", InstitutionName = m.InstitutionNameEn, Degree = m.DegreeEn, FieldOfStudy = m.FieldOfStudyEn, Description = m.DescriptionEn }
        };
        if (!string.IsNullOrWhiteSpace(m.InstitutionNameFa))
            list.Add(new() { LanguageCode = "fa", InstitutionName = m.InstitutionNameFa, Degree = m.DegreeFa, FieldOfStudy = m.FieldOfStudyFa, Description = m.DescriptionFa });
        return new EducationApiRequest
        {
            InstitutionLogo = m.InstitutionLogo, InstitutionUrl = m.InstitutionUrl,
            StartDate = m.StartDate, EndDate = m.EndDate, Gpa = m.Gpa, IsActive = m.IsActive, Translations = list
        };
    }
}
