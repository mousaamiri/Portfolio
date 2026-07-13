using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities.Messages;

/// <summary>
/// A contact-form submission from the public site. Not translatable; content is
/// whatever the visitor typed. Admin-only on the read side.
/// </summary>
public class Message : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? Interest { get; set; }
    public bool IsRead { get; set; }
}
