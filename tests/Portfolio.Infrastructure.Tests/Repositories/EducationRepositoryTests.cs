using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities.Educations;
using Portfolio.Domain.Enums;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure.Tests.Repositories;

public class EducationRepositoryTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    private static Education CreateEducation() => new()
    {
        Id = Guid.NewGuid(),
        InstitutionLogo = "logo.png",
        InstitutionUrl = "https://example.com",
        StartDate = new DateTime(2020, 9, 1),
        EndDate = new DateTime(2024, 6, 1),
        Gpa = 3.8,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    [Fact]
    public async Task GetAllWithTranslationsAsync_ShouldEagerLoadTranslations()
    {
        var dbName = Guid.NewGuid().ToString();
        var education = CreateEducation();
        education.Translations.Add(new EducationTranslation
        {
            Id = Guid.NewGuid(),
            EducationId = education.Id,
            Language = Language.En,
            InstitutionName = "MIT",
            Degree = "BSc",
            FieldOfStudy = "CS",
            Description = "desc"
        });

        await using (var context = CreateContext(dbName))
        {
            context.Educations.Add(education);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new EducationRepository(context);
            var result = await repo.GetAllWithTranslationsAsync();

            result.Should().HaveCount(1);
            result[0].Translations.Should().ContainSingle();
            result[0].Translations.First().InstitutionName.Should().Be("MIT");
        }
    }

    [Fact]
    public async Task AddAsync_ShouldPersistEducation()
    {
        var dbName = Guid.NewGuid().ToString();
        var education = CreateEducation();

        await using (var context = CreateContext(dbName))
        {
            var repo = new EducationRepository(context);
            await repo.AddAsync(education);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Educations.FindAsync(education.Id);
            result.Should().NotBeNull();
            result!.InstitutionUrl.Should().Be("https://example.com");
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnEducation()
    {
        var dbName = Guid.NewGuid().ToString();
        var education = CreateEducation();

        await using (var context = CreateContext(dbName))
        {
            context.Educations.Add(education);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new EducationRepository(context);
            var result = await repo.GetByIdAsync(education.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(education.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new EducationRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEducations()
    {
        var dbName = Guid.NewGuid().ToString();
        var education1 = CreateEducation();
        var education2 = CreateEducation();

        await using (var context = CreateContext(dbName))
        {
            context.Educations.AddRange(education1, education2);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new EducationRepository(context);
            var result = await repo.GetAllAsync();
            result.Should().HaveCount(2);
        }
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyList()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new EducationRepository(context);

        var result = await repo.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingEducations()
    {
        var dbName = Guid.NewGuid().ToString();
        var active = CreateEducation();
        active.IsActive = true;
        var inactive = CreateEducation();
        inactive.IsActive = false;

        await using (var context = CreateContext(dbName))
        {
            context.Educations.AddRange(active, inactive);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new EducationRepository(context);
            var result = await repo.FindAsync(e => e.IsActive);
            result.Should().ContainSingle();
            result[0].Id.Should().Be(active.Id);
        }
    }

    [Fact]
    public async Task Update_ShouldPersistChanges()
    {
        var dbName = Guid.NewGuid().ToString();
        var education = CreateEducation();

        await using (var context = CreateContext(dbName))
        {
            context.Educations.Add(education);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new EducationRepository(context);
            var entity = await repo.GetByIdAsync(education.Id);
            entity!.Gpa = 4.0;
            repo.Update(entity);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Educations.FindAsync(education.Id);
            result!.Gpa.Should().Be(4.0);
        }
    }

    [Fact]
    public async Task Delete_ShouldRemoveEducation()
    {
        var dbName = Guid.NewGuid().ToString();
        var education = CreateEducation();

        await using (var context = CreateContext(dbName))
        {
            context.Educations.Add(education);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new EducationRepository(context);
            var entity = await repo.GetByIdAsync(education.Id);
            repo.Delete(entity!);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Educations.FindAsync(education.Id);
            result.Should().BeNull();
        }
    }
}
