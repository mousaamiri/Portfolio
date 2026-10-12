using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Services;

public class PortfolioApiClientTests
{
    // Stub transport: returns a canned response and records the last request.
    private sealed class StubHandler(HttpStatusCode status, string json) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(status)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        }
    }

    private static PortfolioApiClient BuildSut(StubHandler handler)
        => new(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") },
               NullLogger<PortfolioApiClient>.Instance);

    [Fact]
    public async Task GetProjectsAsync_Ok_ReturnsData()
    {
        var handler = new StubHandler(HttpStatusCode.OK,
            """{"success":true,"data":[{"id":"11111111-1111-1111-1111-111111111111","slug":"vitastic","title":"Vitastic"}]}""");
        var sut = BuildSut(handler);

        var result = await sut.GetProjectsAsync("en");

        result.Should().ContainSingle();
        result[0].Slug.Should().Be("vitastic");
        handler.LastRequest!.RequestUri!.AbsolutePath.Should().Be("/api/public/projects");
        handler.LastRequest!.RequestUri!.Query.Should().Be("?lang=en");
    }

    [Fact]
    public async Task GetProjectsAsync_OkWithEmptyData_ReturnsEmptyList()
    {
        var handler = new StubHandler(HttpStatusCode.OK, """{"success":true,"data":[]}""");
        var sut = BuildSut(handler);

        var result = await sut.GetProjectsAsync("en");

        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]      // proxy/route gone during a recycle or deploy
    [InlineData(HttpStatusCode.Forbidden)]     // proxy denies while the app pool is mid-recycle
    [InlineData(HttpStatusCode.TooManyRequests)]
    public async Task GetProjectsAsync_ClientErrorOnCollection_ThrowsApiUnavailable(HttpStatusCode status)
    {
        // Regression guard: a 4xx on a collection endpoint is an infrastructure fault, so
        // it must surface as an outage (error page) — never as a silently empty list.
        var handler = new StubHandler(status, """{"success":false}""");
        var sut = BuildSut(handler);

        var act = async () => await sut.GetProjectsAsync("en");

        await act.Should().ThrowAsync<ApiUnavailableException>();
    }

    [Fact]
    public async Task GetProjectsAsync_ServerError_ThrowsApiUnavailable()
    {
        var handler = new StubHandler(HttpStatusCode.InternalServerError, "Internal Server Error");
        var sut = BuildSut(handler);

        var act = async () => await sut.GetProjectsAsync("en");

        await act.Should().ThrowAsync<ApiUnavailableException>();
    }

    [Fact]
    public async Task GetProfileAsync_Ok_ReturnsData()
    {
        var handler = new StubHandler(HttpStatusCode.OK,
            """{"success":true,"data":{"id":"11111111-1111-1111-1111-111111111111","email":"ada@example.com","fullName":"Ada","jobTitle":"Developer"}}""");
        var sut = BuildSut(handler);

        var result = await sut.GetProfileAsync("fa");

        result.Should().NotBeNull();
        result!.FullName.Should().Be("Ada");
        handler.LastRequest!.RequestUri!.AbsolutePath.Should().Be("/api/public/profile");
    }

    [Fact]
    public async Task GetProfileAsync_NotFound_ReturnsNull_BehaviorPreserved()
    {
        // Profile is a single optional resource: a 404 means "not configured yet" and the
        // page still renders. This behavior is intentionally kept for such endpoints.
        var handler = new StubHandler(HttpStatusCode.NotFound,
            """{"success":false,"message":"No profile has been configured yet."}""");
        var sut = BuildSut(handler);

        var result = await sut.GetProfileAsync("en");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUiTranslationsAsync_NotFound_ReturnsEmptyMap_BehaviorPreserved()
    {
        var handler = new StubHandler(HttpStatusCode.NotFound, """{"success":false}""");
        var sut = BuildSut(handler);

        var result = await sut.GetUiTranslationsAsync("en");

        result.Should().BeEmpty();
    }
}
