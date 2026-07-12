namespace Portfolio.Web.Services.Api;

/// <summary>
/// Thrown by <see cref="PortfolioApiClient"/> when Portfolio.API is unreachable or
/// returns a server-side failure (connection refused, DNS failure, request timeout,
/// 5xx status, or an unparseable body). Signals that the site cannot load its data
/// from the server/database — the global error page is shown instead of a
/// half-empty page. Client-cancelled requests and 4xx "not found" responses do
/// NOT raise this (those render normally / as empty content).
/// </summary>
public class ApiUnavailableException : Exception
{
    public ApiUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
