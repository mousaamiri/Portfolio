using FluentAssertions;
using Moq;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProjectService _sut;

    public ProjectServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new ProjectService(_unitOfWorkMock.Object);
    }

    private static Project CreateProject(Guid? id = null, string title = "Test Project", string language = "En")
    {
        var project = new Project
        {
            Id = id ?? Guid.NewGuid(),
            ImageUrl = "https://example.com/image.png",
            DemoUrl = "https://example.com/demo",
            SourceCodeUrl = "https://github.com/test",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var lang = Enum.Parse<Language>(language, ignoreCase: true);
        project.Translations.Add(new ProjectTranslation
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            Language = lang,
            Title = title,
            Description = "A test project",
            Technologies = "C#, .NET"
        });

        return project;
    }

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_WithData_ReturnsCorrectlyMappedDtos()
    {
        var project1 = CreateProject(title: "Project One");
        var project2 = CreateProject(title: "Project Two");
        var projects = new List<Project> { project1, project2 };

        _unitOfWorkMock.Setup(u => u.Projects.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        var result = await _sut.GetAllAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Title.Should().Be("Project One");
        result.Value[1].Title.Should().Be("Project Two");
    }

    [Fact]
    public async Task GetAllAsync_WithData_MapsAllPropertiesCorrectly()
    {
        var project = CreateProject(title: "Mapped Project");
        _unitOfWorkMock.Setup(u => u.Projects.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project> { project });

        var result = await _sut.GetAllAsync("en");

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value![0];
        dto.Id.Should().Be(project.Id);
        dto.ImageUrl.Should().Be(project.ImageUrl);
        dto.DemoUrl.Should().Be(project.DemoUrl);
        dto.SourceCodeUrl.Should().Be(project.SourceCodeUrl);
        dto.DisplayOrder.Should().Be(project.DisplayOrder);
        dto.IsActive.Should().Be(project.IsActive);
        dto.CreatedAt.Should().Be(project.CreatedAt);
        dto.Title.Should().Be("Mapped Project");
        dto.Description.Should().Be("A test project");
        dto.Technologies.Should().Be("C#, .NET");
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsEmpty_ReturnsEmptyList()
    {
        _unitOfWorkMock.Setup(u => u.Projects.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>());

        var result = await _sut.GetAllAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_SelectsCorrectLanguageTranslation()
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            ImageUrl = "img.png",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        project.Translations.Add(new ProjectTranslation
        {
            Id = Guid.NewGuid(), ProjectId = project.Id, Language = Language.En,
            Title = "English Title", Description = "English Desc", Technologies = "C#"
        });
        project.Translations.Add(new ProjectTranslation
        {
            Id = Guid.NewGuid(), ProjectId = project.Id, Language = Language.Fa,
            Title = "عنوان فارسی", Description = "توضیحات فارسی", Technologies = "سی‌شارپ"
        });

        _unitOfWorkMock.Setup(u => u.Projects.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project> { project });

        var result = await _sut.GetAllAsync("fa");

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Title.Should().Be("عنوان فارسی");
        result.Value[0].Description.Should().Be("توضیحات فارسی");
    }

    [Fact]
    public async Task GetAllAsync_WhenTranslationMissing_ReturnsEmptyStrings()
    {
        var project = CreateProject(language: "En");
        _unitOfWorkMock.Setup(u => u.Projects.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project> { project });

        var result = await _sut.GetAllAsync("ar");

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Title.Should().BeEmpty();
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
    {
        var projectId = Guid.NewGuid();
        var project = CreateProject(id: projectId, title: "Found Project");

        _unitOfWorkMock.Setup(u => u.Projects.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await _sut.GetByIdAsync(projectId, "en");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(projectId);
        result.Value.Title.Should().Be("Found Project");
        result.Value.ImageUrl.Should().Be(project.ImageUrl);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsFailure()
    {
        var nonExistentId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Projects.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var result = await _sut.GetByIdAsync(nonExistentId, "en");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_SelectsCorrectLanguageTranslation()
    {
        var projectId = Guid.NewGuid();
        var project = new Project
        {
            Id = projectId, ImageUrl = "img.png", DisplayOrder = 1,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        project.Translations.Add(new ProjectTranslation
        {
            Id = Guid.NewGuid(), ProjectId = projectId, Language = Language.En,
            Title = "English", Description = "Desc EN", Technologies = "C#"
        });
        project.Translations.Add(new ProjectTranslation
        {
            Id = Guid.NewGuid(), ProjectId = projectId, Language = Language.Fa,
            Title = "فارسی", Description = "توضیحات", Technologies = "سی‌شارپ"
        });

        _unitOfWorkMock.Setup(u => u.Projects.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await _sut.GetByIdAsync(projectId, "fa");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("فارسی");
    }

    #endregion
}