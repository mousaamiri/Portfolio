using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Testimonials;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Testimonials;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Tests.Services;

public class TestimonialServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly TestimonialService _sut;

    public TestimonialServiceTests()
    {
        _sut = new TestimonialService(_uow.Object);
    }

    private static Testimonial Create(string name = "Jane")
    {
        var t = new Testimonial { Id = Guid.NewGuid(), Initials = "JD", DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
        t.Translations.Add(new TestimonialTranslation { Id = Guid.NewGuid(), TestimonialId = t.Id, Language = Language.En, Quote = "Great", Name = name, Role = "PM" });
        return t;
    }

    [Fact]
    public async Task GetPublicAsync_MapsQuoteAndName()
    {
        _uow.Setup(u => u.Testimonials.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Testimonial> { Create("Jane") });

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value![0].Name.Should().Be("Jane");
        result.Value[0].Quote.Should().Be("Great");
        _uow.Verify(u => u.Testimonials.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPublicAsync_WhenNone_ReturnsEmpty()
    {
        _uow.Setup(u => u.Testimonials.GetActiveWithTranslationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Testimonial>());

        var result = await _sut.GetPublicAsync("en");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_Persists()
    {
        _uow.Setup(u => u.Testimonials.AddAsync(It.IsAny<Testimonial>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(new CreateTestimonialRequest
        {
            Initials = "JD",
            Translations = [new() { LanguageCode = "en", Quote = "Great", Name = "Jane", Role = "PM" }]
        });

        result.IsSuccess.Should().BeTrue();
        _uow.Verify(u => u.Testimonials.AddAsync(It.IsAny<Testimonial>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_MissingId_Fails()
    {
        var id = Guid.NewGuid();
        _uow.Setup(u => u.Testimonials.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Testimonial?)null);

        var result = await _sut.DeleteAsync(id);

        result.IsSuccess.Should().BeFalse();
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
