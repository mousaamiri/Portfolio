namespace Portfolio.Web.Services.Api;

/// <summary>
/// Typed client over the public <c>Portfolio.API</c> read endpoints. Each call
/// passes a resolved language code (<c>?lang</c>) and returns already-filtered,
/// ordered data. Implementations degrade to an empty list on transport failure
/// so the site still renders if the API is unavailable.
/// </summary>
public interface IPortfolioApiClient
{
    Task<IReadOnlyList<ProjectApiDto>> GetProjectsAsync(string lang, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SkillApiDto>> GetSkillsAsync(string lang, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExperienceApiDto>> GetExperiencesAsync(string lang, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EducationApiDto>> GetEducationsAsync(string lang, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArticleApiDto>> GetArticlesAsync(string lang, CancellationToken cancellationToken = default);

    /// <summary>Returns the site profile (hero/bio) for the given language, or null if none/unavailable.</summary>
    Task<ProfileApiDto?> GetProfileAsync(string lang, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FaqApiDto>> GetFaqsAsync(string lang, CancellationToken cancellationToken = default);

    /// <summary>Submits a contact-form message to the public API. Returns true on success.</summary>
    Task<bool> SendMessageAsync(ContactMessageRequest request, CancellationToken cancellationToken = default);
}
