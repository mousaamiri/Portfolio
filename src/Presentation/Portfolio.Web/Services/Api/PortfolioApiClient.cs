using System.Net;
using System.Net.Http.Json;

namespace Portfolio.Web.Services.Api;

/// <summary>
/// <see cref="HttpClient"/>-backed <see cref="IPortfolioApiClient"/>. The base
/// address is configured via <c>AddHttpClient</c> in <c>Program.cs</c>.
/// <para>
/// Error policy: a genuinely unreachable server or server-side failure (connection
/// refused, DNS/TLS failure, timeout, 5xx, unparseable body) throws
/// <see cref="ApiUnavailableException"/> so the global error page renders instead
/// of a half-empty page. A 4xx (server up, resource absent) yields an empty list /
/// null so the page renders normally. Genuine client cancellation (the request's
/// own token) is rethrown untouched.
/// </para>
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

    public Task<IReadOnlyList<ArticleApiDto>> GetArticlesAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<ArticleApiDto>("articles", lang, cancellationToken);

    public Task<IReadOnlyList<FaqApiDto>> GetFaqsAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<FaqApiDto>("faqs", lang, cancellationToken);

    public Task<IReadOnlyList<TimelineEntryApiDto>> GetTimelineAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<TimelineEntryApiDto>("timeline", lang, cancellationToken);

    public Task<IReadOnlyList<InterestApiDto>> GetInterestsAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<InterestApiDto>("interests", lang, cancellationToken);

    public Task<IReadOnlyList<StatCounterApiDto>> GetStatsAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<StatCounterApiDto>("stats", lang, cancellationToken);

    public Task<IReadOnlyList<ImpactMetricApiDto>> GetImpactMetricsAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<ImpactMetricApiDto>("impact-metrics", lang, cancellationToken);

    public Task<IReadOnlyList<PrincipleApiDto>> GetPrinciplesAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<PrincipleApiDto>("principles", lang, cancellationToken);

    public Task<IReadOnlyList<ProficiencyGroupApiDto>> GetProficienciesAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<ProficiencyGroupApiDto>("proficiencies", lang, cancellationToken);

    public Task<IReadOnlyList<TestimonialApiDto>> GetTestimonialsAsync(string lang, CancellationToken cancellationToken = default)
        => GetListAsync<TestimonialApiDto>("testimonials", lang, cancellationToken);

    public async Task<ProfileApiDto?> GetProfileAsync(string lang, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<ApiResponse<ProfileApiDto>>(
                $"api/public/profile?lang={lang}", cancellationToken);

            return response?.Data;
        }
        catch (HttpRequestException ex) when (IsResourceAbsent(ex))
        {
            logger.LogWarning(ex, "Profile not found on Portfolio.API; returning null.");
            return null;
        }
        catch (Exception ex) when (IsServerUnavailable(ex, cancellationToken))
        {
            throw Unavailable("profile", ex);
        }
    }

    public async Task<bool> SendMessageAsync(ContactMessageRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/public/messages", request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // The contact form handles a false result gracefully (inline error), so a
            // failed submit does not escalate to the whole-page error screen.
            logger.LogWarning(ex, "Failed to submit contact message to Portfolio.API.");
            return false;
        }
    }

    public async Task<IReadOnlyDictionary<string, string>> GetUiTranslationsAsync(string lang, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<ApiResponse<Dictionary<string, string>>>(
                $"api/public/ui-translations?lang={lang}", cancellationToken);

            return response?.Data ?? new Dictionary<string, string>();
        }
        catch (HttpRequestException ex) when (IsResourceAbsent(ex))
        {
            logger.LogWarning(ex, "UI translations not found on Portfolio.API; returning empty map.");
            return new Dictionary<string, string>();
        }
        catch (Exception ex) when (IsServerUnavailable(ex, cancellationToken))
        {
            throw Unavailable("ui-translations", ex);
        }
    }

    private async Task<IReadOnlyList<T>> GetListAsync<T>(string resource, string lang, CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<ApiResponse<List<T>>>(
                $"api/public/{resource}?lang={lang}", cancellationToken);

            return response?.Data ?? [];
        }
        catch (HttpRequestException ex) when (IsResourceAbsent(ex))
        {
            logger.LogWarning(ex, "{Resource} not found on Portfolio.API; returning empty list.", resource);
            return [];
        }
        catch (Exception ex) when (IsServerUnavailable(ex, cancellationToken))
        {
            throw Unavailable(resource, ex);
        }
    }

    // A 4xx means the server answered — the resource is just absent. That is not an
    // outage, so the page renders (empty) rather than showing the error screen.
    private static bool IsResourceAbsent(HttpRequestException ex)
        => ex.StatusCode is >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError;

    // Everything else caused by the network/server (connection refused, DNS/TLS,
    // 5xx, request timeout, unparseable body) is an outage. A cancellation raised by
    // the caller's own token is NOT — that is rethrown untouched.
    private static bool IsServerUnavailable(Exception ex, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return false; // genuine client cancellation → let it propagate as-is

        return ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException;
    }

    private ApiUnavailableException Unavailable(string resource, Exception ex)
    {
        logger.LogError(ex, "Portfolio.API unavailable while fetching {Resource}.", resource);
        return new ApiUnavailableException(
            $"Could not reach Portfolio.API while fetching '{resource}'.", ex);
    }
}
