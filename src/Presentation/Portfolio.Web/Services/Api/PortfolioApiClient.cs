using System.Net.Http.Json;

namespace Portfolio.Web.Services.Api;

/// <summary>
/// <see cref="HttpClient"/>-backed <see cref="IPortfolioApiClient"/>. The base
/// address is configured via <c>AddHttpClient</c> in <c>Program.cs</c>. Failures
/// (network, non-success status, malformed body) are swallowed and surfaced as an
/// empty list so a page still renders when the API is down.
/// </summary>
public class PortfolioApiClient(HttpClient httpClient, ILogger<PortfolioApiClient> logger) : IPortfolioApiClient
{
    public Task<IReadOnlyList<ProjectApiDto>> GetProjectsAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<ProjectApiDto>("projects", lang, cancellationToken);

    public Task<IReadOnlyList<SkillApiDto>> GetSkillsAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<SkillApiDto>("skills", lang, cancellationToken);

    public Task<IReadOnlyList<ExperienceApiDto>> GetExperiencesAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<ExperienceApiDto>("experiences", lang, cancellationToken);

    public Task<IReadOnlyList<EducationApiDto>> GetEducationsAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<EducationApiDto>("educations", lang, cancellationToken);

    private async Task<IReadOnlyList<T>> GetListAsync<T>(string resource, string lang, CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<ApiResponse<List<T>>>(
                $"api/public/{resource}?lang={lang}", cancellationToken);

            return response?.Data ?? [];
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            logger.LogWarning(ex, "Failed to fetch {Resource} from Portfolio.API; returning empty list.", resource);
            return [];
        }
    }
}
