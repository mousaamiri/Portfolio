using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.Infrastructure.Email;

/// <summary>
/// MailKit-based SMTP sender for contact-form notifications. Uses STARTTLS
/// (port 587) — the setup Gmail expects with an App Password. No-ops when
/// disabled or unconfigured so a dev machine without credentials still runs.
/// </summary>
public sealed class SmtpEmailSender(
    IOptions<EmailSettings> options,
    ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendContactNotificationAsync(ContactNotification notification, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
        {
            logger.LogInformation("Email notifications disabled; skipping send for {From}.", notification.Email);
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.Host)
            || string.IsNullOrWhiteSpace(_settings.FromAddress)
            || string.IsNullOrWhiteSpace(_settings.ToAddress))
        {
            logger.LogWarning("Email notifications enabled but SMTP is not fully configured; skipping send.");
            return;
        }

        var mail = new MimeMessage();
        mail.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        mail.To.Add(MailboxAddress.Parse(_settings.ToAddress));
        // Let the owner hit "Reply" and answer the visitor directly.
        mail.ReplyTo.Add(new MailboxAddress(notification.Name, notification.Email));
        mail.Subject = BuildSubject(notification);
        mail.Body = new BodyBuilder { HtmlBody = BuildHtmlBody(notification) }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
        await client.SendAsync(mail, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Contact notification email sent for submission from {From}.", notification.Email);
    }

    private static string BuildSubject(ContactNotification n)
    {
        var suffix = string.IsNullOrWhiteSpace(n.Subject) ? "" : $": {n.Subject}";
        return $"New contact message from {n.Name}{suffix}";
    }

    private static string BuildHtmlBody(ContactNotification n)
    {
        string Row(string label, string? value) =>
            string.IsNullOrWhiteSpace(value)
                ? ""
                : $"<tr><td style=\"padding:6px 12px;font-weight:600;color:#555;vertical-align:top\">{WebUtility.HtmlEncode(label)}</td>" +
                  $"<td style=\"padding:6px 12px;color:#111\">{WebUtility.HtmlEncode(value)}</td></tr>";

        var bodyHtml = WebUtility.HtmlEncode(n.Body).Replace("\n", "<br/>");

        return $$"""
            <div style="font-family:system-ui,Segoe UI,Arial,sans-serif;max-width:640px;margin:0 auto">
              <h2 style="color:#111;border-bottom:2px solid #eee;padding-bottom:8px">New contact-form message</h2>
              <table style="border-collapse:collapse;width:100%">
                {{Row("Name", n.Name)}}
                {{Row("Email", n.Email)}}
                {{Row("Phone", n.Phone)}}
                {{Row("Subject", n.Subject)}}
                {{Row("Interest", n.Interest)}}
                {{Row("Received (UTC)", n.SubmittedAtUtc.ToString("yyyy-MM-dd HH:mm:ss"))}}
              </table>
              <div style="margin-top:16px;padding:12px 16px;background:#f7f7f8;border-radius:8px;color:#111;line-height:1.6">
                {{bodyHtml}}
              </div>
            </div>
            """;
    }
}
