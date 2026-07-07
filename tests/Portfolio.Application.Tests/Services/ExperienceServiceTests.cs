using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Experiences;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Experiences;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class ExperienceServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExperienceService _sut;

    public ExperienceServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new ExperienceService(_unitOfWorkMock.Object);
    }

    private static Experience CreateExperience(Guid? id = null, string companyName = "Test Co", string language = "En")
    {
        var experience = new Experience
        {
            Id = id ?? Guid.NewGuid(),
            CompanyLogo = "https://example.com/logo.png",
            CompanyUrl = "https://example.com",
            StartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var lang = Enum.Parse<Language>(language, ignoreCase: true);
        experience.Translations.Add(new ExperienceTranslation
        {
            Id = Guid.NewGuid(),
            ExperienceId = experience.Id,
            Language = lang,
            CompanyName = companyName,
            JobTitle = "Software Engineer",
            Description = "Test description",
            Location = "Tehran"
        });

        return experience;
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_WithData_ReturnsCorrectlyMappedDtos()
    {
        var exp1 = CreateExperience(companyName: "Company A");
        var exp2 = CreateExperience(companyName: "Company B");
        _unitOfWorkMock.Setup(u => u.Experiences.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Experience> { exp1, exp2 });

        var result = await _sut.GetAllAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].CompanyName.Should().Be("Company A");
        result.Value[1].CompanyName.Should().Be("Company B");
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        _unitOfWorkMock.Setup(u => u.Experiences.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Experience>());

        var result = await _sut.GetAllAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_MapsAllPropertiesCorrectly()
    {
        var exp = CreateExperience(companyName: "Mapped Co");
        _unitOfWorkMock.Setup(u => u.Experiences.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Experience> { exp });

        var result = await _sut.GetAllAsync("en");

        var dto = result.Value![0];
        dto.Id.Should().Be(exp.Id);
        dto.CompanyLogo.Should().Be(exp.CompanyLogo);
        dto.CompanyUrl.Should().Be(exp.CompanyUrl);
        dto.StartDate.Should().Be(exp.StartDate);
        dto.EndDate.Should().Be(exp.EndDate);
        dto.IsActive.Should().Be(exp.IsActive);
        dto.CreatedAt.Should().Be(exp.CreatedAt);
        dto.CompanyName.Should().Be("Mapped Co");
        dto.JobTitle.Should().Be("Software Engineer");
        dto.Description.Should().Be("Test description");
        dto.Location.Should().Be("Tehran");
    }

    [Fact]
    public async Task GetAllAsync_SelectsCorrectLanguageTranslation()
    {
        var exp = new Experience
        {
            Id = Guid.NewGuid(), CompanyLogo = "logo.png", CompanyUrl = "url.com",
            StartDate = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow
        };
        exp.Translations.Add(new ExperienceTranslation
        {
            Id = Guid.NewGuid(), ExperienceId = exp.Id, Language = Language.En,
            CompanyName = "English Co", JobTitle = "Engineer", Description = "Desc", Location = "London"
        });
        exp.Translations.Add(new ExperienceTranslation
        {
            Id = Guid.NewGuid(), ExperienceId = exp.Id, Language = Language.Fa,
            CompanyName = "شرکت فارسی", JobTitle = "مهندس", Description = "توضیحات", Location = "تهران"
        });

        _unitOfWorkMock.Setup(u => u.Experiences.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Experience> { exp });

        var result = await _sut.GetAllAsync("fa");

        result.Value![0].CompanyName.Should().Be("شرکت فارسی");
        result.Value[0].JobTitle.Should().Be("مهندس");
    }

    [Fact]
    public async Task GetAllAsync_WhenTranslationMissing_ReturnsEmptyStrings()
    {
        var exp = CreateExperience(language: "En");
        _unitOfWorkMock.Setup(u => u.Experiences.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Experience> { exp });

        var result = await _sut.GetAllAsync("ar");

        result.Value![0].CompanyName.Should().BeEmpty();
        result.Value[0].JobTitle.Should().BeEmpty();
    }

    #endregion

    #region GetPublicAsync

    [Fact]
    public async Task GetPublicAsync_UsesActiveWithTranslationsRepositoryMethod()
    {
        var exp = CreateExperience(companyName: "Public Co");
        _unitOfWorkMock.Setup(u => u.Experiences.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Experience> { exp });

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value![0].CompanyName.Should().Be("Public Co");
        _unitOfWorkMock.Verify(u => u.Experiences.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
    {
        var expId = Guid.NewGuid();
        var exp = CreateExperience(id: expId, companyName: "Found Co");
        _unitOfWorkMock.Setup(u => u.Experiences.GetByIdWithTranslationsAsync(expId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exp);

        var result = await _sut.GetByIdAsync(expId, "en");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(expId);
        result.Value.CompanyName.Should().Be("Found Co");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Experiences.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Experience?)null);

        var result = await _sut.GetByIdAsync(id, "en");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidRequest_CallsAddAndSaveChanges()
    {
        var request = new CreateExperienceRequest
        {
            CompanyLogo = "logo.png", CompanyUrl = "url.com",
            StartDate = DateTime.UtcNow,
            Translations = [new ExperienceTranslationRequest { LanguageCode = "en", CompanyName = "Co", JobTitle = "Dev", Description = "Desc", Location = "City" }]
        };
        _unitOfWorkMock.Setup(u => u.Experiences.AddAsync(It.IsAny<Experience>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _unitOfWorkMock.Verify(u => u.Experiences.AddAsync(It.IsAny<Experience>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_MapsRequestToEntityCorrectly()
    {
        Experience? captured = null;
        _unitOfWorkMock.Setup(u => u.Experiences.AddAsync(It.IsAny<Experience>(), It.IsAny<CancellationToken>()))
            .Callback<Experience, CancellationToken>((e, _) => captured = e).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var startDate = new DateTime(2023, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateExperienceRequest
        {
            CompanyLogo = "logo.png", CompanyUrl = "url.com", StartDate = startDate, EndDate = null,
            Translations =
            [
                new ExperienceTranslationRequest { LanguageCode = "en", CompanyName = "Co", JobTitle = "Dev", Description = "Desc", Location = "City" },
                new ExperienceTranslationRequest { LanguageCode = "fa", CompanyName = "شرکت", JobTitle = "برنامه‌نویس", Description = "توضیحات", Location = "تهران" }
            ]
        };

        await _sut.CreateAsync(request);

        captured.Should().NotBeNull();
        captured!.CompanyLogo.Should().Be("logo.png");
        captured.CompanyUrl.Should().Be("url.com");
        captured.StartDate.Should().Be(startDate);
        captured.EndDate.Should().BeNull();
        captured.Translations.Should().HaveCount(2);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesAndSaves()
    {
        var expId = Guid.NewGuid();
        var existing = CreateExperience(id: expId);
        _unitOfWorkMock.Setup(u => u.Experiences.GetByIdWithTranslationsAsync(expId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new UpdateExperienceRequest
        {
            CompanyLogo = "new.png", CompanyUrl = "new.com", StartDate = DateTime.UtcNow, IsActive = false,
            Translations = [new ExperienceTranslationRequest { LanguageCode = "en", CompanyName = "New Co", JobTitle = "Sr Dev", Description = "New Desc", Location = "NY" }]
        };

        var result = await _sut.UpdateAsync(expId, request);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Experiences.GetByIdWithTranslationsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Experience?)null);

        var request = new UpdateExperienceRequest
        {
            CompanyLogo = "x", CompanyUrl = "x", StartDate = DateTime.UtcNow, IsActive = true, Translations = []
        };
        var result = await _sut.UpdateAsync(id, request);

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_MapsFieldsCorrectly()
    {
        var expId = Guid.NewGuid();
        var existing = CreateExperience(id: expId);
        _unitOfWorkMock.Setup(u => u.Experiences.GetByIdWithTranslationsAsync(expId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var newStart = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var newEnd = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var request = new UpdateExperienceRequest
        {
            CompanyLogo = "updated.png", CompanyUrl = "updated.com",
            StartDate = newStart, EndDate = newEnd, IsActive = false,
            Translations = [new ExperienceTranslationRequest { LanguageCode = "en", CompanyName = "Updated", JobTitle = "CTO", Description = "New", Location = "SF" }]
        };

        await _sut.UpdateAsync(expId, request);

        existing.CompanyLogo.Should().Be("updated.png");
        existing.CompanyUrl.Should().Be("updated.com");
        existing.StartDate.Should().Be(newStart);
        existing.EndDate.Should().Be(newEnd);
        existing.IsActive.Should().BeFalse();
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesAndSaves()
    {
        var expId = Guid.NewGuid();
        var existing = CreateExperience(id: expId);
        _unitOfWorkMock.Setup(u => u.Experiences.GetByIdAsync(expId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.DeleteAsync(expId);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.Experiences.Delete(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Experiences.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Experience?)null);

        var result = await _sut.DeleteAsync(id);

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.Experiences.Delete(It.IsAny<Experience>()), Times.Never);
    }

    #endregion
}