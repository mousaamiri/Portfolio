namespace Portfolio.Web.Models.ViewModels;

/// <summary>
/// Backs the standalone global error page. Rendered without an API call so it works
/// during a full outage. <see cref="IsServerUnavailable"/> switches the copy between
/// "server/database unreachable" and a generic unexpected-error message.
/// </summary>
public class ErrorViewModel
{
    public string Language { get; init; } = "en";
    public bool IsServerUnavailable { get; init; }
    public string? RequestId { get; init; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
