namespace Portfolio.Web.Services.Api;

/// <summary>
/// Payload sent to the public <c>api/public/messages</c> endpoint when a visitor
/// submits the contact form. Mirrors the API's CreateMessageRequest JSON shape.
/// </summary>
public class ContactMessageRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? Interest { get; set; }
}
