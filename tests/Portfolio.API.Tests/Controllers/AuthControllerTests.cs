using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Portfolio.Application.DTOs;

namespace Portfolio.API.Tests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturn200WithToken()
    {
        var request = new LoginRequest("admin", "TestPassword123!");

        var response = await _client.PostAsJsonAsync("/api/admin/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrWhiteSpace();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldReturn401()
    {
        var request = new LoginRequest("admin", "WrongPassword!");

        var response = await _client.PostAsJsonAsync("/api/admin/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentUsername_ShouldReturn401()
    {
        var request = new LoginRequest("nonexistent", "TestPassword123!");

        var response = await _client.PostAsJsonAsync("/api/admin/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PublicEndpoint_WithoutToken_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/public/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminEndpoint_WithoutToken_ShouldReturn401()
    {
        var response = await _client.GetAsync("/api/admin/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminEndpoint_WithValidToken_ShouldReturn200()
    {
        var loginRequest = new LoginRequest("admin", "TestPassword123!");
        var loginResponse = await _client.PostAsJsonAsync("/api/admin/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/auth/me");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult!.Token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
