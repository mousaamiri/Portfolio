using Microsoft.AspNetCore.Identity;
using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Admins;

namespace Portfolio.Application.Services;

public class AuthService(
    IAdminRepository adminRepository,
    ITokenService tokenService,
    IPasswordHasher<Admin> passwordHasher)
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var admin = await adminRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (admin is null)
            return null;

        var result = passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            return null;

        var token = tokenService.GenerateToken(admin.Id, admin.Username);
        var expiresAt = DateTime.UtcNow.AddDays(tokenService.ExpiryInDays);

        return new LoginResponse(token, expiresAt);
    }
}
