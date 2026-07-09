using Riok.Mapperly.Abstractions;
using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Services.Admin;

/// <summary>
/// Skill admin mapping. Mapperly source-generates the straight DTO→ViewModel
/// projections; the bilingual flatten (form → EN/FA translation requests) and
/// merge (EN+FA DTOs → one form) stay as hand-written helpers, since they are not
/// 1:1 property maps.
/// </summary>
[Mapper]
public static partial class AdminSkillMapper
{
    // ── Straight maps (generated) ──
    [MapProperty(nameof(SkillApiDto.Name), nameof(SkillFormModel.NameEn))]
    [MapProperty(nameof(SkillApiDto.Category), nameof(SkillFormModel.CategoryEn))]
    [MapProperty(nameof(SkillApiDto.Description), nameof(SkillFormModel.DescriptionEn))]
    private static partial SkillFormModel ToFormEn(SkillApiDto en);

    public static AdminListRow ToRow(SkillApiDto d) => new()
    {
        Id = d.Id,
        Cells = [d.Name, d.Category ?? "—", $"{d.Proficiency}%"],
        Active = d.IsActive
    };

    // ── Bilingual merge (hand-written: two DTOs → one form) ──
    public static SkillFormModel ToFormModel(SkillApiDto en, SkillApiDto? fa)
    {
        var m = ToFormEn(en);
        m.NameFa = fa?.Name ?? string.Empty;
        m.CategoryFa = fa?.Category;
        m.DescriptionFa = fa?.Description;
        return m;
    }

    // ── Bilingual flatten (hand-written: form → EN/FA translations) ──
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
