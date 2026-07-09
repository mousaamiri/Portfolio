using Riok.Mapperly.Abstractions;
using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Services.Admin;

[Mapper]
public static partial class AdminArticleMapper
{
    public static AdminListRow ToRow(ArticleApiDto d) => new()
    {
        Id = d.Id,
        Cells = [d.Title, d.Category ?? "—", d.PublishDate.ToString("yyyy-MM-dd")],
        Published = d.IsPublished
    };

    [MapProperty(nameof(ArticleApiDto.Title), nameof(ArticleFormModel.TitleEn))]
    [MapProperty(nameof(ArticleApiDto.Excerpt), nameof(ArticleFormModel.ExcerptEn))]
    [MapProperty(nameof(ArticleApiDto.Body), nameof(ArticleFormModel.BodyEn))]
    private static partial ArticleFormModel ToFormEn(ArticleApiDto en);

    public static ArticleFormModel ToFormModel(ArticleApiDto en, ArticleApiDto? fa)
    {
        var m = ToFormEn(en);
        m.TitleFa = fa?.Title ?? string.Empty;
        m.ExcerptFa = fa?.Excerpt;
        m.BodyFa = fa?.Body;
        return m;
    }

    public static ArticleApiRequest ToRequest(ArticleFormModel m)
    {
        var list = new List<ArticleTranslationApiRequest>
        {
            new() { LanguageCode = "en", Title = m.TitleEn, Excerpt = m.ExcerptEn, Body = m.BodyEn }
        };
        if (!string.IsNullOrWhiteSpace(m.TitleFa))
            list.Add(new() { LanguageCode = "fa", Title = m.TitleFa, Excerpt = m.ExcerptFa, Body = m.BodyFa });
        return new ArticleApiRequest
        {
            Slug = m.Slug, Category = m.Category, Tags = m.Tags, CoverImageUrl = m.CoverImageUrl,
            PublishDate = m.PublishDate, ReadTimeMinutes = m.ReadTimeMinutes, IsPublished = m.IsPublished,
            DisplayOrder = m.DisplayOrder, IsActive = m.IsActive, Translations = list
        };
    }
}

[Mapper]
public static partial class AdminTestimonialMapper
{
    public static AdminListRow ToRow(TestimonialApiDto d) => new()
    {
        Id = d.Id, Cells = [d.Name, d.Role, d.Quote.Length <= 50 ? d.Quote : d.Quote[..47] + "…"], Active = d.IsActive
    };

    [MapProperty(nameof(TestimonialApiDto.Quote), nameof(TestimonialFormModel.QuoteEn))]
    [MapProperty(nameof(TestimonialApiDto.Name), nameof(TestimonialFormModel.NameEn))]
    [MapProperty(nameof(TestimonialApiDto.Role), nameof(TestimonialFormModel.RoleEn))]
    private static partial TestimonialFormModel ToFormEn(TestimonialApiDto en);

    public static TestimonialFormModel ToFormModel(TestimonialApiDto en, TestimonialApiDto? fa)
    {
        var m = ToFormEn(en);
        m.QuoteFa = fa?.Quote ?? string.Empty;
        m.NameFa = fa?.Name ?? string.Empty;
        m.RoleFa = fa?.Role ?? string.Empty;
        return m;
    }

    public static TestimonialApiRequest ToRequest(TestimonialFormModel m)
    {
        var list = new List<TestimonialTranslationApiRequest>
        {
            new() { LanguageCode = "en", Quote = m.QuoteEn, Name = m.NameEn, Role = m.RoleEn }
        };
        if (!string.IsNullOrWhiteSpace(m.NameFa))
            list.Add(new() { LanguageCode = "fa", Quote = m.QuoteFa, Name = m.NameFa, Role = m.RoleFa });
        return new TestimonialApiRequest
        {
            Initials = m.Initials, AvatarColor = m.AvatarColor,
            DisplayOrder = m.DisplayOrder, IsActive = m.IsActive, Translations = list
        };
    }
}
