using Riok.Mapperly.Abstractions;
using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Services.Admin;

/// <summary>
/// Project admin mapping. Mapperly source-generates the property projections
/// (DTO→list, DTO→form EN half, form→request); the bilingual flatten/merge stay
/// as hand-written wrappers since they are not 1:1.
/// </summary>
[Mapper]
public static partial class AdminProjectMapper
{
    public static partial ProjectListItem ToListItem(ProjectApiDto dto);

    // DTO (English) → form. FA half is filled by ToFormModel below.
    [MapProperty(nameof(ProjectApiDto.Title), nameof(ProjectFormModel.TitleEn))]
    [MapProperty(nameof(ProjectApiDto.ShortDescription), nameof(ProjectFormModel.ShortDescriptionEn))]
    [MapProperty(nameof(ProjectApiDto.Description), nameof(ProjectFormModel.DescriptionEn))]
    [MapProperty(nameof(ProjectApiDto.Technologies), nameof(ProjectFormModel.TechnologiesEn))]
    private static partial ProjectFormModel ToFormEn(ProjectApiDto en);

    // form → create/update request (translations set by the wrappers below).
    [MapperIgnoreTarget(nameof(CreateProjectApiRequest.Translations))]
    private static partial CreateProjectApiRequest ToCreateBase(ProjectFormModel m);

    [MapperIgnoreTarget(nameof(UpdateProjectApiRequest.Translations))]
    private static partial UpdateProjectApiRequest ToUpdateBase(ProjectFormModel m);

    public static ProjectFormModel ToFormModel(ProjectApiDto en, ProjectApiDto? fa)
    {
        var m = ToFormEn(en);
        m.TitleFa = fa?.Title ?? string.Empty;
        m.ShortDescriptionFa = fa?.ShortDescription ?? string.Empty;
        m.DescriptionFa = fa?.Description;
        m.TechnologiesFa = fa?.Technologies;
        return m;
    }

    public static CreateProjectApiRequest ToCreateRequest(ProjectFormModel m)
    {
        var r = ToCreateBase(m);
        r.Translations = BuildTranslations(m);
        return r;
    }

    public static UpdateProjectApiRequest ToUpdateRequest(ProjectFormModel m)
    {
        var r = ToUpdateBase(m);
        r.Translations = BuildTranslations(m);
        return r;
    }

    private static List<ProjectTranslationApiRequest> BuildTranslations(ProjectFormModel m)
    {
        var translations = new List<ProjectTranslationApiRequest>
        {
            new()
            {
                LanguageCode = "en",
                Title = m.TitleEn,
                ShortDescription = m.ShortDescriptionEn,
                Description = m.DescriptionEn,
                Technologies = m.TechnologiesEn
            }
        };

        if (!string.IsNullOrWhiteSpace(m.TitleFa))
        {
            translations.Add(new ProjectTranslationApiRequest
            {
                LanguageCode = "fa",
                Title = m.TitleFa,
                ShortDescription = m.ShortDescriptionFa,
                Description = m.DescriptionFa,
                Technologies = m.TechnologiesFa
            });
        }

        return translations;
    }
}
