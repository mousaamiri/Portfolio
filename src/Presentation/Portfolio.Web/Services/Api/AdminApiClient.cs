using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Portfolio.Web.Services.Api;

public class AdminApiClient(HttpClient httpClient, ILogger<AdminApiClient> logger) : IAdminApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

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

            var body = await response.Content.ReadFromJsonAsync<ApiResponse<AdminLoginResult>>(JsonOptions, cancellationToken);
            return body?.Data;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin login request to Portfolio.API failed.");
            return null;
        }
    }

    public async Task<string?> GetCurrentAdminAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync("api/admin/auth/me", cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var body = await response.Content.ReadFromJsonAsync<ApiResponse<MeResponse>>(JsonOptions, cancellationToken);
            return body?.Data?.Username;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin 'me' request to Portfolio.API failed.");
            return null;
        }
    }

    public async Task<IReadOnlyList<ProjectApiDto>> GetProjectsAsync(string lang, CancellationToken cancellationToken = default)
    {
        try
        {
            var body = await httpClient.GetFromJsonAsync<ApiResponse<List<ProjectApiDto>>>(
                $"api/admin/projects?lang={lang}", JsonOptions, cancellationToken);
            return body?.Data ?? [];
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin GetProjects request failed.");
            return [];
        }
    }

    public async Task<ProjectApiDto?> GetProjectAsync(Guid id, string lang, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"api/admin/projects/{id}?lang={lang}", cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var body = await response.Content.ReadFromJsonAsync<ApiResponse<ProjectApiDto>>(JsonOptions, cancellationToken);
            return body?.Data;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin GetProject request failed.");
            return null;
        }
    }

    public async Task<(ProjectApiDto? En, ProjectApiDto? Fa)> GetProjectBothLanguagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var en = await GetProjectAsync(id, "en", cancellationToken);
        var fa = await GetProjectAsync(id, "fa", cancellationToken);
        return (en, fa);
    }

    public async Task<Guid?> CreateProjectAsync(CreateProjectApiRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/admin/projects", request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(JsonOptions, cancellationToken);
            return body?.Data;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin CreateProject request failed.");
            return null;
        }
    }

    public async Task<bool> UpdateProjectAsync(Guid id, UpdateProjectApiRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"api/admin/projects/{id}", request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin UpdateProject request failed.");
            return false;
        }
    }

    public async Task<bool> DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"api/admin/projects/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin DeleteProject request failed.");
            return false;
        }
    }

    // ── Messages ──

    public async Task<IReadOnlyList<MessageApiDto>> GetMessagesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var body = await httpClient.GetFromJsonAsync<ApiResponse<List<MessageApiDto>>>("api/admin/messages", JsonOptions, cancellationToken);
            return body?.Data ?? [];
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin GetMessages request failed.");
            return [];
        }
    }

    public async Task<MessageApiDto?> GetMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"api/admin/messages/{id}", cancellationToken);
            if (!response.IsSuccessStatusCode) return null;
            var body = await response.Content.ReadFromJsonAsync<ApiResponse<MessageApiDto>>(JsonOptions, cancellationToken);
            return body?.Data;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin GetMessage request failed.");
            return null;
        }
    }

    public async Task<bool> MarkMessageReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsync($"api/admin/messages/{id}/read", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin MarkMessageRead request failed.");
            return false;
        }
    }

    public async Task<bool> DeleteMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"api/admin/messages/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin DeleteMessage request failed.");
            return false;
        }
    }

    // ── Profile (upsert via PUT api/admin/profile) ──

    // ── Profile (upsert via PUT api/admin/profile) ──

    public async Task<bool> UpsertProfileAsync(UpsertProfileApiRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync("api/admin/profile", request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin UpsertProfile request failed.");
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                "api/admin/auth/change-password",
                new { currentPassword, newPassword },
                cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin ChangePassword request failed.");
            return false;
        }
    }

    private static bool IsTransport(Exception ex)
        => ex is HttpRequestException or TaskCanceledException or JsonException;

    private sealed class MeResponse
    {
        public string? Username { get; set; }
    }
}
