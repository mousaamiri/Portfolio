using System.Net.Http.Headers;
using System.Security.Claims;

namespace Portfolio.Web.Services.Api;

/// <summary>
/// Attaches the admin's API JWT (stored as the <c>access_token</c> claim inside the
/// auth cookie during login) as a <c>Bearer</c> header on every outgoing admin API
/// request. Keeps the token server-side only — it is read from the current request's
/// authenticated principal, never from the browser.
/// </summary>
public class BearerTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.User.FindFirstValue("access_token");
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
