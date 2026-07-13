using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs.Messages;

/// <summary>Public contact-form submission payload.</summary>
public class CreateMessageRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(40)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Subject { get; set; }

    [Required]
    [MaxLength(5000)]
    public string Body { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Interest { get; set; }
}
