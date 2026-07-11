using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Persistence.Seed;

namespace Portfolio.Infrastructure.Tests.Persistence;

public class ContentSeederTests
{
    private static AppDbContext CreateContext(string dbName)
        => new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(dbName).Options);

    [Fact]
    public async Task SeedAsync_PopulatesRealContent()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var sut = new ContentSeeder(context, NullLogger<ContentSeeder>.Instance);

        await sut.SeedAsync();

        (await context.Profiles.CountAsync()).Should().Be(1);
        (await context.Skills.CountAsync()).Should().BeGreaterThan(0);
        (await context.Projects.CountAsync()).Should().Be(3);
        (await context.Interests.CountAsync()).Should().BeGreaterThan(0);
        (await context.StatCounters.CountAsync()).Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SeedAsync_IsIdempotent()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var sut = new ContentSeeder(context, NullLogger<ContentSeeder>.Instance);

        await sut.SeedAsync();
        var projectsAfterFirst = await context.Projects.CountAsync();
        await sut.SeedAsync();

        (await context.Projects.CountAsync()).Should().Be(projectsAfterFirst);
        (await context.Profiles.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task SeedAsync_ProfileHasBothLanguages()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var sut = new ContentSeeder(context, NullLogger<ContentSeeder>.Instance);

        await sut.SeedAsync();

        var profile = await context.Profiles.Include(p => p.Translations).FirstAsync();
        profile.Email.Should().Be("mousaamiri.code@gmail.com");
        profile.Translations.Should().HaveCount(2);
    }
}
