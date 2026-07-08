using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities.Messages;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure.Tests.Repositories;

public class MessageRepositoryTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    private static Message CreateMessage(DateTime createdAt) => new()
    {
        Id = Guid.NewGuid(),
        Name = "Jane",
        Email = "jane@example.com",
        Body = "Hello",
        IsActive = true,
        CreatedAt = createdAt
    };

    [Fact]
    public async Task AddAsync_ShouldPersistMessage()
    {
        var dbName = Guid.NewGuid().ToString();
        var message = CreateMessage(DateTime.UtcNow);

        await using (var context = CreateContext(dbName))
        {
            var repo = new MessageRepository(context);
            await repo.AddAsync(message);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var result = await context.Messages.FindAsync(message.Id);
            result.Should().NotBeNull();
            result!.Email.Should().Be("jane@example.com");
        }
    }

    [Fact]
    public async Task GetAllOrderedByNewestAsync_ReturnsNewestFirst()
    {
        var dbName = Guid.NewGuid().ToString();
        var older = CreateMessage(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var newer = CreateMessage(new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc));

        await using (var context = CreateContext(dbName))
        {
            context.Messages.AddRange(older, newer);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(dbName))
        {
            var repo = new MessageRepository(context);
            var result = await repo.GetAllOrderedByNewestAsync();

            result.Should().HaveCount(2);
            result[0].Id.Should().Be(newer.Id);
            result[1].Id.Should().Be(older.Id);
        }
    }
}
