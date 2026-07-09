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

    [Fact]
    public async Task ChangePasswordAsync_WithCorrectCurrentPassword_UpdatesHashAndReturnsTrue()
    {
        var admin = new Admin("admin", "old_hash");
        _adminRepoMock.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>())).ReturnsAsync(admin);
        _passwordHasherMock.Setup(h => h.VerifyHashedPassword(admin, "old_hash", "current"))
            .Returns(PasswordVerificationResult.Success);
        _passwordHasherMock.Setup(h => h.HashPassword(admin, "new-strong-pass")).Returns("new_hash");

        var result = await _sut.ChangePasswordAsync("admin", new ChangePasswordRequest { CurrentPassword = "current", NewPassword = "new-strong-pass" });

        result.Should().BeTrue();
        admin.PasswordHash.Should().Be("new_hash");
        _adminRepoMock.Verify(r => r.UpdateAsync(admin, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithWrongCurrentPassword_ReturnsFalseAndDoesNotUpdate()
    {
        var admin = new Admin("admin", "old_hash");
        _adminRepoMock.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>())).ReturnsAsync(admin);
        _passwordHasherMock.Setup(h => h.VerifyHashedPassword(admin, "old_hash", "wrong"))
            .Returns(PasswordVerificationResult.Failed);

        var result = await _sut.ChangePasswordAsync("admin", new ChangePasswordRequest { CurrentPassword = "wrong", NewPassword = "new-strong-pass" });

        result.Should().BeFalse();
        admin.PasswordHash.Should().Be("old_hash");
        _adminRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Admin>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithUnknownAdmin_ReturnsFalse()
    {
        _adminRepoMock.Setup(r => r.GetByUsernameAsync("ghost", It.IsAny<CancellationToken>())).ReturnsAsync((Admin?)null);

        var result = await _sut.ChangePasswordAsync("ghost", new ChangePasswordRequest { CurrentPassword = "x", NewPassword = "new-strong-pass" });

        result.Should().BeFalse();
    }
}
