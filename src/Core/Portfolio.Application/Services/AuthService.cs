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

    /// <summary>
    /// Changes the given admin's password after verifying the current one.
    /// Returns false if the admin is unknown or the current password is wrong.
    /// </summary>
    public async Task<bool> ChangePasswordAsync(string username, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var admin = await adminRepository.GetByUsernameAsync(username, cancellationToken);
        if (admin is null)
            return false;

        var verification = passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, request.CurrentPassword);
        if (verification == PasswordVerificationResult.Failed)
            return false;

        var newHash = passwordHasher.HashPassword(admin, request.NewPassword);
        admin.ChangePassword(newHash);
        await adminRepository.UpdateAsync(admin, cancellationToken);

        return true;
    }
}
