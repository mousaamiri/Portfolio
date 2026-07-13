using System.ComponentModel.DataAnnotations;

namespace Portfolio.Web.Models.ViewModels;

/// <summary>Bound payload from the public contact form (posted as JSON by contact.js).</summary>
public class ContactFormModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Subject { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public string? Interest { get; set; }

    /// <summary>
    /// Honeypot decoy. Hidden from real users via CSS; only bots fill it. A
    /// non-empty value means spam — the submission is silently dropped.
    /// </summary>
    public string? Website { get; set; }
}
