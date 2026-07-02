using FluentAssertions;
using Portfolio.Domain.Admins;

namespace Portfolio.Domain.Tests.Admins;

public class AdminTests
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldCreateAdmin()
    {
        var admin = new Admin("admin", "hashed_password");

        admin.Id.Should().NotBeEmpty();
        admin.Username.Should().Be("admin");
        admin.PasswordHash.Should().Be("hashed_password");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyUsername_ShouldThrowArgumentException(string? username)
    {
        var act = () => new Admin(username!, "hashed_password");

        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("username");
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        var admin1 = new Admin("admin1", "hash1");
        var admin2 = new Admin("admin2", "hash2");

        admin1.Id.Should().NotBe(admin2.Id);
    }
}
