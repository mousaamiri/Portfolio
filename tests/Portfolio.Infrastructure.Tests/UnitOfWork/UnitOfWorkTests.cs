using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Educations;
using Portfolio.Domain.Entities.Experiences;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Domain.Entities.Skills;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure.Tests.UnitOfWork;

public class UnitOfWorkTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void Educations_ShouldReturnEducationRepository()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        using var uow = new Data.UnitOfWork(context);

        uow.Educations.Should().NotBeNull();
        uow.Educations.Should().BeAssignableTo<IEducationRepository>();
    }

    [Fact]
    public void Experiences_ShouldReturnExperienceRepository()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        using var uow = new Data.UnitOfWork(context);

        uow.Experiences.Should().NotBeNull();
        uow.Experiences.Should().BeAssignableTo<IExperienceRepository>();
    }

    [Fact]
    public void Projects_ShouldReturnProjectRepository()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        using var uow = new Data.UnitOfWork(context);

        uow.Projects.Should().NotBeNull();
        uow.Projects.Should().BeAssignableTo<IProjectRepository>();
    }

    [Fact]
    public void Skills_ShouldReturnSkillRepository()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        using var uow = new Data.UnitOfWork(context);

        uow.Skills.Should().NotBeNull();
        uow.Skills.Should().BeAssignableTo<ISkillRepository>();
    }

    [Fact]
    public void RepositoryProperties_ShouldReturnSameInstanceOnMultipleAccess()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        using var uow = new Data.UnitOfWork(context);

        var first = uow.Educations;
        var second = uow.Educations;

        first.Should().BeSameAs(second);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChangesAcrossMultipleRepositories()
    {
        var dbName = Guid.NewGuid().ToString();
        var project = new Project
        {
            Id = Guid.NewGuid(),
            ThumbnailUrl = "img.png",
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        var skill = new Skill
        {
            Id = Guid.NewGuid(),
            IconUrl = "icon.svg",
            Proficiency = 80,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await using (var context = CreateContext(dbName))
        {
            using var uow = new Data.UnitOfWork(context);
            await uow.Projects.AddAsync(project);
            await uow.Skills.AddAsync(skill);
            var saved = await uow.SaveChangesAsync();
            saved.Should().Be(2);
        }

        await using (var context = CreateContext(dbName))
        {
            var savedProject = await context.Projects.FindAsync(project.Id);
            var savedSkill = await context.Skills.FindAsync(skill.Id);
            savedProject.Should().NotBeNull();
            savedSkill.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task SaveChangesAsync_WithoutChanges_ShouldReturnZero()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        using var uow = new Data.UnitOfWork(context);

        var result = await uow.SaveChangesAsync();

        result.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistAddAndUpdateTogether()
    {
        var dbName = Guid.NewGuid().ToString();
        var education = new Education
        {
            Id = Guid.NewGuid(),
            InstitutionLogo = "logo.png",
            InstitutionUrl = "https://uni.com",
            StartDate = new DateTime(2020, 9, 1),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await using (var context = CreateContext(dbName))
        {
            context.Educations.Add(education);
            await context.SaveChangesAsync();
        }

        var experience = new Experience
        {
            Id = Guid.NewGuid(),
            CompanyLogo = "co.png",
            CompanyUrl = "https://co.com",
            StartDate = new DateTime(2022, 1, 1),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await using (var context = CreateContext(dbName))
        {
            using var uow = new Data.UnitOfWork(context);

            var existingEdu = await uow.Educations.GetByIdAsync(education.Id);
            existingEdu!.Gpa = 3.9;
            uow.Educations.Update(existingEdu);

            await uow.Experiences.AddAsync(experience);

            var saved = await uow.SaveChangesAsync();
            saved.Should().Be(2);
        }

        await using (var context = CreateContext(dbName))
        {
            var updatedEdu = await context.Educations.FindAsync(education.Id);
            updatedEdu!.Gpa.Should().Be(3.9);

            var newExp = await context.Experiences.FindAsync(experience.Id);
            newExp.Should().NotBeNull();
        }
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        var dbName = Guid.NewGuid().ToString();
        var context = CreateContext(dbName);
        var uow = new Data.UnitOfWork(context);

        var act = () => uow.Dispose();

        act.Should().NotThrow();
    }
}
