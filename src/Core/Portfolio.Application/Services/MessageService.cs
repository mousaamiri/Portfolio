using Microsoft.Extensions.Logging;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Messages;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Messages;

namespace Portfolio.Application.Services;

public class MessageService(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    ILogger<MessageService> logger) : IMessageService
{
    public async Task<Result<Guid>> CreateAsync(CreateMessageRequest request, CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Subject = request.Subject,
            Body = request.Body,
            Interest = request.Interest,
            IsRead = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Messages.AddAsync(message, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Best-effort email notification. The message is already persisted, so a
        // mail failure must never fail the submission — log and swallow.
        try
        {
            await emailSender.SendContactNotificationAsync(
                new ContactNotification(
                    message.Name,
                    message.Email,
                    message.Phone,
                    message.Subject,
                    message.Interest,
                    message.Body,
                    message.CreatedAt),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send contact-form notification email for message {MessageId}.", message.Id);
        }

        return Result<Guid>.Success(message.Id);
    }

    public async Task<Result<IReadOnlyList<MessageDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var messages = await unitOfWork.Messages.GetAllOrderedByNewestAsync(cancellationToken);

        var dtos = messages.Select(MapToDto).ToList();
        return Result<IReadOnlyList<MessageDto>>.Success(dtos);
    }

    public async Task<Result<MessageDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = await unitOfWork.Messages.GetByIdAsync(id, cancellationToken);
        if (message is null)
            return Result<MessageDto>.Failure($"Message with id '{id}' was not found.");

        return Result<MessageDto>.Success(MapToDto(message));
    }

    public async Task<Result<bool>> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = await unitOfWork.Messages.GetByIdAsync(id, cancellationToken);
        if (message is null)
            return Result<bool>.Failure($"Message with id '{id}' was not found.");

        message.IsRead = true;
        message.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = await unitOfWork.Messages.GetByIdAsync(id, cancellationToken);
        if (message is null)
            return Result<bool>.Failure($"Message with id '{id}' was not found.");

        unitOfWork.Messages.Delete(message);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static MessageDto MapToDto(Message message) => new()
    {
        Id = message.Id,
        Name = message.Name,
        Email = message.Email,
        Phone = message.Phone,
        Subject = message.Subject,
        Body = message.Body,
        Interest = message.Interest,
        IsRead = message.IsRead,
        CreatedAt = message.CreatedAt
    };
}
