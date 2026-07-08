using System.Net.Http.Json;

namespace Portfolio.Web.Services.Api;

public class AdminApiClient(HttpClient httpClient, ILogger<AdminApiClient> logger) : IAdminApiClient
{
    public async Task<AdminLoginResult?> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                "api/admin/auth/login",
                new { username, password },
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var body = await response.Content.ReadFromJsonAsync<ApiResponse<AdminLoginResult>>(
                cancellationToken: cancellationToken);

            return body?.Data;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            logger.LogWarning(ex, "Admin login request to Portfolio.API failed.");
            return null;
        }
    }
}
