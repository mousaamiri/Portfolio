using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Interests;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Interests;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class InterestServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly InterestService _sut;

    public InterestServiceTests()
    {
        _sut = new InterestService(_unitOfWorkMock.Object);
    }

    private static Interest CreateInterest(string label = "Coding")
    {
        var interest = new Interest { Id = Guid.NewGuid(), Icon = "code", DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
        interest.Translations.Add(new InterestTranslation
        {
            Id = Guid.NewGuid(), InterestId = interest.Id, Language = Language.En, Label = label
        });
        return interest;
    }

    [Fact]
    public async Task GetPublicAsync_ReturnsMappedDtos()
    {
        _unitOfWorkMock.Setup(u => u.Interests.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Interest> { CreateInterest("Hiking") });

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value![0].Label.Should().Be("Hiking");
        result.Value[0].Icon.Should().Be("code");
    }

    [Fact]
    public async Task CreateAsync_PersistsAndReturnsId()
    {
        _unitOfWorkMock.Setup(u => u.Interests.AddAsync(It.IsAny<Interest>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(new CreateInterestRequest
        {
            Icon = "code",
            Translations = [new() { LanguageCode = "en", Label = "Coding" }]
        });

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.Interests.AddAsync(It.IsAny<Interest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
