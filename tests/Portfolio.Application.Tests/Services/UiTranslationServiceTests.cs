using FluentAssertions;
using Moq;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.UiTranslations;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class UiTranslationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly UiTranslationService _sut;

    public UiTranslationServiceTests()
    {
        _sut = new UiTranslationService(_unitOfWorkMock.Object);
    }

    private static UiTranslation Row(string key, string value, Language lang = Language.Fa) => new()
    {
        Id = Guid.NewGuid(), Key = key, Language = lang, Value = value, IsActive = true, CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task GetMapAsync_English_ReturnsEmptyMap_WithoutHittingRepository()
    {
        var result = await _sut.GetMapAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        _unitOfWorkMock.Verify(
            u => u.UiTranslations.GetByLanguageAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetMapAsync_Farsi_ReturnsKeyValueMap()
    {
        _unitOfWorkMock
            .Setup(u => u.UiTranslations.GetByLanguageAsync(Language.Fa, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UiTranslation> { Row("nav.home", "خانه"), Row("nav.about", "درباره من") });

        var result = await _sut.GetMapAsync("fa");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(2);
        result.Value["nav.home"].Should().Be("خانه");
        result.Value["nav.about"].Should().Be("درباره من");
    }

    [Fact]
    public async Task GetMapAsync_UnknownLanguage_FallsBackToEnglishEmptyMap()
    {
        var result = await _sut.GetMapAsync("zz");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        _unitOfWorkMock.Verify(
            u => u.UiTranslations.GetByLanguageAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
