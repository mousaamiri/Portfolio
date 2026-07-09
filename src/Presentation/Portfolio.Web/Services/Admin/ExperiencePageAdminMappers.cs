using Riok.Mapperly.Abstractions;
using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Services.Admin;

[Mapper]
public static partial class AdminImpactMetricMapper
{
    public static AdminListRow ToRow(ImpactMetricApiDto d) => new()
    {
        Id = d.Id, Cells = [d.Value, d.Tag, d.Color], Active = d.IsActive
    };

    [MapProperty(nameof(ImpactMetricApiDto.Tag), nameof(ImpactMetricFormModel.TagEn))]
    [MapProperty(nameof(ImpactMetricApiDto.Description), nameof(ImpactMetricFormModel.DescriptionEn))]
    private static partial ImpactMetricFormModel ToFormEn(ImpactMetricApiDto en);

    public static ImpactMetricFormModel ToFormModel(ImpactMetricApiDto en, ImpactMetricApiDto? fa)
    {
        var m = ToFormEn(en);
        m.TagFa = fa?.Tag ?? string.Empty;
        m.DescriptionFa = fa?.Description ?? string.Empty;
        return m;
    }

    public static ImpactMetricApiRequest ToRequest(ImpactMetricFormModel m)
    {
        var list = new List<ImpactMetricTranslationApiRequest>
        {
            new() { LanguageCode = "en", Tag = m.TagEn, Description = m.DescriptionEn }
        };
        if (!string.IsNullOrWhiteSpace(m.TagFa))
            list.Add(new() { LanguageCode = "fa", Tag = m.TagFa, Description = m.DescriptionFa });
        return new ImpactMetricApiRequest { Value = m.Value, Color = m.Color, DisplayOrder = m.DisplayOrder, IsActive = m.IsActive, Translations = list };
    }
}

[Mapper]
public static partial class AdminPrincipleMapper
{
    public static AdminListRow ToRow(PrincipleApiDto d) => new()
    {
        Id = d.Id, Cells = [d.Title, d.Description.Length <= 60 ? d.Description : d.Description[..57] + "…"], Active = d.IsActive
    };

    [MapProperty(nameof(PrincipleApiDto.Title), nameof(PrincipleFormModel.TitleEn))]
    [MapProperty(nameof(PrincipleApiDto.Description), nameof(PrincipleFormModel.DescriptionEn))]
    private static partial PrincipleFormModel ToFormEn(PrincipleApiDto en);

    public static PrincipleFormModel ToFormModel(PrincipleApiDto en, PrincipleApiDto? fa)
    {
        var m = ToFormEn(en);
        m.TitleFa = fa?.Title ?? string.Empty;
        m.DescriptionFa = fa?.Description ?? string.Empty;
        return m;
    }

    public static PrincipleApiRequest ToRequest(PrincipleFormModel m)
    {
        var list = new List<PrincipleTranslationApiRequest>
        {
            new() { LanguageCode = "en", Title = m.TitleEn, Description = m.DescriptionEn }
        };
        if (!string.IsNullOrWhiteSpace(m.TitleFa))
            list.Add(new() { LanguageCode = "fa", Title = m.TitleFa, Description = m.DescriptionFa });
        return new PrincipleApiRequest { DisplayOrder = m.DisplayOrder, IsActive = m.IsActive, Translations = list };
    }
}

[Mapper]
public static partial class AdminProficiencyMapper
{
    public static AdminListRow ToRow(ProficiencyGroupApiDto d) => new()
    {
        Id = d.Id, Cells = [d.Title, d.Items, d.Color], Active = d.IsActive
    };

    [MapProperty(nameof(ProficiencyGroupApiDto.Title), nameof(ProficiencyFormModel.TitleEn))]
    [MapProperty(nameof(ProficiencyGroupApiDto.Items), nameof(ProficiencyFormModel.ItemsEn))]
    private static partial ProficiencyFormModel ToFormEn(ProficiencyGroupApiDto en);

    public static ProficiencyFormModel ToFormModel(ProficiencyGroupApiDto en, ProficiencyGroupApiDto? fa)
    {
        var m = ToFormEn(en);
        m.TitleFa = fa?.Title ?? string.Empty;
        m.ItemsFa = fa?.Items ?? string.Empty;
        return m;
    }

    public static ProficiencyApiRequest ToRequest(ProficiencyFormModel m)
    {
        var list = new List<ProficiencyTranslationApiRequest>
        {
            new() { LanguageCode = "en", Title = m.TitleEn, Items = m.ItemsEn }
        };
        if (!string.IsNullOrWhiteSpace(m.TitleFa))
            list.Add(new() { LanguageCode = "fa", Title = m.TitleFa, Items = m.ItemsFa });
        return new ProficiencyApiRequest { Color = m.Color, DisplayOrder = m.DisplayOrder, IsActive = m.IsActive, Translations = list };
    }
}
