namespace Portfolio.Application.Interfaces.Services;

/// <summary>
/// Sends the owner an email notification when a visitor submits the contact form.
/// Implementations must be best-effort: a send failure should be logged and
/// swallowed by the caller, never breaking the submission that already persisted.
/// </summary>
public interface IEmailSender
{
    Task SendContactNotificationAsync(ContactNotification notification, CancellationToken cancellationToken = default);
}

/// <summary>Data for the contact-form notification email.</summary>
public sealed record ContactNotification(
    string Name,
    string Email,
    string? Phone,
    string? Subject,
    string? Interest,
    string Body,
    DateTime SubmittedAtUtc);
