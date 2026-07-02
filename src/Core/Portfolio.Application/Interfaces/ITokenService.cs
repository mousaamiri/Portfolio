namespace Portfolio.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(Guid adminId, string username);
    int ExpiryInDays { get; }
}
