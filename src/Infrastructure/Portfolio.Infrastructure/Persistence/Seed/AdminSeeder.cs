using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portfolio.Domain.Admins;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Persistence.Seed;

public class AdminSeeder(
    AppDbContext context,
    IPasswordHasher<Admin> passwordHasher,
    ILogger<AdminSeeder> logger)
{
    public async Task SeedAsync(string username, string password)
    {
        if (await context.Admins.AnyAsync())
        {
            logger.LogInformation("Admin already exists, skipping seed");
            return;
        }

        var hash = passwordHasher.HashPassword(null!, password);
        var admin = new Admin(username, hash);

        await context.Admins.AddAsync(admin);
        await context.SaveChangesAsync();

        logger.LogInformation("Default admin user created");
    }
}
