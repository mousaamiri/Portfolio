namespace Portfolio.Web.Services.Api;

/// <summary>
/// Mirrors the <c>Portfolio.API</c> response envelope so the Web app can
/// deserialize public endpoint payloads without referencing the API project.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public IReadOnlyList<string>? Errors { get; set; }
}
