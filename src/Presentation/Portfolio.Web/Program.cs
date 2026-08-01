using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Web.Localization;
using Portfolio.Web.Services.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Trust X-Forwarded-* from a reverse proxy so RemoteIpAddress is the visitor's
// real IP. The "contact" rate-limit partitions by it — without this, everyone
// behind a proxy shares one partition and the 3/10min limit is consumed globally.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// Per-request localization state (language + UI-chrome map), populated by
// LanguageMiddleware from the portfolio-lang cookie.
builder.Services.AddScoped<LocalizationState>();

// Typed clients over Portfolio.API. The public client hits anonymous read
// endpoints; the admin client hits the authenticated api/admin/* endpoints.
var configuredBaseUrl = builder.Configuration["PortfolioApi:BaseUrl"];
// Outside Development this fallback almost certainly can't reach the API, so every
// content page and contact-form submit fails silently — warn after the app builds.
var baseUrlMissing = string.IsNullOrWhiteSpace(configuredBaseUrl);
var apiBaseUrl = baseUrlMissing ? "https://localhost:7003" : configuredBaseUrl!;
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

if (baseUrlMissing && !app.Environment.IsDevelopment())
{
    app.Logger.LogWarning(
        "PortfolioApi:BaseUrl is not configured; falling back to {Fallback}. " +
        "Set PortfolioApi:BaseUrl for this environment or the site cannot reach the API.",
        apiBaseUrl);
}

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

// Must run before rate limiting so the "contact" limiter partitions by the real
// client IP rather than the reverse proxy's.
app.UseForwardedHeaders();

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
