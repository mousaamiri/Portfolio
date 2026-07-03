using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Educations;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Educations;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class EducationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly EducationService _sut;

    public EducationServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new EducationService(_unitOfWorkMock.Object);
    }

    private static Education CreateEducation(Guid? id = null, string institutionName = "Test University", string language = "En")
    {
        var education = new Education
        {
            Id = id ?? Guid.NewGuid(),
            InstitutionLogo = "https://example.com/logo.png",
            InstitutionUrl = "https://example.com",
            StartDate = new DateTime(2020, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            Gpa = 3.8,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var lang = Enum.Parse<Language>(language, ignoreCase: true);
        education.Translations.Add(new EducationTranslation
        {
            Id = Guid.NewGuid(),
            EducationId = education.Id,
            Language = lang,
            InstitutionName = institutionName,
            Degree = "Bachelor",
            FieldOfStudy = "Computer Science",
            Description = "Test description"
        });

        return education;
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_WithData_ReturnsCorrectlyMappedDtos()
    {
        var edu1 = CreateEducation(institutionName: "MIT");
        var edu2 = CreateEducation(institutionName: "Stanford");
        _unitOfWorkMock.Setup(u => u.Educations.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Education> { edu1, edu2 });

        var result = await _sut.GetAllAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].InstitutionName.Should().Be("MIT");
        result.Value[1].InstitutionName.Should().Be("Stanford");
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        _unitOfWorkMock.Setup(u => u.Educations.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Education>());

        var result = await _sut.GetAllAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_MapsAllPropertiesCorrectly()
    {
        var edu = CreateEducation(institutionName: "Mapped Uni");
        _unitOfWorkMock.Setup(u => u.Educations.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Education> { edu });

        var result = await _sut.GetAllAsync("en");

        var dto = result.Value![0];
        dto.Id.Should().Be(edu.Id);
        dto.InstitutionLogo.Should().Be(edu.InstitutionLogo);
        dto.InstitutionUrl.Should().Be(edu.InstitutionUrl);
        dto.StartDate.Should().Be(edu.StartDate);
        dto.EndDate.Should().Be(edu.EndDate);
        dto.Gpa.Should().Be(edu.Gpa);
        dto.IsActive.Should().Be(edu.IsActive);
        dto.CreatedAt.Should().Be(edu.CreatedAt);
        dto.InstitutionName.Should().Be("Mapped Uni");
        dto.Degree.Should().Be("Bachelor");
        dto.FieldOfStudy.Should().Be("Computer Science");
        dto.Description.Should().Be("Test description");
    }

    [Fact]
    public async Task GetAllAsync_SelectsCorrectLanguageTranslation()
    {
        var edu = new Education
        {
            Id = Guid.NewGuid(), InstitutionLogo = "logo.png", InstitutionUrl = "url.com",
            StartDate = DateTime.UtcNow, Gpa = 4.0, IsActive = true, CreatedAt = DateTime.UtcNow
        };
        edu.Translations.Add(new EducationTranslation
        {
            Id = Guid.NewGuid(), EducationId = edu.Id, Language = Language.En,
            InstitutionName = "MIT", Degree = "BSc", FieldOfStudy = "CS", Description = "Desc"
        });
        edu.Translations.Add(new EducationTranslation
        {
            Id = Guid.NewGuid(), EducationId = edu.Id, Language = Language.Fa,
            InstitutionName = "دانشگاه صنعتی", Degree = "کارشناسی", FieldOfStudy = "کامپیوتر", Description = "توضیحات"
        });

        _unitOfWorkMock.Setup(u => u.Educations.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Education> { edu });

        var result = await _sut.GetAllAsync("fa");

        result.Value![0].InstitutionName.Should().Be("دانشگاه صنعتی");
        result.Value[0].Degree.Should().Be("کارشناسی");
    }

    [Fact]
    public async Task GetAllAsync_WhenTranslationMissing_ReturnsEmptyStrings()
    {
        var edu = CreateEducation(language: "En");
        _unitOfWorkMock.Setup(u => u.Educations.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Education> { edu });

        var result = await _sut.GetAllAsync("ar");

        result.Value![0].InstitutionName.Should().BeEmpty();
        result.Value[0].Degree.Should().BeEmpty();
        result.Value[0].FieldOfStudy.Should().BeEmpty();
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
    {
        var eduId = Guid.NewGuid();
        var edu = CreateEducation(id: eduId, institutionName: "Found Uni");
        _unitOfWorkMock.Setup(u => u.Educations.GetByIdAsync(eduId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(edu);

        var result = await _sut.GetByIdAsync(eduId, "en");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(eduId);
        result.Value.InstitutionName.Should().Be("Found Uni");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Educations.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Education?)null);

        var result = await _sut.GetByIdAsync(id, "en");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidRequest_CallsAddAndSaveChanges()
    {
        var request = new CreateEducationRequest
        {
            InstitutionLogo = "logo.png", InstitutionUrl = "url.com",
            StartDate = DateTime.UtcNow, Gpa = 3.9,
            Translations = [new EducationTranslationRequest { LanguageCode = "en", InstitutionName = "MIT", Degree = "BSc", FieldOfStudy = "CS", Description = "Desc" }]
        };
        _unitOfWorkMock.Setup(u => u.Educations.AddAsync(It.IsAny<Education>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _unitOfWorkMock.Verify(u => u.Educations.AddAsync(It.IsAny<Education>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_MapsRequestToEntityCorrectly()
    {
        Education? captured = null;
        _unitOfWorkMock.Setup(u => u.Educations.AddAsync(It.IsAny<Education>(), It.IsAny<CancellationToken>()))
            .Callback<Education, CancellationToken>((e, _) => captured = e).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var startDate = new DateTime(2020, 9, 1, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateEducationRequest
        {
            InstitutionLogo = "logo.png", InstitutionUrl = "url.com",
            StartDate = startDate, EndDate = null, Gpa = 3.5,
            Translations =
            [
                new EducationTranslationRequest { LanguageCode = "en", InstitutionName = "MIT", Degree = "BSc", FieldOfStudy = "CS", Description = "Desc" },
                new EducationTranslationRequest { LanguageCode = "fa", InstitutionName = "دانشگاه", Degree = "کارشناسی", FieldOfStudy = "کامپیوتر", Description = "توضیحات" }
            ]
        };

        await _sut.CreateAsync(request);

        captured.Should().NotBeNull();
        captured!.InstitutionLogo.Should().Be("logo.png");
        captured.InstitutionUrl.Should().Be("url.com");
        captured.StartDate.Should().Be(startDate);
        captured.EndDate.Should().BeNull();
        captured.Gpa.Should().Be(3.5);
        captured.Translations.Should().HaveCount(2);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesAndSaves()
    {
        var eduId = Guid.NewGuid();
        var existing = CreateEducation(id: eduId);
        _unitOfWorkMock.Setup(u => u.Educations.GetByIdWithTranslationsAsync(eduId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new UpdateEducationRequest
        {
            InstitutionLogo = "new.png", InstitutionUrl = "new.com",
            StartDate = DateTime.UtcNow, Gpa = 4.0, IsActive = false,
            Translations = [new EducationTranslationRequest { LanguageCode = "en", InstitutionName = "Updated", Degree = "MSc", FieldOfStudy = "AI", Description = "New" }]
        };

        var result = await _sut.UpdateAsync(eduId, request);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Educations.GetByIdWithTranslationsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Education?)null);

        var request = new UpdateEducationRequest
        {
            InstitutionLogo = "x", InstitutionUrl = "x", StartDate = DateTime.UtcNow, IsActive = true, Translations = []
        };
        var result = await _sut.UpdateAsync(id, request);

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_MapsFieldsCorrectly()
    {
        var eduId = Guid.NewGuid();
        var existing = CreateEducation(id: eduId);
        _unitOfWorkMock.Setup(u => u.Educations.GetByIdWithTranslationsAsync(eduId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var newStart = new DateTime(2021, 9, 1, 0, 0, 0, DateTimeKind.Utc);
        var newEnd = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var request = new UpdateEducationRequest
        {
            InstitutionLogo = "updated.png", InstitutionUrl = "updated.com",
            StartDate = newStart, EndDate = newEnd, Gpa = 3.2, IsActive = false,
            Translations = [new EducationTranslationRequest { LanguageCode = "en", InstitutionName = "Updated", Degree = "PhD", FieldOfStudy = "ML", Description = "New" }]
        };

        await _sut.UpdateAsync(eduId, request);

        existing.InstitutionLogo.Should().Be("updated.png");
        existing.InstitutionUrl.Should().Be("updated.com");
        existing.StartDate.Should().Be(newStart);
        existing.EndDate.Should().Be(newEnd);
        existing.Gpa.Should().Be(3.2);
        existing.IsActive.Should().BeFalse();
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesAndSaves()
    {
        var eduId = Guid.NewGuid();
        var existing = CreateEducation(id: eduId);
        _unitOfWorkMock.Setup(u => u.Educations.GetByIdAsync(eduId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.DeleteAsync(eduId);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.Educations.Delete(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Educations.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Education?)null);

        var result = await _sut.DeleteAsync(id);

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.Educations.Delete(It.IsAny<Education>()), Times.Never);
    }

    #endregion
}