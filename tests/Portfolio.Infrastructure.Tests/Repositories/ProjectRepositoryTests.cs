using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure.Tests.Repositories;

public class ProjectRepositoryTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    private static Project CreateProject() => new()
    {
        Id = Guid.NewGuid(),
        ImageUrl = "project.png",
        DemoUrl = "https://demo.com",
        SourceCodeUrl = "https://github.com/test",
        DisplayOrder = 1,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    [Fact]
    public async Task AddAsync_ShouldPersistProject()
    {
        var dbName = Guid.NewGuid().ToString();
        var project = CreateProject();

        await using (var context = CreateContext(dbName))
        {
            var repo = new ProjectRepository(context);
            await repo.AddAsync(project);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Projects.FindAsync(project.Id);
            result.Should().NotBeNull();
            result!.DemoUrl.Should().Be("https://demo.com");
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnProject()
    {
        var dbName = Guid.NewGuid().ToString();
        var project = CreateProject();

        await using (var context = CreateContext(dbName))
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ProjectRepository(context);
            var result = await repo.GetByIdAsync(project.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(project.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProjectRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProjects()
    {
        var dbName = Guid.NewGuid().ToString();

        await using (var context = CreateContext(dbName))
        {
            context.Projects.AddRange(CreateProject(), CreateProject(), CreateProject());
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ProjectRepository(context);
            var result = await repo.GetAllAsync();
            result.Should().HaveCount(3);
        }
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyList()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new ProjectRepository(context);

        var result = await repo.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingProjects()
    {
        var dbName = Guid.NewGuid().ToString();
        var first = CreateProject();
        first.DisplayOrder = 1;
        var second = CreateProject();
        second.DisplayOrder = 2;
        var third = CreateProject();
        third.DisplayOrder = 2;

        await using (var context = CreateContext(dbName))
        {
            context.Projects.AddRange(first, second, third);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ProjectRepository(context);
            var result = await repo.FindAsync(p => p.DisplayOrder == 2);
            result.Should().HaveCount(2);
        }
    }

    [Fact]
    public async Task Update_ShouldPersistChanges()
    {
        var dbName = Guid.NewGuid().ToString();
        var project = CreateProject();

        await using (var context = CreateContext(dbName))
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ProjectRepository(context);
            var entity = await repo.GetByIdAsync(project.Id);
            entity!.DisplayOrder = 99;
            repo.Update(entity);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Projects.FindAsync(project.Id);
            result!.DisplayOrder.Should().Be(99);
        }
    }

    [Fact]
    public async Task Delete_ShouldRemoveProject()
    {
        var dbName = Guid.NewGuid().ToString();
        var project = CreateProject();

        await using (var context = CreateContext(dbName))
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new ProjectRepository(context);
            var entity = await repo.GetByIdAsync(project.Id);
            repo.Delete(entity!);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Projects.FindAsync(project.Id);
            result.Should().BeNull();
        }
    }
}
