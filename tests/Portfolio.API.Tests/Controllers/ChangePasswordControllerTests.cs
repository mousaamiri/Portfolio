using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Portfolio.API.Common;
using Portfolio.Application.DTOs;

namespace Portfolio.API.Tests.Controllers;

// Uses its own factory instance (fresh InMemory DB) so the password change does
// not leak into the shared-fixture AuthControllerTests.
public class ChangePasswordControllerTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory = new();
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ChangePasswordControllerTests() => _client = _factory.CreateClient();

    public void Dispose() => _factory.Dispose();

    private async Task<string> LoginAsync(string password)
    {
        var response = await _client.PostAsJsonAsync("/api/admin/auth/login", new LoginRequest("admin", password));
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(JsonOptions);
        return body!.Data!.Token;
    }

    [Fact]
    public async Task ChangePassword_WithoutToken_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/admin/auth/change-password",
            new ChangePasswordRequest { CurrentPassword = "x", NewPassword = "new-strong-pass" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrent_Returns400()
    {
        var token = await LoginAsync("TestPassword123!");
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/admin/auth/change-password")
        {
            Content = JsonContent.Create(new ChangePasswordRequest { CurrentPassword = "WRONG", NewPassword = "new-strong-pass" })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_ThenLoginWithNewPassword_Works()
    {
        var token = await LoginAsync("TestPassword123!");
        var change = new HttpRequestMessage(HttpMethod.Post, "/api/admin/auth/change-password")
        {
            Content = JsonContent.Create(new ChangePasswordRequest { CurrentPassword = "TestPassword123!", NewPassword = "BrandNewPass456!" })
        };
        change.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var changeResponse = await _client.SendAsync(change);
        changeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Old password no longer works; new one does.
        var oldLogin = await _client.PostAsJsonAsync("/api/admin/auth/login", new LoginRequest("admin", "TestPassword123!"));
        oldLogin.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var newLogin = await _client.PostAsJsonAsync("/api/admin/auth/login", new LoginRequest("admin", "BrandNewPass456!"));
        newLogin.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
