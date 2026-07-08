using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.API.Common;
using Portfolio.API.Tests.Helpers;
using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Messages;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.API.Tests.Admin;

public class MessagesControllerTests : IDisposable
{
    private readonly Mock<IMessageService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly string _token = TestTokenHelper.GenerateToken();

    public MessagesControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessageService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }

    public void Dispose() => _factory.Dispose();

    private HttpClient CreateUnauthenticatedClient() => _factory.CreateClient();

    [Fact]
    public async Task GetAll_WithToken_ShouldReturn200()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<MessageDto>>.Success(new List<MessageDto>()));

        var response = await _client.GetAsync("/api/admin/messages");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithoutToken_ShouldReturn401()
    {
        var client = CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/admin/messages");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MarkAsRead_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.MarkAsReadAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var response = await _client.PutAsync($"/api/admin/messages/{id}/read", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MarkAsRead_MissingId_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.MarkAsReadAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("not found"));

        var response = await _client.PutAsync($"/api/admin/messages/{id}/read", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ExistingId_ShouldReturn200()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var response = await _client.DeleteAsync($"/api/admin/messages/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
