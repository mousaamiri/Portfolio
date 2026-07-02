using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities.Skills;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure.Tests.Repositories;

public class SkillRepositoryTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    private static Skill CreateSkill() => new()
    {
        Id = Guid.NewGuid(),
        IconUrl = "csharp.svg",
        Proficiency = 90,
        DisplayOrder = 1,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    [Fact]
    public async Task AddAsync_ShouldPersistSkill()
    {
        var dbName = Guid.NewGuid().ToString();
        var skill = CreateSkill();

        await using (var context = CreateContext(dbName))
        {
            var repo = new SkillRepository(context);
            await repo.AddAsync(skill);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Skills.FindAsync(skill.Id);
            result.Should().NotBeNull();
            result!.Proficiency.Should().Be(90);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnSkill()
    {
        var dbName = Guid.NewGuid().ToString();
        var skill = CreateSkill();

        await using (var context = CreateContext(dbName))
        {
            context.Skills.Add(skill);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new SkillRepository(context);
            var result = await repo.GetByIdAsync(skill.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(skill.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new SkillRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSkills()
    {
        var dbName = Guid.NewGuid().ToString();

        await using (var context = CreateContext(dbName))
        {
            context.Skills.AddRange(CreateSkill(), CreateSkill());
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new SkillRepository(context);
            var result = await repo.GetAllAsync();
            result.Should().HaveCount(2);
        }
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyList()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new SkillRepository(context);

        var result = await repo.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingSkills()
    {
        var dbName = Guid.NewGuid().ToString();
        var high = CreateSkill();
        high.Proficiency = 95;
        var low = CreateSkill();
        low.Proficiency = 40;

        await using (var context = CreateContext(dbName))
        {
            context.Skills.AddRange(high, low);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new SkillRepository(context);
            var result = await repo.FindAsync(s => s.Proficiency > 80);
            result.Should().ContainSingle();
            result[0].Id.Should().Be(high.Id);
        }
    }

    [Fact]
    public async Task Update_ShouldPersistChanges()
    {
        var dbName = Guid.NewGuid().ToString();
        var skill = CreateSkill();

        await using (var context = CreateContext(dbName))
        {
            context.Skills.Add(skill);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new SkillRepository(context);
            var entity = await repo.GetByIdAsync(skill.Id);
            entity!.Proficiency = 100;
            repo.Update(entity);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Skills.FindAsync(skill.Id);
            result!.Proficiency.Should().Be(100);
        }
    }

    [Fact]
    public async Task Delete_ShouldRemoveSkill()
    {
        var dbName = Guid.NewGuid().ToString();
        var skill = CreateSkill();

        await using (var context = CreateContext(dbName))
        {
            context.Skills.Add(skill);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new SkillRepository(context);
            var entity = await repo.GetByIdAsync(skill.Id);
            repo.Delete(entity!);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Skills.FindAsync(skill.Id);
            result.Should().BeNull();
        }
    }
}
