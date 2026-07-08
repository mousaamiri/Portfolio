using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Stats;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Stats;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class StatCounterServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly StatCounterService _sut;

    public StatCounterServiceTests()
    {
        _sut = new StatCounterService(_unitOfWorkMock.Object);
    }

    private static StatCounter CreateStat(string label = "Projects", long target = 48)
    {
        var stat = new StatCounter
        {
            Id = Guid.NewGuid(), Icon = "folder", CountTarget = target, Suffix = "+",
            DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow
        };
        stat.Translations.Add(new StatCounterTranslation
        {
            Id = Guid.NewGuid(), StatCounterId = stat.Id, Language = Language.En, Label = label
        });
        return stat;
    }

    [Fact]
    public async Task GetPublicAsync_ReturnsMappedDtos()
    {
        _unitOfWorkMock.Setup(u => u.StatCounters.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StatCounter> { CreateStat("Commits", 1500) });

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Label.Should().Be("Commits");
        result.Value[0].CountTarget.Should().Be(1500);
        _unitOfWorkMock.Verify(u => u.StatCounters.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PersistsAndReturnsId()
    {
        _unitOfWorkMock.Setup(u => u.StatCounters.AddAsync(It.IsAny<StatCounter>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new CreateStatCounterRequest
        {
            Icon = "coffee", CountTarget = 999, Suffix = "+",
            Translations = [new() { LanguageCode = "en", Label = "Coffee" }]
        };

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.StatCounters.AddAsync(It.IsAny<StatCounter>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithMissingId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.StatCounters.GetByIdWithTranslationsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StatCounter?)null);

        var result = await _sut.UpdateAsync(id, new UpdateStatCounterRequest { Translations = [] });

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
