using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Skills;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Skills;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class SkillServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SkillService _sut;

    public SkillServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new SkillService(_unitOfWorkMock.Object);
    }

    private static Skill CreateSkill(Guid? id = null, string name = "Test Skill", string language = "En")
    {
        var skill = new Skill
        {
            Id = id ?? Guid.NewGuid(),
            IconUrl = "https://example.com/icon.png",
            Proficiency = 85,
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var lang = Enum.Parse<Language>(language, ignoreCase: true);
        skill.Translations.Add(new SkillTranslation
        {
            Id = Guid.NewGuid(),
            SkillId = skill.Id,
            Language = lang,
            Name = name,
            Description = "A test skill",
            Category = "Backend"
        });

        return skill;
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_WithData_ReturnsCorrectlyMappedDtos()
    {
        var skill1 = CreateSkill(name: "C#");
        var skill2 = CreateSkill(name: "Go");
        _unitOfWorkMock.Setup(u => u.Skills.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Skill> { skill1, skill2 });

        var result = await _sut.GetAllAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Name.Should().Be("C#");
        result.Value[1].Name.Should().Be("Go");
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        _unitOfWorkMock.Setup(u => u.Skills.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Skill>());

        var result = await _sut.GetAllAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_MapsAllPropertiesCorrectly()
    {
        var skill = CreateSkill(name: "C#");
        _unitOfWorkMock.Setup(u => u.Skills.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Skill> { skill });

        var result = await _sut.GetAllAsync("en");

        var dto = result.Value![0];
        dto.Id.Should().Be(skill.Id);
        dto.IconUrl.Should().Be(skill.IconUrl);
        dto.Proficiency.Should().Be(skill.Proficiency);
        dto.DisplayOrder.Should().Be(skill.DisplayOrder);
        dto.IsActive.Should().Be(skill.IsActive);
        dto.CreatedAt.Should().Be(skill.CreatedAt);
        dto.Name.Should().Be("C#");
        dto.Description.Should().Be("A test skill");
        dto.Category.Should().Be("Backend");
    }

    [Fact]
    public async Task GetAllAsync_SelectsCorrectLanguageTranslation()
    {
        var skill = new Skill
        {
            Id = Guid.NewGuid(), IconUrl = "icon.png", Proficiency = 90,
            DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow
        };
        skill.Translations.Add(new SkillTranslation
        {
            Id = Guid.NewGuid(), SkillId = skill.Id, Language = Language.En,
            Name = "C#", Description = "C# lang", Category = "Backend"
        });
        skill.Translations.Add(new SkillTranslation
        {
            Id = Guid.NewGuid(), SkillId = skill.Id, Language = Language.Fa,
            Name = "سی‌شارپ", Description = "زبان سی‌شارپ", Category = "بک‌اند"
        });

        _unitOfWorkMock.Setup(u => u.Skills.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Skill> { skill });

        var result = await _sut.GetAllAsync("fa");

        result.Value![0].Name.Should().Be("سی‌شارپ");
    }

    [Fact]
    public async Task GetAllAsync_WhenTranslationMissing_ReturnsEmptyStrings()
    {
        var skill = CreateSkill(language: "En");
        _unitOfWorkMock.Setup(u => u.Skills.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Skill> { skill });

        var result = await _sut.GetAllAsync("ar");

        result.Value![0].Name.Should().BeEmpty();
    }

    #endregion

    #region GetPublicAsync

    [Fact]
    public async Task GetPublicAsync_UsesActiveWithTranslationsRepositoryMethod()
    {
        var skill = CreateSkill(name: "Public Skill");
        _unitOfWorkMock.Setup(u => u.Skills.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Skill> { skill });

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value![0].Name.Should().Be("Public Skill");
        _unitOfWorkMock.Verify(u => u.Skills.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
    {
        var skillId = Guid.NewGuid();
        var skill = CreateSkill(id: skillId, name: "Found Skill");
        _unitOfWorkMock.Setup(u => u.Skills.GetByIdWithTranslationsAsync(skillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(skill);

        var result = await _sut.GetByIdAsync(skillId, "en");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(skillId);
        result.Value.Name.Should().Be("Found Skill");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Skills.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Skill?)null);

        var result = await _sut.GetByIdAsync(id, "en");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidRequest_CallsAddAndSaveChanges()
    {
        var request = new CreateSkillRequest
        {
            IconUrl = "icon.png", Proficiency = 80, DisplayOrder = 1,
            Translations = [new SkillTranslationRequest { LanguageCode = "en", Name = "C#", Description = "Desc", Category = "Backend" }]
        };
        _unitOfWorkMock.Setup(u => u.Skills.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _unitOfWorkMock.Verify(u => u.Skills.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_MapsRequestToEntityCorrectly()
    {
        Skill? captured = null;
        _unitOfWorkMock.Setup(u => u.Skills.AddAsync(It.IsAny<Skill>(), It.IsAny<CancellationToken>()))
            .Callback<Skill, CancellationToken>((s, _) => captured = s).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new CreateSkillRequest
        {
            IconUrl = "icon.png", Proficiency = 95, DisplayOrder = 3,
            Translations =
            [
                new SkillTranslationRequest { LanguageCode = "en", Name = "Go", Description = "Go lang", Category = "Backend" },
                new SkillTranslationRequest { LanguageCode = "fa", Name = "گو", Description = "زبان گو", Category = "بک‌اند" }
            ]
        };

        await _sut.CreateAsync(request);

        captured.Should().NotBeNull();
        captured!.IconUrl.Should().Be("icon.png");
        captured.Proficiency.Should().Be(95);
        captured.DisplayOrder.Should().Be(3);
        captured.Translations.Should().HaveCount(2);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesAndSaves()
    {
        var skillId = Guid.NewGuid();
        var existing = CreateSkill(id: skillId);
        _unitOfWorkMock.Setup(u => u.Skills.GetByIdWithTranslationsAsync(skillId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new UpdateSkillRequest
        {
            IconUrl = "new.png", Proficiency = 50, DisplayOrder = 2, IsActive = false,
            Translations = [new SkillTranslationRequest { LanguageCode = "en", Name = "Updated", Description = "Desc", Category = "Frontend" }]
        };

        var result = await _sut.UpdateAsync(skillId, request);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Skills.GetByIdWithTranslationsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Skill?)null);

        var request = new UpdateSkillRequest { IconUrl = "x", Proficiency = 1, DisplayOrder = 1, IsActive = true, Translations = [] };
        var result = await _sut.UpdateAsync(id, request);

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_MapsFieldsCorrectly()
    {
        var skillId = Guid.NewGuid();
        var existing = CreateSkill(id: skillId);
        _unitOfWorkMock.Setup(u => u.Skills.GetByIdWithTranslationsAsync(skillId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new UpdateSkillRequest
        {
            IconUrl = "updated.png", Proficiency = 100, DisplayOrder = 99, IsActive = false,
            Translations = [new SkillTranslationRequest { LanguageCode = "en", Name = "Updated", Description = "New", Category = "DevOps" }]
        };

        await _sut.UpdateAsync(skillId, request);

        existing.IconUrl.Should().Be("updated.png");
        existing.Proficiency.Should().Be(100);
        existing.DisplayOrder.Should().Be(99);
        existing.IsActive.Should().BeFalse();
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesAndSaves()
    {
        var skillId = Guid.NewGuid();
        var existing = CreateSkill(id: skillId);
        _unitOfWorkMock.Setup(u => u.Skills.GetByIdAsync(skillId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.DeleteAsync(skillId);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.Skills.Delete(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Skills.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Skill?)null);

        var result = await _sut.DeleteAsync(id);

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.Skills.Delete(It.IsAny<Skill>()), Times.Never);
    }

    #endregion
}