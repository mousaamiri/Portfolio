using FluentAssertions;
using Portfolio.Web.Models.Admin;
using Portfolio.Web.Services.Admin;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Services.Admin;

public class AdminProfileMapperTests
{
    [Fact]
    public void ToRequest_MapsAllSocialLinksIncludingTelegramAndTwitter()
    {
        var form = new ProfileFormModel
        {
            Email = "me@example.com",
            GitHubUrl = "https://github.com/mousaamiri",
            LinkedInUrl = "https://linkedin.com/in/mousaamiri",
            InstagramUrl = "https://instagram.com/mousaamiri",
            TelegramUrl = "https://t.me/mousaamiri",
            TwitterUrl = "https://x.com/mousaamiri",
            WebsiteUrl = "https://mousaamiri.ir",
            FullNameEn = "Mousa", JobTitleEn = "Backend Developer"
        };

        var request = AdminProfileMapper.ToRequest(form);

        request.TelegramUrl.Should().Be("https://t.me/mousaamiri");
        request.TwitterUrl.Should().Be("https://x.com/mousaamiri");
        request.GitHubUrl.Should().Be("https://github.com/mousaamiri");
    }

    [Fact]
    public void ToFormModel_ReadsTelegramAndTwitterFromEnDto()
    {
        var en = new ProfileApiDto
        {
            Email = "me@example.com",
            TelegramUrl = "https://t.me/mousaamiri",
            TwitterUrl = "https://x.com/mousaamiri",
            FullName = "Mousa", JobTitle = "Backend Developer"
        };

        var form = AdminProfileMapper.ToFormModel(en, null);

        form.TelegramUrl.Should().Be("https://t.me/mousaamiri");
        form.TwitterUrl.Should().Be("https://x.com/mousaamiri");
    }

    [Fact]
    public void ToRequest_MapsContactAndLocalizedBadgeFields()
    {
        var form = new ProfileFormModel
        {
            Email = "me@example.com",
            Phone = "09906720069",
            CountryCode = "IR",
            FullNameEn = "Mousa", JobTitleEn = "Backend Developer",
            RoleBadgeEn = "Software Engineer", ExperienceBadgeEn = "9+ Years",
            DegreeBadgeEn = "Self-taught", PortraitAltEn = "Mousa portrait",
            LocationEn = "Tehran", CountryEn = "Iran",
            FullNameFa = "موسی", JobTitleFa = "توسعه‌دهنده", LocationFa = "تهران", CountryFa = "ایران"
        };

        var request = AdminProfileMapper.ToRequest(form);

        request.Phone.Should().Be("09906720069");
        request.CountryCode.Should().Be("IR");

        var en = request.Translations.Single(t => t.LanguageCode == "en");
        en.RoleBadge.Should().Be("Software Engineer");
        en.ExperienceBadge.Should().Be("9+ Years");
        en.DegreeBadge.Should().Be("Self-taught");
        en.PortraitAlt.Should().Be("Mousa portrait");
        en.Location.Should().Be("Tehran");
        en.Country.Should().Be("Iran");

        var fa = request.Translations.Single(t => t.LanguageCode == "fa");
        fa.Location.Should().Be("تهران");
        fa.Country.Should().Be("ایران");
    }

    [Fact]
    public void ToFormModel_ReadsContactAndBadgeFields()
    {
        var en = new ProfileApiDto
        {
            Email = "me@example.com",
            Phone = "09906720069", CountryCode = "IR",
            FullName = "Mousa", JobTitle = "Backend Developer",
            RoleBadge = "Software Engineer", ExperienceBadge = "9+ Years",
            Location = "Tehran", Country = "Iran"
        };
        var fa = new ProfileApiDto { FullName = "موسی", JobTitle = "توسعه‌دهنده", Location = "تهران", Country = "ایران" };

        var form = AdminProfileMapper.ToFormModel(en, fa);

        form.Phone.Should().Be("09906720069");
        form.CountryCode.Should().Be("IR");
        form.RoleBadgeEn.Should().Be("Software Engineer");
        form.ExperienceBadgeEn.Should().Be("9+ Years");
        form.LocationEn.Should().Be("Tehran");
        form.CountryEn.Should().Be("Iran");
        form.LocationFa.Should().Be("تهران");
        form.CountryFa.Should().Be("ایران");
    }
}
