namespace Portfolio.Infrastructure.Email;

/// <summary>
/// SMTP configuration for contact-form notification emails. Bound from the
/// "Email" configuration section. Secrets (Username/Password) live in
/// user-secrets in development and environment/appsettings.Production on the
/// server — never committed.
/// </summary>
public class EmailSettings
{
    /// <summary>Master switch. When false, the sender is a no-op (dev without creds).</summary>
    public bool Enabled { get; set; }

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    /// <summary>The "from" address. For Gmail this must equal the authenticated Username.</summary>
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "Portfolio Contact Form";

    /// <summary>Where notifications are delivered (the owner's inbox).</summary>
    public string ToAddress { get; set; } = string.Empty;
}
