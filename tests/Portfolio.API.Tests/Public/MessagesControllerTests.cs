using System.Net;
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

namespace Portfolio.API.Tests.Public;

public class MessagesControllerTests : IDisposable
{
    private readonly Mock<IMessageService> _mockService = new();
    private readonly MockWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public MessagesControllerTests()
    {
        _factory = new MockWebApplicationFactory(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessageService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddScoped(_ => _mockService.Object);
        });
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Post_ValidMessage_ShouldReturn200AndCreateWithoutAuth()
    {
        var newId = Guid.NewGuid();
        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(newId));

        var request = new CreateMessageRequest
        {
            Name = "Jane", Email = "jane@example.com", Body = "Hello there", Interest = "Freelance"
        };

        var response = await _client.PostAsJsonAsync("/api/public/messages", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(JsonOptions);
        body!.Success.Should().BeTrue();
        body.Data.Should().Be(newId);
    }

    [Fact]
    public async Task Post_InvalidMessage_ShouldReturn400()
    {
        var request = new { Name = "", Email = "not-an-email", Body = "" };

        var response = await _client.PostAsJsonAsync("/api/public/messages", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body!.Success.Should().BeFalse();
        body.Errors.Should().NotBeNullOrEmpty();
    }
}
