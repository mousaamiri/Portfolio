using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Faqs;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Faqs;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class FaqServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly FaqService _sut;

    public FaqServiceTests()
    {
        _sut = new FaqService(_unitOfWorkMock.Object);
    }

    private static Faq CreateFaq(string question = "Q?")
    {
        var faq = new Faq { Id = Guid.NewGuid(), DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
        faq.Translations.Add(new FaqTranslation
        {
            Id = Guid.NewGuid(), FaqId = faq.Id, Language = Language.En, Question = question, Answer = "A"
        });
        return faq;
    }

    [Fact]
    public async Task GetPublicAsync_ReturnsMappedDtos()
    {
        _unitOfWorkMock.Setup(u => u.Faqs.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Faq> { CreateFaq("Remote?") });

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value![0].Question.Should().Be("Remote?");
        _unitOfWorkMock.Verify(u => u.Faqs.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PersistsAndReturnsId()
    {
        _unitOfWorkMock.Setup(u => u.Faqs.AddAsync(It.IsAny<Faq>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new CreateFaqRequest
        {
            DisplayOrder = 1,
            Translations = [new() { LanguageCode = "en", Question = "Q?", Answer = "A" }]
        };

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.Faqs.AddAsync(It.IsAny<Faq>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithMissingId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Faqs.GetByIdWithTranslationsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Faq?)null);

        var result = await _sut.UpdateAsync(id, new UpdateFaqRequest { Translations = [] });

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
