using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Infrastructure.Data;

namespace Portfolio.API.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "TestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                          || d.ServiceType == typeof(AppDbContext)
                          || d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
                .ToList();
            foreach (var d in descriptors)
                services.Remove(d);

            var dbName = _dbName;
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));
        });

        builder.UseSetting("JwtSettings:Secret", "TestSecretKeyThatIsAtLeast32BytesLong!!");
        builder.UseSetting("JwtSettings:Issuer", "Portfolio.API");
        builder.UseSetting("JwtSettings:Audience", "Portfolio.Client");
        builder.UseSetting("JwtSettings:ExpiryInDays", "7");
        builder.UseSetting("AdminSeed:Username", "admin");
        builder.UseSetting("AdminSeed:Password", "TestPassword123!");
    }
}
