using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Portfolio.Infrastructure.Auth;

namespace Portfolio.Infrastructure.Tests.Auth;

public class TokenServiceTests
{
    private readonly TokenService _sut;
    private readonly JwtSettings _settings = new()
    {
        Secret = "ThisIsASecretKeyForTestingPurposesOnly_MustBe32BytesLong!",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpiryInDays = 7
    };

    public TokenServiceTests()
    {
        _sut = new TokenService(Options.Create(_settings));
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwt()
    {
        var adminId = Guid.NewGuid();

        var token = _sut.GenerateToken(adminId, "admin");

        token.Should().NotBeNullOrWhiteSpace();
        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(token).Should().BeTrue();
    }

    [Fact]
    public void GenerateToken_ShouldContainCorrectClaims()
    {
        var adminId = Guid.NewGuid();

        var token = _sut.GenerateToken(adminId, "admin");

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == adminId.ToString());
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == "admin");
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public void GenerateToken_ShouldSetCorrectExpiry()
    {
        var adminId = Guid.NewGuid();

        var token = _sut.GenerateToken(adminId, "admin");

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void GenerateToken_ShouldSetCorrectIssuerAndAudience()
    {
        var adminId = Guid.NewGuid();

        var token = _sut.GenerateToken(adminId, "admin");

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Issuer.Should().Be("TestIssuer");
        jwt.Audiences.Should().Contain("TestAudience");
    }

    [Fact]
    public void ExpiryInDays_ShouldReturnConfiguredValue()
    {
        _sut.ExpiryInDays.Should().Be(7);
    }
}
