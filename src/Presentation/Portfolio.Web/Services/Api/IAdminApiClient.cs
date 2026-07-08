namespace Portfolio.Web.Services.Api;

/// <summary>Result of a successful admin login against Portfolio.API.</summary>
public record AdminLoginResult(string Token, DateTime ExpiresAt);

/// <summary>
/// Server-side client for the authenticated <c>api/admin/*</c> endpoints. Phase 3
/// wires the admin panel through the Web app (MVC proxy): the JWT never reaches the
/// browser — it is obtained here and stored in the admin's auth cookie.
/// </summary>
public interface IAdminApiClient
{
    /// <summary>Authenticates against the API; returns the token/expiry, or null on invalid credentials.</summary>
    Task<AdminLoginResult?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}
