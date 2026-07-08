using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.Messages;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Messages;

namespace Portfolio.Application.Tests.Services;

public class MessageServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly MessageService _sut;

    public MessageServiceTests()
    {
        _sut = new MessageService(_unitOfWorkMock.Object);
    }

    private static Message CreateMessage(Guid? id = null, bool isRead = false) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Jane",
        Email = "jane@example.com",
        Subject = "Hi",
        Body = "Hello there",
        Interest = "Freelance",
        IsRead = isRead,
        IsActive = true,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    };

    [Fact]
    public async Task CreateAsync_PersistsMessageAndReturnsId()
    {
        Message? captured = null;
        _unitOfWorkMock.Setup(u => u.Messages.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .Callback<Message, CancellationToken>((m, _) => captured = m)
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new CreateMessageRequest
        {
            Name = "Jane", Email = "jane@example.com", Subject = "Hi",
            Body = "Hello", Interest = "Freelance"
        };

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        captured.Should().NotBeNull();
        captured!.Name.Should().Be("Jane");
        captured.Body.Should().Be("Hello");
        captured.IsRead.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtosNewestFirst()
    {
        var messages = new List<Message> { CreateMessage(), CreateMessage() };
        _unitOfWorkMock.Setup(u => u.Messages.GetAllOrderedByNewestAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        var result = await _sut.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Name.Should().Be("Jane");
        _unitOfWorkMock.Verify(u => u.Messages.GetAllOrderedByNewestAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_WithValidId_SetsIsReadAndSaves()
    {
        var id = Guid.NewGuid();
        var message = CreateMessage(id);
        _unitOfWorkMock.Setup(u => u.Messages.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.MarkAsReadAsync(id);

        result.IsSuccess.Should().BeTrue();
        message.IsRead.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_WithMissingId_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Messages.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Message?)null);

        var result = await _sut.MarkAsReadAsync(id);

        result.IsSuccess.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesAndSaves()
    {
        var id = Guid.NewGuid();
        var message = CreateMessage(id);
        _unitOfWorkMock.Setup(u => u.Messages.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.DeleteAsync(id);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.Messages.Delete(message), Times.Once);
    }
}
