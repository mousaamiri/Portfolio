using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Services;

public class AdminCrudClientTests
{
    private sealed class StubHandler(HttpStatusCode status, string json) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (request.Content is not null) LastBody = await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        }
    }

    private sealed class SampleDto { public Guid Id { get; set; } public string Name { get; set; } = ""; }

    private static AdminCrudClient Build(StubHandler h)
        => new(new HttpClient(h) { BaseAddress = new Uri("http://localhost/") }, NullLogger<AdminCrudClient>.Instance);

    [Fact]
    public async Task ListAsync_HitsResourceEndpoint_AndParses()
    {
        var h = new StubHandler(HttpStatusCode.OK,
            """{"success":true,"data":[{"id":"11111111-1111-1111-1111-111111111111","name":"C#"}]}""");
        var sut = Build(h);

        var result = await sut.ListAsync<SampleDto>("skills", "en");

        result.Should().ContainSingle().Which.Name.Should().Be("C#");
        h.LastRequest!.RequestUri!.AbsolutePath.Should().Be("/api/admin/skills");
    }

    [Fact]
    public async Task CreateAsync_PostsToResource_ReturnsId()
    {
        var id = Guid.NewGuid();
        var h = new StubHandler(HttpStatusCode.Created, $$"""{"success":true,"data":"{{id}}"}""");
        var sut = Build(h);

        var result = await sut.CreateAsync("skills", new { name = "Docker" });

        result.Should().Be(id);
        h.LastRequest!.Method.Should().Be(HttpMethod.Post);
        h.LastRequest.RequestUri!.AbsolutePath.Should().Be("/api/admin/skills");
        h.LastBody.Should().Contain("Docker");
    }

    [Fact]
    public async Task UpdateAsync_PutsToResourceId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var h = new StubHandler(HttpStatusCode.OK, """{"success":true,"data":true}""");
        var sut = Build(h);

        var ok = await sut.UpdateAsync("faqs", id, new { });

        ok.Should().BeTrue();
        h.LastRequest!.Method.Should().Be(HttpMethod.Put);
        h.LastRequest.RequestUri!.AbsolutePath.Should().Be($"/api/admin/faqs/{id}");
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        var h = new StubHandler(HttpStatusCode.NotFound, """{"success":false}""");
        var sut = Build(h);

        var ok = await sut.DeleteAsync("interests", id);

        ok.Should().BeFalse();
        h.LastRequest!.Method.Should().Be(HttpMethod.Delete);
    }

    [Fact]
    public async Task GetAsync_NotFound_ReturnsNull()
    {
        var h = new StubHandler(HttpStatusCode.NotFound, """{"success":false}""");
        var sut = Build(h);

        var result = await sut.GetAsync<SampleDto>("skills", Guid.NewGuid(), "fa");

        result.Should().BeNull();
    }
}
