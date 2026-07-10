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

    // ── Admin CRUD ──

    [Fact]
    public async Task GetAllAsync_ReturnsMappedRows()
    {
        _unitOfWorkMock.Setup(u => u.UiTranslations.GetAllOrderedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UiTranslation> { Row("nav.home", "خانه") });

        var result = await _sut.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value![0].Key.Should().Be("nav.home");
        result.Value[0].Language.Should().Be("fa");
    }

    [Fact]
    public async Task CreateAsync_NewKey_PersistsAndReturnsId()
    {
        _unitOfWorkMock.Setup(u => u.UiTranslations.GetByKeyAsync("nav.home", Language.Fa, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UiTranslation?)null);
        _unitOfWorkMock.Setup(u => u.UiTranslations.AddAsync(It.IsAny<UiTranslation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(new() { Key = "nav.home", LanguageCode = "fa", Value = "خانه" });

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.UiTranslations.AddAsync(It.IsAny<UiTranslation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DuplicateKeyLanguage_ReturnsFailure()
    {
        _unitOfWorkMock.Setup(u => u.UiTranslations.GetByKeyAsync("nav.home", Language.Fa, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Row("nav.home", "خانه"));

        var result = await _sut.CreateAsync(new() { Key = "nav.home", LanguageCode = "fa", Value = "خانه" });

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.UiTranslations.AddAsync(It.IsAny<UiTranslation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ExistingRow_UpdatesValue()
    {
        var row = Row("nav.home", "خانه");
        _unitOfWorkMock.Setup(u => u.UiTranslations.GetByIdAsync(row.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(row);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.UpdateAsync(row.Id, new() { Key = "nav.home", LanguageCode = "fa", Value = "خانهٔ نو" });

        result.IsSuccess.Should().BeTrue();
        row.Value.Should().Be("خانهٔ نو");
    }

    [Fact]
    public async Task UpdateAsync_MissingId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.UiTranslations.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UiTranslation?)null);

        var result = await _sut.UpdateAsync(id, new() { Key = "nav.home", LanguageCode = "fa", Value = "x" });

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_RenameOntoExistingKey_ReturnsFailure()
    {
        var row = Row("nav.home", "خانه");
        var clash = Row("nav.about", "درباره");
        _unitOfWorkMock.Setup(u => u.UiTranslations.GetByIdAsync(row.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(row);
        _unitOfWorkMock.Setup(u => u.UiTranslations.GetByKeyAsync("nav.about", Language.Fa, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clash);

        var result = await _sut.UpdateAsync(row.Id, new() { Key = "nav.about", LanguageCode = "fa", Value = "x" });

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistingRow_Deletes()
    {
        var row = Row("nav.home", "خانه");
        _unitOfWorkMock.Setup(u => u.UiTranslations.GetByIdAsync(row.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(row);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.DeleteAsync(row.Id);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.UiTranslations.Delete(row), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_MissingId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.UiTranslations.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UiTranslation?)null);

        var result = await _sut.DeleteAsync(id);

        result.IsSuccess.Should().BeFalse();
    }
}
