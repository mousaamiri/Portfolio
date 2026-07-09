namespace Portfolio.Web.Services.Api;

/// <summary>Result of a successful admin login against Portfolio.API.</summary>
public record AdminLoginResult(string Token, DateTime ExpiresAt);

/// <summary>
/// Server-side client for the authenticated <c>api/admin/*</c> endpoints. Phase 3
/// wires the admin panel through the Web app (MVC proxy): the JWT never reaches the
/// browser — it is obtained here and stored in the admin's auth cookie, then
/// replayed as a Bearer header by <see cref="BearerTokenHandler"/>.
/// </summary>
public interface IAdminApiClient
{
    /// <summary>Authenticates against the API; returns the token/expiry, or null on invalid credentials.</summary>
    Task<AdminLoginResult?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>Returns the authenticated admin's username (via api/admin/auth/me), or null if unauthenticated.</summary>
    Task<string?> GetCurrentAdminAsync(CancellationToken cancellationToken = default);

    /// <summary>Changes the current admin's password; true on success, false if the current password is wrong.</summary>
    Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    // ── Projects (admin CRUD) ──
    Task<IReadOnlyList<ProjectApiDto>> GetProjectsAsync(string lang, CancellationToken cancellationToken = default);
    Task<ProjectApiDto?> GetProjectAsync(Guid id, string lang, CancellationToken cancellationToken = default);
    /// <summary>Fetches a project in both EN and FA (for the bilingual edit form).</summary>
    Task<(ProjectApiDto? En, ProjectApiDto? Fa)> GetProjectBothLanguagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid?> CreateProjectAsync(CreateProjectApiRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateProjectAsync(Guid id, UpdateProjectApiRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default);

    // ── Messages (inbox: list / view / mark-read / delete — no create) ──
    Task<IReadOnlyList<MessageApiDto>> GetMessagesAsync(CancellationToken cancellationToken = default);
    Task<MessageApiDto?> GetMessageAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> MarkMessageReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteMessageAsync(Guid id, CancellationToken cancellationToken = default);

    // ── Profile (single-row upsert) ──
    Task<bool> UpsertProfileAsync(UpsertProfileApiRequest request, CancellationToken cancellationToken = default);
}
