using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Services;

public class AdminApiClientTests
{
    // Stub transport: returns a canned response and records the last request.
    private sealed class StubHandler(HttpStatusCode status, string json) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastRequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (request.Content is not null)
                LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(status)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }

    private static AdminApiClient BuildSut(StubHandler handler)
        => new(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") },
               NullLogger<AdminApiClient>.Instance);

    [Fact]
    public async Task LoginAsync_Success_ReturnsToken()
    {
        var handler = new StubHandler(HttpStatusCode.OK,
            """{"success":true,"data":{"token":"jwt-xyz","expiresAt":"2026-07-15T00:00:00Z"}}""");
        var sut = BuildSut(handler);

        var result = await sut.LoginAsync("admin", "secret");

        result!.Token.Should().Be("jwt-xyz");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.AbsolutePath.Should().Be("/api/admin/auth/login");
    }

    [Fact]
    public async Task LoginAsync_Unauthorized_ReturnsNull()
    {
        var handler = new StubHandler(HttpStatusCode.Unauthorized,
            """{"success":false,"message":"Invalid username or password."}""");
        var sut = BuildSut(handler);

        var result = await sut.LoginAsync("admin", "wrong");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetProjectsAsync_ReturnsMappedList_FromAdminEndpoint()
    {
        var handler = new StubHandler(HttpStatusCode.OK,
            """{"success":true,"data":[{"id":"11111111-1111-1111-1111-111111111111","slug":"vitastic","title":"Vitastic","isActive":true,"isPublished":false}]}""");
        var sut = BuildSut(handler);

        var result = await sut.GetProjectsAsync("en");

        result.Should().ContainSingle();
        result[0].Slug.Should().Be("vitastic");
        handler.LastRequest!.RequestUri!.AbsolutePath.Should().Be("/api/admin/projects");
    }

    [Fact]
    public async Task CreateProjectAsync_Returns201Id_AndPostsBody()
    {
        var newId = Guid.NewGuid();
        var handler = new StubHandler(HttpStatusCode.Created,
            $$"""{"success":true,"data":"{{newId}}"}""");
        var sut = BuildSut(handler);

        var id = await sut.CreateProjectAsync(new CreateProjectApiRequest
        {
            Slug = "new-proj",
            ThumbnailUrl = "t.jpg",
            Translations = [new() { LanguageCode = "en", Title = "New" }]
        });

        id.Should().Be(newId);
        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.AbsolutePath.Should().Be("/api/admin/projects");
        handler.LastRequestBody.Should().Contain("new-proj");
    }

    [Fact]
    public async Task UpdateProjectAsync_Success_ReturnsTrueAndUsesPut()
    {
        var id = Guid.NewGuid();
        var handler = new StubHandler(HttpStatusCode.OK, """{"success":true,"data":true}""");
        var sut = BuildSut(handler);

        var ok = await sut.UpdateProjectAsync(id, new UpdateProjectApiRequest { Slug = "s", ThumbnailUrl = "t", IsActive = true });

        ok.Should().BeTrue();
        handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
        handler.LastRequest.RequestUri!.AbsolutePath.Should().Be($"/api/admin/projects/{id}");
    }

    [Fact]
    public async Task DeleteProjectAsync_NotFound_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        var handler = new StubHandler(HttpStatusCode.NotFound, """{"success":false}""");
        var sut = BuildSut(handler);

        var ok = await sut.DeleteProjectAsync(id);

        ok.Should().BeFalse();
        handler.LastRequest!.Method.Should().Be(HttpMethod.Delete);
    }

    [Fact]
    public async Task GetCurrentAdminAsync_ReturnsUsername()
    {
        var handler = new StubHandler(HttpStatusCode.OK, """{"success":true,"data":{"username":"admin"}}""");
        var sut = BuildSut(handler);

        var user = await sut.GetCurrentAdminAsync();

        user.Should().Be("admin");
        handler.LastRequest!.RequestUri!.AbsolutePath.Should().Be("/api/admin/auth/me");
    }
}
