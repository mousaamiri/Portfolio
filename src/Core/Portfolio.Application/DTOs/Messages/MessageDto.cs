namespace Portfolio.Application.DTOs.Messages;

public class MessageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? Interest { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
