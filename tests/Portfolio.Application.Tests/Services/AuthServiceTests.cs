using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Services;
using Portfolio.Domain.Admins;

namespace Portfolio.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IAdminRepository> _adminRepoMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IPasswordHasher<Admin>> _passwordHasherMock = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _tokenServiceMock.Setup(x => x.ExpiryInDays).Returns(7);
        _sut = new AuthService(_adminRepoMock.Object, _tokenServiceMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        var admin = new Admin("admin", "hashed_pw");
        _adminRepoMock.Setup(r => r.GetByUsernameAsync("admin", default))
            .ReturnsAsync(admin);
        _passwordHasherMock.Setup(h => h.VerifyHashedPassword(admin, "hashed_pw", "password"))
            .Returns(PasswordVerificationResult.Success);
        _tokenServiceMock.Setup(t => t.GenerateToken(admin.Id, "admin"))
            .Returns("jwt_token");

        var result = await _sut.LoginAsync(new LoginRequest("admin", "password"));

        result.Should().NotBeNull();
        result!.Token.Should().Be("jwt_token");
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ShouldReturnNull()
    {
        var admin = new Admin("admin", "hashed_pw");
        _adminRepoMock.Setup(r => r.GetByUsernameAsync("admin", default))
            .ReturnsAsync(admin);
        _passwordHasherMock.Setup(h => h.VerifyHashedPassword(admin, "hashed_pw", "wrong"))
            .Returns(PasswordVerificationResult.Failed);

        var result = await _sut.LoginAsync(new LoginRequest("admin", "wrong"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUsername_ShouldReturnNull()
    {
        _adminRepoMock.Setup(r => r.GetByUsernameAsync("unknown", default))
            .ReturnsAsync((Admin?)null);

        var result = await _sut.LoginAsync(new LoginRequest("unknown", "password"));

        result.Should().BeNull();
    }
}
