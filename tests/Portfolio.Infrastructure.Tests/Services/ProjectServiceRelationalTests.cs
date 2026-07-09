using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Services;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Domain.Enums;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Tests.Services;

/// <summary>
/// Regression tests against a REAL relational provider (SQLite). The admin update
/// path rebuilds an entity's translations; on a relational store EF enforces
/// rows-affected, so a naive Clear()+re-add (new keys) throws
/// DbUpdateConcurrencyException. The in-memory provider does NOT catch this, which
/// is why it slipped past the existing integration tests. These lock the fix in.
/// </summary>
public class ProjectServiceRelationalTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public ProjectServiceRelationalTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        using var ctx = new AppDbContext(_options);
        ctx.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private AppDbContext NewContext() => new(_options);

    private async Task<Guid> SeedProjectAsync()
    {
        await using var ctx = NewContext();
        var service = new ProjectService(new Portfolio.Infrastructure.Data.UnitOfWork(ctx));
        var result = await service.CreateAsync(new CreateProjectRequest
        {
            Slug = "vitastic",
            ThumbnailUrl = "/t.jpg",
            Translations =
            [
                new() { LanguageCode = "en", Title = "Vitastic", ShortDescription = "LMS" },
                new() { LanguageCode = "fa", Title = "ویتاستیک", ShortDescription = "سامانه" }
            ]
        });
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    [Fact]
    public async Task UpdateAsync_ChangingTranslations_PersistsWithoutConcurrencyError()
    {
        var id = await SeedProjectAsync();

        await using (var ctx = NewContext())
        {
            var service = new ProjectService(new Portfolio.Infrastructure.Data.UnitOfWork(ctx));
            var result = await service.UpdateAsync(id, new UpdateProjectRequest
            {
                Slug = "vitastic",
                ThumbnailUrl = "/t2.jpg",
                IsActive = true,
                Translations =
                [
                    new() { LanguageCode = "en", Title = "Vitastic (edited)", ShortDescription = "LMS v2" },
                    new() { LanguageCode = "fa", Title = "ویتاستیک (ویرایش)", ShortDescription = "سامانه" }
                ]
            });

            result.IsSuccess.Should().BeTrue();
        }

        await using (var verify = NewContext())
        {
            var project = await verify.Projects.Include(p => p.Translations).FirstAsync(p => p.Id == id);
            project.ThumbnailUrl.Should().Be("/t2.jpg");
            project.Translations.Should().HaveCount(2);
            project.Translations.Single(t => t.Language == Language.En).Title.Should().Be("Vitastic (edited)");
        }
    }

    [Fact]
    public async Task UpdateAsync_RemovingALanguage_DeletesThatTranslation()
    {
        var id = await SeedProjectAsync();

        await using (var ctx = NewContext())
        {
            var service = new ProjectService(new Portfolio.Infrastructure.Data.UnitOfWork(ctx));
            var result = await service.UpdateAsync(id, new UpdateProjectRequest
            {
                Slug = "vitastic",
                ThumbnailUrl = "/t.jpg",
                IsActive = true,
                Translations = [new() { LanguageCode = "en", Title = "Only English" }]
            });
            result.IsSuccess.Should().BeTrue();
        }

        await using (var verify = NewContext())
        {
            var project = await verify.Projects.Include(p => p.Translations).FirstAsync(p => p.Id == id);
            project.Translations.Should().ContainSingle();
            project.Translations.Single().Language.Should().Be(Language.En);
        }
    }
}
