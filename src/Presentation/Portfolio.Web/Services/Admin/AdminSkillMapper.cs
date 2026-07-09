using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Services.Admin;

public static class AdminSkillMapper
{
    public static AdminListRow ToRow(SkillApiDto d) => new()
    {
        Id = d.Id,
        Cells = [d.Name, d.Category ?? "—", $"{d.Proficiency}%"],
        Active = d.IsActive
    };

    public static SkillFormModel ToFormModel(SkillApiDto en, SkillApiDto? fa) => new()
    {
        Id = en.Id,
        IconUrl = en.IconUrl,
        Proficiency = en.Proficiency,
        DisplayOrder = en.DisplayOrder,
        IsActive = en.IsActive,
        NameEn = en.Name,
        CategoryEn = en.Category,
        DescriptionEn = en.Description,
        NameFa = fa?.Name ?? string.Empty,
        CategoryFa = fa?.Category,
        DescriptionFa = fa?.Description
    };

    public static SkillApiRequest ToRequest(SkillFormModel m) => new()
    {
        IconUrl = m.IconUrl,
        Proficiency = m.Proficiency,
        DisplayOrder = m.DisplayOrder,
        IsActive = m.IsActive,
        Translations = BuildTranslations(m)
    };

    private static List<SkillTranslationApiRequest> BuildTranslations(SkillFormModel m)
    {
        var list = new List<SkillTranslationApiRequest>
        {
            new() { LanguageCode = "en", Name = m.NameEn, Category = m.CategoryEn, Description = m.DescriptionEn }
        };
        if (!string.IsNullOrWhiteSpace(m.NameFa))
            list.Add(new() { LanguageCode = "fa", Name = m.NameFa, Category = m.CategoryFa, Description = m.DescriptionFa });
        return list;
    }
}
