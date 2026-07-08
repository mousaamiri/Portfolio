using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Timeline;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Timeline;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class TimelineEntryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly TimelineEntryService _sut;

    public TimelineEntryServiceTests()
    {
        _sut = new TimelineEntryService(_unitOfWorkMock.Object);
    }

    private static TimelineEntry CreateEntry(string title = "Milestone")
    {
        var entry = new TimelineEntry { Id = Guid.NewGuid(), Year = "2024", Icon = "rocket", DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
        entry.Translations.Add(new TimelineEntryTranslation
        {
            Id = Guid.NewGuid(), TimelineEntryId = entry.Id, Language = Language.En, Title = title, Description = "d"
        });
        return entry;
    }

    [Fact]
    public async Task GetPublicAsync_ReturnsMappedDtos()
    {
        _unitOfWorkMock.Setup(u => u.TimelineEntries.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimelineEntry> { CreateEntry("Built portfolio") });

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value![0].Title.Should().Be("Built portfolio");
        result.Value[0].Year.Should().Be("2024");
    }

    [Fact]
    public async Task CreateAsync_PersistsAndReturnsId()
    {
        _unitOfWorkMock.Setup(u => u.TimelineEntries.AddAsync(It.IsAny<TimelineEntry>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(new CreateTimelineEntryRequest
        {
            Year = "2024", Icon = "rocket",
            Translations = [new() { LanguageCode = "en", Title = "T", Description = "d" }]
        });

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.TimelineEntries.AddAsync(It.IsAny<TimelineEntry>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
