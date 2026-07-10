using Riok.Mapperly.Abstractions;
using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Services.Admin;

[Mapper]
public static partial class AdminFaqMapper
{
    public static AdminListRow ToRow(FaqApiDto d) => new()
    {
        Id = d.Id,
        Cells = [Truncate(d.Question), Truncate(d.Answer)],
        Active = d.IsActive
    };

    [MapProperty(nameof(FaqApiDto.Question), nameof(FaqFormModel.QuestionEn))]
    [MapProperty(nameof(FaqApiDto.Answer), nameof(FaqFormModel.AnswerEn))]
    private static partial FaqFormModel ToFormEn(FaqApiDto en);

    public static FaqFormModel ToFormModel(FaqApiDto en, FaqApiDto? fa)
    {
        var m = ToFormEn(en);
        m.QuestionFa = fa?.Question ?? string.Empty;
        m.AnswerFa = fa?.Answer ?? string.Empty;
        return m;
    }

    public static FaqApiRequest ToRequest(FaqFormModel m)
    {
        var list = new List<FaqTranslationApiRequest>
        {
            new() { LanguageCode = "en", Question = m.QuestionEn, Answer = m.AnswerEn }
        };
        if (!string.IsNullOrWhiteSpace(m.QuestionFa))
            list.Add(new() { LanguageCode = "fa", Question = m.QuestionFa, Answer = m.AnswerFa });
        return new FaqApiRequest { DisplayOrder = m.DisplayOrder, IsActive = m.IsActive, Translations = list };
    }

    private static string Truncate(string s) => s.Length <= 60 ? s : s[..57] + "…";
}

[Mapper]
public static partial class AdminInterestMapper
{
    public static AdminListRow ToRow(InterestApiDto d) => new()
    {
        Id = d.Id, Cells = [d.Label, d.Icon], Active = d.IsActive
    };

    [MapProperty(nameof(InterestApiDto.Label), nameof(InterestFormModel.LabelEn))]
    private static partial InterestFormModel ToFormEn(InterestApiDto en);

    public static InterestFormModel ToFormModel(InterestApiDto en, InterestApiDto? fa)
    {
        var m = ToFormEn(en);
        m.LabelFa = fa?.Label ?? string.Empty;
        return m;
    }

    public static InterestApiRequest ToRequest(InterestFormModel m)
    {
        var list = new List<InterestTranslationApiRequest> { new() { LanguageCode = "en", Label = m.LabelEn } };
        if (!string.IsNullOrWhiteSpace(m.LabelFa))
            list.Add(new() { LanguageCode = "fa", Label = m.LabelFa });
        return new InterestApiRequest { Icon = m.Icon, DisplayOrder = m.DisplayOrder, IsActive = m.IsActive, Translations = list };
    }
}

[Mapper]
public static partial class AdminTimelineMapper
{
    public static AdminListRow ToRow(TimelineEntryApiDto d) => new()
    {
        Id = d.Id, Cells = [d.Year, d.Title, d.Icon], Active = d.IsActive
    };

    [MapProperty(nameof(TimelineEntryApiDto.Title), nameof(TimelineFormModel.TitleEn))]
    [MapProperty(nameof(TimelineEntryApiDto.Description), nameof(TimelineFormModel.DescriptionEn))]
    private static partial TimelineFormModel ToFormEn(TimelineEntryApiDto en);

    public static TimelineFormModel ToFormModel(TimelineEntryApiDto en, TimelineEntryApiDto? fa)
    {
        var m = ToFormEn(en);
        m.TitleFa = fa?.Title ?? string.Empty;
        m.DescriptionFa = fa?.Description;
        return m;
    }

    public static TimelineApiRequest ToRequest(TimelineFormModel m)
    {
        var list = new List<TimelineTranslationApiRequest>
        {
            new() { LanguageCode = "en", Title = m.TitleEn, Description = m.DescriptionEn }
        };
        if (!string.IsNullOrWhiteSpace(m.TitleFa))
            list.Add(new() { LanguageCode = "fa", Title = m.TitleFa, Description = m.DescriptionFa });
        return new TimelineApiRequest { Year = m.Year, Icon = m.Icon, DisplayOrder = m.DisplayOrder, IsActive = m.IsActive, Translations = list };
    }
}

[Mapper]
public static partial class AdminStatMapper
{
    public static AdminListRow ToRow(StatCounterApiDto d) => new()
    {
        Id = d.Id, Cells = [d.Label, $"{d.CountTarget}{d.Suffix}", d.Icon], Active = d.IsActive
    };

    [MapProperty(nameof(StatCounterApiDto.Label), nameof(StatFormModel.LabelEn))]
    [MapProperty(nameof(StatCounterApiDto.TipText), nameof(StatFormModel.TipTextEn))]
    private static partial StatFormModel ToFormEn(StatCounterApiDto en);

    public static StatFormModel ToFormModel(StatCounterApiDto en, StatCounterApiDto? fa)
    {
        var m = ToFormEn(en);
        m.LabelFa = fa?.Label ?? string.Empty;
        m.TipTextFa = fa?.TipText;
        return m;
    }

    public static StatApiRequest ToRequest(StatFormModel m)
    {
        var list = new List<StatTranslationApiRequest>
        {
            new() { LanguageCode = "en", Label = m.LabelEn, TipText = m.TipTextEn }
        };
        if (!string.IsNullOrWhiteSpace(m.LabelFa))
            list.Add(new() { LanguageCode = "fa", Label = m.LabelFa, TipText = m.TipTextFa });
        return new StatApiRequest
        {
            Icon = m.Icon, CountTarget = m.CountTarget, Suffix = m.Suffix,
            DisplayOrder = m.DisplayOrder, IsActive = m.IsActive, Translations = list
        };
    }
}

// ── UI translation (flat key→value; not bilingual, so a plain mapper) ──
public static class AdminUiTranslationMapper
{
    public static AdminListRow ToRow(UiTranslationApiDto d) => new()
    {
        Id = d.Id,
        Cells = [d.Key, d.Language.ToUpperInvariant(), Trim(d.Value)],
        Active = d.IsActive
    };

    public static UiTranslationFormModel ToFormModel(UiTranslationApiDto d) => new()
    {
        Id = d.Id,
        Key = d.Key,
        LanguageCode = string.IsNullOrWhiteSpace(d.Language) ? "fa" : d.Language.ToLowerInvariant(),
        Value = d.Value,
        IsActive = d.IsActive
    };

    public static UiTranslationApiRequest ToRequest(UiTranslationFormModel m) => new()
    {
        Key = m.Key.Trim(),
        LanguageCode = m.LanguageCode,
        Value = m.Value,
        IsActive = m.IsActive
    };

    private static string Trim(string s) => s.Length <= 80 ? s : s[..77] + "…";
}
