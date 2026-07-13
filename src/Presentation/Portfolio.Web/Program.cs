using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Web.Localization;
using Portfolio.Web.Services.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Per-request localization state (language + UI-chrome map), populated by
// LanguageMiddleware from the portfolio-lang cookie.
builder.Services.AddScoped<LocalizationState>();

// Typed clients over Portfolio.API. The public client hits anonymous read
// endpoints; the admin client hits the authenticated api/admin/* endpoints.
var apiBaseUrl = builder.Configuration["PortfolioApi:BaseUrl"] ?? "https://localhost:7003";
builder.Services.AddHttpClient<IPortfolioApiClient, PortfolioApiClient>(
    client => client.BaseAddress = new Uri(apiBaseUrl));

// The admin client's requests carry the admin's JWT (from the auth cookie) as a
// Bearer header via BearerTokenHandler.
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<BearerTokenHandler>();
builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>(
    client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddHttpClient<IAdminCrudClient, AdminCrudClient>(
    client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<BearerTokenHandler>();

// Cookie authentication for the admin panel (MVC proxy). The API JWT is stored
// inside the encrypted, HttpOnly auth cookie — it never reaches the browser.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/Login";
        options.Cookie.Name = "Portfolio.Admin";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

// Per-visitor rate limiting for the public contact form. Partitioned by the
// real client IP (the Web layer, unlike the API, sees the visitor's address).
// 3 submissions / 10 minutes; excess is rejected with 429 (contact.js shows a
// "too many attempts" message).
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("contact", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // Any unhandled exception (notably ApiUnavailableException when Portfolio.API /
    // the database is unreachable) renders the clean error page instead of a 500.
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Resolve language from the cookie and load the UI-chrome map (non-English)
// before MVC renders. Placed after auth so the admin area can be skipped inside.
app.UseMiddleware<LanguageMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program;
