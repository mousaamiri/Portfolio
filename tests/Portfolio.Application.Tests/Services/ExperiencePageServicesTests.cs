using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.ImpactMetrics;
using Portfolio.Application.DTOs.Principles;
using Portfolio.Application.DTOs.Proficiencies;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.ImpactMetrics;
using Portfolio.Domain.Entities.Principles;
using Portfolio.Domain.Entities.Proficiencies;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class ExperiencePageServicesTests
{
    private readonly Mock<IUnitOfWork> _uow = new();

    // ── ImpactMetric ──

    [Fact]
    public async Task ImpactMetric_GetPublicAsync_MapsTagAndDescription()
    {
        var metric = new ImpactMetric { Id = Guid.NewGuid(), Value = "99.9%", Color = "pink", IsActive = true, CreatedAt = DateTime.UtcNow };
        metric.Translations.Add(new ImpactMetricTranslation { Id = Guid.NewGuid(), ImpactMetricId = metric.Id, Language = Language.En, Tag = "UPTIME", Description = "d" });
        _uow.Setup(u => u.ImpactMetrics.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ImpactMetric> { metric });

        var result = await new ImpactMetricService(_uow.Object).GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Value.Should().Be("99.9%");
        result.Value[0].Tag.Should().Be("UPTIME");
    }

    [Fact]
    public async Task ImpactMetric_CreateAsync_Persists()
    {
        _uow.Setup(u => u.ImpactMetrics.AddAsync(It.IsAny<ImpactMetric>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new ImpactMetricService(_uow.Object).CreateAsync(new CreateImpactMetricRequest
        {
            Value = "10x", Color = "amber",
            Translations = [new() { LanguageCode = "en", Tag = "SCALE", Description = "d" }]
        });

        result.IsSuccess.Should().BeTrue();
        _uow.Verify(u => u.ImpactMetrics.AddAsync(It.IsAny<ImpactMetric>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Principle ──

    [Fact]
    public async Task Principle_GetPublicAsync_MapsTitleAndDescription()
    {
        var principle = new Principle { Id = Guid.NewGuid(), IsActive = true, CreatedAt = DateTime.UtcNow };
        principle.Translations.Add(new PrincipleTranslation { Id = Guid.NewGuid(), PrincipleId = principle.Id, Language = Language.En, Title = "Scale-First", Description = "d" });
        _uow.Setup(u => u.Principles.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Principle> { principle });

        var result = await new PrincipleService(_uow.Object).GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Title.Should().Be("Scale-First");
    }

    [Fact]
    public async Task Principle_UpdateAsync_MissingId_Fails()
    {
        var id = Guid.NewGuid();
        _uow.Setup(u => u.Principles.GetByIdWithTranslationsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Principle?)null);

        var result = await new PrincipleService(_uow.Object).UpdateAsync(id, new UpdatePrincipleRequest { Translations = [] });

        result.IsSuccess.Should().BeFalse();
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── ProficiencyGroup ──

    [Fact]
    public async Task Proficiency_GetPublicAsync_MapsTitleAndItems()
    {
        var group = new ProficiencyGroup { Id = Guid.NewGuid(), Color = "amber", IsActive = true, CreatedAt = DateTime.UtcNow };
        group.Translations.Add(new ProficiencyGroupTranslation { Id = Guid.NewGuid(), ProficiencyGroupId = group.Id, Language = Language.En, Title = "MASTERY", Items = "Java 21, Spring Boot" });
        _uow.Setup(u => u.ProficiencyGroups.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProficiencyGroup> { group });

        var result = await new ProficiencyGroupService(_uow.Object).GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Title.Should().Be("MASTERY");
        result.Value[0].Items.Should().Be("Java 21, Spring Boot");
    }

    [Fact]
    public async Task Proficiency_CreateAsync_Persists()
    {
        _uow.Setup(u => u.ProficiencyGroups.AddAsync(It.IsAny<ProficiencyGroup>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await new ProficiencyGroupService(_uow.Object).CreateAsync(new CreateProficiencyGroupRequest
        {
            Color = "pink",
            Translations = [new() { LanguageCode = "en", Title = "TOOLS", Items = "Git, Docker" }]
        });

        result.IsSuccess.Should().BeTrue();
        _uow.Verify(u => u.ProficiencyGroups.AddAsync(It.IsAny<ProficiencyGroup>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
