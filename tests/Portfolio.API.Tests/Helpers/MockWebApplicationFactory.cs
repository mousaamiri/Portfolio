using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Infrastructure.Data;

namespace Portfolio.API.Tests.Helpers;

public class MockWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection> _configureTestServices;

    public MockWebApplicationFactory(Action<IServiceCollection> configureTestServices)
    {
        _configureTestServices = configureTestServices;
    }

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

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));

            _configureTestServices(services);
        });

        builder.UseSetting("JwtSettings:Secret", TestTokenHelper.Secret);
        builder.UseSetting("JwtSettings:Issuer", TestTokenHelper.Issuer);
        builder.UseSetting("JwtSettings:Audience", TestTokenHelper.Audience);
        builder.UseSetting("JwtSettings:ExpiryInDays", "7");
        builder.UseSetting("AdminSeed:Username", "admin");
        builder.UseSetting("AdminSeed:Password", "TestPassword123!");
    }
}
