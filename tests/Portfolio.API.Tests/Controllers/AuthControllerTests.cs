using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Portfolio.API.Common;
using Portfolio.Application.DTOs;

namespace Portfolio.API.Tests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturn200WithApiResponse()
    {
        var request = new LoginRequest("admin", "TestPassword123!");

        var response = await _client.PostAsJsonAsync("/api/admin/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(JsonOptions);
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.Token.Should().NotBeNullOrWhiteSpace();
        body.Data.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldReturn401WithApiResponse()
    {
        var request = new LoginRequest("admin", "WrongPassword!");

        var response = await _client.PostAsJsonAsync("/api/admin/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body.Should().NotBeNull();
        body!.Success.Should().BeFalse();
        body.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithNonExistentUsername_ShouldReturn401WithApiResponse()
    {
        var request = new LoginRequest("nonexistent", "TestPassword123!");

        var response = await _client.PostAsJsonAsync("/api/admin/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body.Should().NotBeNull();
        body!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task PublicEndpoint_WithoutToken_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/public/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>(JsonOptions);
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task AdminEndpoint_WithoutToken_ShouldReturn401WithApiResponse()
    {
        var response = await _client.GetAsync("/api/admin/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions);
        body.Should().NotBeNull();
        body!.Success.Should().BeFalse();
        body.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task AdminEndpoint_WithValidToken_ShouldReturn200WithApiResponse()
    {
        var loginRequest = new LoginRequest("admin", "TestPassword123!");
        var loginResponse = await _client.PostAsJsonAsync("/api/admin/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(JsonOptions);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/auth/me");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult!.Data!.Token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>(JsonOptions);
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Data.GetProperty("username").GetString().Should().Be("admin");
    }
}
