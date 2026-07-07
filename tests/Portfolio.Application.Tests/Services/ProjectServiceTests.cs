using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Projects;
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
            Slug = "test-project",
            ThumbnailUrl = "https://example.com/image.png",
            PreviewUrl = "https://example.com/demo",
            SourceCodeUrl = "https://github.com/test",
            IsPublished = true,
            StartedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
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
            ShortDescription = "A short subtitle",
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

        _unitOfWorkMock.Setup(u => u.Projects.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
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
        _unitOfWorkMock.Setup(u => u.Projects.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project> { project });

        var result = await _sut.GetAllAsync("en");

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value![0];
        dto.Id.Should().Be(project.Id);
        dto.Slug.Should().Be(project.Slug);
        dto.ThumbnailUrl.Should().Be(project.ThumbnailUrl);
        dto.PreviewUrl.Should().Be(project.PreviewUrl);
        dto.SourceCodeUrl.Should().Be(project.SourceCodeUrl);
        dto.IsPublished.Should().Be(project.IsPublished);
        dto.StartedAt.Should().Be(project.StartedAt);
        dto.DisplayOrder.Should().Be(project.DisplayOrder);
        dto.IsActive.Should().Be(project.IsActive);
        dto.CreatedAt.Should().Be(project.CreatedAt);
        dto.Title.Should().Be("Mapped Project");
        dto.ShortDescription.Should().Be("A short subtitle");
        dto.Description.Should().Be("A test project");
        dto.Technologies.Should().Be("C#, .NET");
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsEmpty_ReturnsEmptyList()
    {
        _unitOfWorkMock.Setup(u => u.Projects.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
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
            ThumbnailUrl = "img.png",
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

        _unitOfWorkMock.Setup(u => u.Projects.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
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
        _unitOfWorkMock.Setup(u => u.Projects.GetAllWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project> { project });

        var result = await _sut.GetAllAsync("ar");

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Title.Should().BeEmpty();
    }

    #endregion

    #region GetPublicAsync

    [Fact]
    public async Task GetPublicAsync_UsesActiveWithTranslationsRepositoryMethod()
    {
        var project = CreateProject(title: "Public Project");
        _unitOfWorkMock.Setup(u => u.Projects.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project> { project });

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value![0].Title.Should().Be("Public Project");
        _unitOfWorkMock.Verify(u => u.Projects.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCorrectDto()
    {
        var projectId = Guid.NewGuid();
        var project = CreateProject(id: projectId, title: "Found Project");

        _unitOfWorkMock.Setup(u => u.Projects.GetByIdWithTranslationsAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await _sut.GetByIdAsync(projectId, "en");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(projectId);
        result.Value.Title.Should().Be("Found Project");
        result.Value.ThumbnailUrl.Should().Be(project.ThumbnailUrl);
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
            Id = projectId, ThumbnailUrl = "img.png", DisplayOrder = 1,
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

        _unitOfWorkMock.Setup(u => u.Projects.GetByIdWithTranslationsAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await _sut.GetByIdAsync(projectId, "fa");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("فارسی");
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithValidRequest_CallsAddAsyncAndSaveChanges()
    {
        var request = new CreateProjectRequest
        {
            Slug = "my-project",
            ThumbnailUrl = "https://example.com/image.png",
            PreviewUrl = "https://example.com/demo",
            SourceCodeUrl = "https://github.com/test",
            DisplayOrder = 1,
            Translations =
            [
                new ProjectTranslationRequest
                {
                    LanguageCode = "en", Title = "My Project",
                    Description = "Desc", Technologies = "C#"
                }
            ]
        };

        _unitOfWorkMock.Setup(u => u.Projects.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _unitOfWorkMock.Verify(u => u.Projects.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_MapsRequestToEntityCorrectly()
    {
        Project? capturedProject = null;
        _unitOfWorkMock.Setup(u => u.Projects.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Callback<Project, CancellationToken>((p, _) => capturedProject = p)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new CreateProjectRequest
        {
            Slug = "title-en",
            ThumbnailUrl = "img.png",
            PreviewUrl = "demo.com",
            SourceCodeUrl = "github.com",
            IsClientProject = true,
            IsFeatured = true,
            DisplayOrder = 5,
            Translations =
            [
                new ProjectTranslationRequest
                {
                    LanguageCode = "en", Title = "Title EN", ShortDescription = "Sub EN",
                    Description = "Desc EN", Technologies = "C#"
                },
                new ProjectTranslationRequest
                {
                    LanguageCode = "fa", Title = "عنوان",
                    Description = "توضیحات", Technologies = "سی‌شارپ"
                }
            ]
        };

        await _sut.CreateAsync(request);

        capturedProject.Should().NotBeNull();
        capturedProject!.Slug.Should().Be("title-en");
        capturedProject.ThumbnailUrl.Should().Be("img.png");
        capturedProject.PreviewUrl.Should().Be("demo.com");
        capturedProject.SourceCodeUrl.Should().Be("github.com");
        capturedProject.IsClientProject.Should().BeTrue();
        capturedProject.IsFeatured.Should().BeTrue();
        capturedProject.DisplayOrder.Should().Be(5);
        capturedProject.Translations.Should().HaveCount(2);
        capturedProject.Translations.First().ShortDescription.Should().Be("Sub EN");
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesEntityAndSavesChanges()
    {
        var projectId = Guid.NewGuid();
        var existingProject = CreateProject(id: projectId);

        _unitOfWorkMock.Setup(u => u.Projects.GetByIdWithTranslationsAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProject);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new UpdateProjectRequest
        {
            Slug = "updated-slug",
            ThumbnailUrl = "new-image.png",
            PreviewUrl = "new-demo.com",
            SourceCodeUrl = "new-github.com",
            DisplayOrder = 10,
            IsActive = false,
            Translations =
            [
                new ProjectTranslationRequest
                {
                    LanguageCode = "en", Title = "Updated Title",
                    Description = "Updated Desc", Technologies = "Go"
                }
            ]
        };

        var result = await _sut.UpdateAsync(projectId, request);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ReturnsFailure()
    {
        var nonExistentId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Projects.GetByIdWithTranslationsAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var request = new UpdateProjectRequest
        {
            Slug = "slug", ThumbnailUrl = "img.png", DisplayOrder = 1, IsActive = true, Translations = []
        };

        var result = await _sut.UpdateAsync(nonExistentId, request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_MapsRequestFieldsToEntity()
    {
        var projectId = Guid.NewGuid();
        var existingProject = CreateProject(id: projectId);

        _unitOfWorkMock.Setup(u => u.Projects.GetByIdWithTranslationsAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProject);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new UpdateProjectRequest
        {
            Slug = "updated-slug",
            ThumbnailUrl = "updated.png",
            PreviewUrl = "updated-demo.com",
            SourceCodeUrl = "updated-src.com",
            IsClientProject = true,
            DisplayOrder = 99,
            IsActive = false,
            Translations =
            [
                new ProjectTranslationRequest
                {
                    LanguageCode = "en", Title = "Updated", Description = "New Desc", Technologies = "Rust"
                }
            ]
        };

        await _sut.UpdateAsync(projectId, request);

        existingProject.Slug.Should().Be("updated-slug");
        existingProject.ThumbnailUrl.Should().Be("updated.png");
        existingProject.PreviewUrl.Should().Be("updated-demo.com");
        existingProject.SourceCodeUrl.Should().Be("updated-src.com");
        existingProject.IsClientProject.Should().BeTrue();
        existingProject.DisplayOrder.Should().Be(99);
        existingProject.IsActive.Should().BeFalse();
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesEntityAndSavesChanges()
    {
        var projectId = Guid.NewGuid();
        var existingProject = CreateProject(id: projectId);

        _unitOfWorkMock.Setup(u => u.Projects.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProject);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.DeleteAsync(projectId);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.Projects.Delete(existingProject), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ReturnsFailure()
    {
        var nonExistentId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Projects.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var result = await _sut.DeleteAsync(nonExistentId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
        _unitOfWorkMock.Verify(u => u.Projects.Delete(It.IsAny<Project>()), Times.Never);
    }

    #endregion
}