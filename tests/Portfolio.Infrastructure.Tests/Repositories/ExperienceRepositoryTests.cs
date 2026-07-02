using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities.Experiences;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure.Tests.Repositories;

public class ExperienceRepositoryTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    private static Experience CreateExperience() => new()
    {
        Id = Guid.NewGuid(),
        CompanyLogo = "company.png",
        CompanyUrl = "https://company.com",
        StartDate = new DateTime(2021, 1, 1),
        EndDate = new DateTime(2023, 12, 31),
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    [Fact]
    public async Task AddAsync_ShouldPersistExperience()
    {
        var dbName = Guid.NewGuid().ToString();
        var experience = CreateExperience();

        await using (var context = CreateContext(dbName))
        {
            var repo = new ExperienceRepository(context);
            await repo.AddAsync(experience);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Experiences.FindAsync(experience.Id);
            result.Should().NotBeNull();
            result!.CompanyUrl.Should().Be("https://company.com");
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnExperience()
    {
        var dbName = Guid.NewGuid().ToString();
        var experience = CreateExperience();

        await using (var context = CreateContext(dbName))
        {
            context.Experiences.Add(experience);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ExperienceRepository(context);
            var result = await repo.GetByIdAsync(experience.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(experience.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ExperienceRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllExperiences()
    {
        var dbName = Guid.NewGuid().ToString();

        await using (var context = CreateContext(dbName))
        {
            context.Experiences.AddRange(CreateExperience(), CreateExperience());
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ExperienceRepository(context);
            var result = await repo.GetAllAsync();
            result.Should().HaveCount(2);
        }
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyList()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ExperienceRepository(context);

        var result = await repo.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingExperiences()
    {
        var dbName = Guid.NewGuid().ToString();
        var current = CreateExperience();
        current.EndDate = null;
        var past = CreateExperience();

        await using (var context = CreateContext(dbName))
        {
            context.Experiences.AddRange(current, past);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ExperienceRepository(context);
            var result = await repo.FindAsync(e => e.EndDate == null);
            result.Should().ContainSingle();
            result[0].Id.Should().Be(current.Id);
        }
    }

    [Fact]
    public async Task Update_ShouldPersistChanges()
    {
        var dbName = Guid.NewGuid().ToString();
        var experience = CreateExperience();

        await using (var context = CreateContext(dbName))
        {
            context.Experiences.Add(experience);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ExperienceRepository(context);
            var entity = await repo.GetByIdAsync(experience.Id);
            entity!.CompanyUrl = "https://updated.com";
            repo.Update(entity);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Experiences.FindAsync(experience.Id);
            result!.CompanyUrl.Should().Be("https://updated.com");
        }
    }

    [Fact]
    public async Task Delete_ShouldRemoveExperience()
    {
        var dbName = Guid.NewGuid().ToString();
        var experience = CreateExperience();

        await using (var context = CreateContext(dbName))
        {
            context.Experiences.Add(experience);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ExperienceRepository(context);
            var entity = await repo.GetByIdAsync(experience.Id);
            repo.Delete(entity!);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Experiences.FindAsync(experience.Id);
            result.Should().BeNull();
        }
    }
}
