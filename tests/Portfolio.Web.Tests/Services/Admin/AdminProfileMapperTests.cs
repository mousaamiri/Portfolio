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
}
