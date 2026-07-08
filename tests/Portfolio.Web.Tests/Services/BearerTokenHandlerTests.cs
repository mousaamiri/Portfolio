using System.Net;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Services;

public class BearerTokenHandlerTests
{
    // Test double: captures the request it was asked to send and returns 200.
    private sealed class CapturingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    private static HttpClient BuildClient(CapturingHandler inner, ClaimsPrincipal? user)
    {
        var accessor = new HttpContextAccessor
        {
            HttpContext = user is null ? null : new DefaultHttpContext { User = user }
        };
        var handler = new BearerTokenHandler(accessor) { InnerHandler = inner };
        return new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") };
    }

    private static ClaimsPrincipal UserWithToken(string token)
        => new(new ClaimsIdentity(new[] { new Claim("access_token", token) }, "cookie"));

    [Fact]
    public async Task SendAsync_WithToken_AttachesBearerHeader()
    {
        var inner = new CapturingHandler();
        var client = BuildClient(inner, UserWithToken("jwt-123"));

        await client.GetAsync("api/admin/projects");

        inner.LastRequest!.Headers.Authorization!.Scheme.Should().Be("Bearer");
        inner.LastRequest.Headers.Authorization.Parameter.Should().Be("jwt-123");
    }

    [Fact]
    public async Task SendAsync_WithoutToken_DoesNotAttachHeader()
    {
        var inner = new CapturingHandler();
        var client = BuildClient(inner, user: null);

        await client.GetAsync("api/admin/projects");

        inner.LastRequest!.Headers.Authorization.Should().BeNull();
    }
}
