using System.Net.Http.Json;
using System.Text.Json;

namespace Portfolio.Web.Services.Api;

/// <summary>
/// Generic typed access to the authenticated <c>api/admin/{resource}</c> CRUD
/// endpoints (all follow the same shape: GET list, GET {id}, POST, PUT {id},
/// DELETE {id}, wrapped in <see cref="ApiResponse{T}"/>). Each admin entity's
/// controller uses this with its own DTO/request types instead of a bespoke
/// method per entity. Transport failures degrade to empty/false/null.
/// </summary>
public interface IAdminCrudClient
{
    Task<IReadOnlyList<TDto>> ListAsync<TDto>(string resource, string lang, CancellationToken ct = default);
    Task<TDto?> GetAsync<TDto>(string resource, Guid id, string lang, CancellationToken ct = default) where TDto : class;
    Task<Guid?> CreateAsync<TRequest>(string resource, TRequest request, CancellationToken ct = default);
    Task<bool> UpdateAsync<TRequest>(string resource, Guid id, TRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(string resource, Guid id, CancellationToken ct = default);
}

public class AdminCrudClient(HttpClient httpClient, ILogger<AdminCrudClient> logger) : IAdminCrudClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IReadOnlyList<TDto>> ListAsync<TDto>(string resource, string lang, CancellationToken ct = default)
    {
        try
        {
            var body = await httpClient.GetFromJsonAsync<ApiResponse<List<TDto>>>(
                $"api/admin/{resource}?lang={lang}", JsonOptions, ct);
            return body?.Data ?? [];
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin list {Resource} failed.", resource);
            return [];
        }
    }

    public async Task<TDto?> GetAsync<TDto>(string resource, Guid id, string lang, CancellationToken ct = default) where TDto : class
    {
        try
        {
            var response = await httpClient.GetAsync($"api/admin/{resource}/{id}?lang={lang}", ct);
            if (!response.IsSuccessStatusCode) return null;
            var body = await response.Content.ReadFromJsonAsync<ApiResponse<TDto>>(JsonOptions, ct);
            return body?.Data;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin get {Resource}/{Id} failed.", resource, id);
            return null;
        }
    }

    public async Task<Guid?> CreateAsync<TRequest>(string resource, TRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"api/admin/{resource}", request, ct);
            if (!response.IsSuccessStatusCode) return null;
            var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(JsonOptions, ct);
            return body?.Data;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin create {Resource} failed.", resource);
            return null;
        }
    }

    public async Task<bool> UpdateAsync<TRequest>(string resource, Guid id, TRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"api/admin/{resource}/{id}", request, ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin update {Resource}/{Id} failed.", resource, id);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string resource, Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"api/admin/{resource}/{id}", ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (IsTransport(ex))
        {
            logger.LogWarning(ex, "Admin delete {Resource}/{Id} failed.", resource, id);
            return false;
        }
    }

    private static bool IsTransport(Exception ex)
        => ex is HttpRequestException or TaskCanceledException or JsonException;
}
