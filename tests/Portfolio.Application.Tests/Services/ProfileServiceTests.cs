using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Profiles;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Profiles;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class ProfileServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ProfileService _sut;

    public ProfileServiceTests()
    {
        _sut = new ProfileService(_unitOfWorkMock.Object);
    }

    private static Profile CreateProfile()
    {
        var profile = new Profile
        {
            Id = Guid.NewGuid(),
            Email = "me@example.com",
            GitHubUrl = "https://github.com/x",
            TelegramUrl = "https://t.me/x",
            TwitterUrl = "https://x.com/x",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        profile.Translations.Add(new ProfileTranslation
        {
            Id = Guid.NewGuid(), ProfileId = profile.Id, Language = Language.En,
            FullName = "Mousa Amiri", JobTitle = "Backend Developer", Bio = "bio"
        });
        return profile;
    }

    [Fact]
    public async Task GetPublicAsync_WithProfile_ReturnsMappedDto()
    {
        _unitOfWorkMock.Setup(u => u.Profiles.GetFirstActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateProfile());

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value!.FullName.Should().Be("Mousa Amiri");
        result.Value.Email.Should().Be("me@example.com");
        result.Value.TelegramUrl.Should().Be("https://t.me/x");
        result.Value.TwitterUrl.Should().Be("https://x.com/x");
    }

    [Fact]
    public async Task GetPublicAsync_WhenNoProfile_ReturnsFailure()
    {
        _unitOfWorkMock.Setup(u => u.Profiles.GetFirstActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile?)null);

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task UpsertAsync_WhenNoProfile_CreatesNew()
    {
        Profile? captured = null;
        _unitOfWorkMock.Setup(u => u.Profiles.GetFirstActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile?)null);
        _unitOfWorkMock.Setup(u => u.Profiles.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .Callback<Profile, CancellationToken>((p, _) => captured = p)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new UpsertProfileRequest
        {
            Email = "new@example.com",
            TelegramUrl = "https://t.me/mousaamiri",
            TwitterUrl = "https://x.com/mousaamiri",
            Translations = [new() { LanguageCode = "en", FullName = "N", JobTitle = "Dev" }]
        };

        var result = await _sut.UpsertAsync(request);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.Email.Should().Be("new@example.com");
        captured.TelegramUrl.Should().Be("https://t.me/mousaamiri");
        captured.TwitterUrl.Should().Be("https://x.com/mousaamiri");
        _unitOfWorkMock.Verify(u => u.Profiles.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpsertAsync_WhenProfileExists_UpdatesWithoutAdding()
    {
        var existing = CreateProfile();
        _unitOfWorkMock.Setup(u => u.Profiles.GetFirstActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new UpsertProfileRequest
        {
            Email = "updated@example.com",
            Translations = [new() { LanguageCode = "en", FullName = "Updated", JobTitle = "Dev" }]
        };

        var result = await _sut.UpsertAsync(request);

        result.IsSuccess.Should().BeTrue();
        existing.Email.Should().Be("updated@example.com");
        _unitOfWorkMock.Verify(u => u.Profiles.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpsertAsync_PersistsContactAndLocalizedBadgeFields()
    {
        Profile? captured = null;
        _unitOfWorkMock.Setup(u => u.Profiles.GetFirstActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile?)null);
        _unitOfWorkMock.Setup(u => u.Profiles.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .Callback<Profile, CancellationToken>((p, _) => captured = p)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new UpsertProfileRequest
        {
            Email = "me@example.com",
            Phone = "09906720069",
            CountryCode = "IR",
            Translations =
            [
                new()
                {
                    LanguageCode = "en", FullName = "Mousa", JobTitle = "Dev",
                    RoleBadge = "Software Engineer", ExperienceBadge = "9+ Years",
                    DegreeBadge = "Self-taught", PortraitAlt = "portrait",
                    Location = "Tehran", Country = "Iran"
                }
            ]
        };

        var result = await _sut.UpsertAsync(request);

        result.IsSuccess.Should().BeTrue();
        captured!.Phone.Should().Be("09906720069");
        captured.CountryCode.Should().Be("IR");

        var t = captured.Translations.Single();
        t.RoleBadge.Should().Be("Software Engineer");
        t.ExperienceBadge.Should().Be("9+ Years");
        t.DegreeBadge.Should().Be("Self-taught");
        t.PortraitAlt.Should().Be("portrait");
        t.Location.Should().Be("Tehran");
        t.Country.Should().Be("Iran");
    }

    [Fact]
    public async Task GetPublicAsync_MapsContactAndBadgeFieldsToDto()
    {
        var profile = CreateProfile();
        profile.Phone = "09906720069";
        profile.CountryCode = "IR";
        var t = profile.Translations.First();
        t.RoleBadge = "Software Engineer";
        t.ExperienceBadge = "9+ Years";
        t.Location = "Tehran";
        t.Country = "Iran";

        _unitOfWorkMock.Setup(u => u.Profiles.GetFirstActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Phone.Should().Be("09906720069");
        result.Value.CountryCode.Should().Be("IR");
        result.Value.RoleBadge.Should().Be("Software Engineer");
        result.Value.ExperienceBadge.Should().Be("9+ Years");
        result.Value.Location.Should().Be("Tehran");
        result.Value.Country.Should().Be("Iran");
    }
}
