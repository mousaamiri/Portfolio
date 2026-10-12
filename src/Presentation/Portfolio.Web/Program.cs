using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Portfolio.Web.Features;
using Portfolio.Web.Localization;
using Portfolio.Web.Services.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// HTML/CSS/JS are highly compressible; UiTranslations-heavy pages shrink 60–80%.
// No secrets in these responses (contact-form POSTs are neither cached nor
// compressed), so compression is enabled for HTTPS too.
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

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

// Optional public sections (currently the Blog). A disabled section is unlinked
// from every navigation + SEO surface; flipping the flag re-links it.
builder.Services.Configure<FeatureFlags>(builder.Configuration.GetSection("Features"));

// Typed clients over Portfolio.API. The public client hits anonymous read
// endpoints; the admin client hits the authenticated api/admin/* endpoints.
var configuredBaseUrl = builder.Configuration["PortfolioApi:BaseUrl"];
// Outside Development this fallback almost certainly can't reach the API, so every
// content page and contact-form submit fails silently — warn after the app builds.
var baseUrlMissing = string.IsNullOrWhiteSpace(configuredBaseUrl);
var apiBaseUrl = baseUrlMissing ? "https://localhost:7003" : configuredBaseUrl!;
// A transient blip in front of the API (a proxy 403/404/429 or a 5xx during an
// app-pool recycle or the FTP deploy window) is retried before the visitor sees it,
// so a content page self-heals (second render has data) instead of flashing an empty
// shell or the error page for a second or two. Added to the public client only — the
// admin clients deliberately surface failures to the signed-in admin.
builder.Services.AddHttpClient<IPortfolioApiClient, PortfolioApiClient>(
    client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        // Bound well below the 100s default so a hung API call fails fast and the
        // retry below still fits inside a sane page-load budget.
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddResilienceHandler("public-api", pipeline => pipeline.AddRetry(
        new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 2,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromMilliseconds(500),
            // The built-in default retries only 408/429/5xx. A reverse proxy in front
            // of the API answers 403/404 while the route is briefly gone, so those are
            // added here. Network-level failures (HttpRequestException) always retry.
            ShouldHandle = args => ValueTask.FromResult(
                args.Outcome.Exception is HttpRequestException
                || args.Outcome.Result?.StatusCode is { } status && IsTransientProxyBlip(status))
        }));

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

// Before UseStaticFiles so served files are compressed; before response caching is
// not needed here (the Web app does not cache responses server-side).
app.UseResponseCompression();

// Static assets are content-addressed via asp-append-version in the views, so they
// can be cached immutably for a year: a file edit changes its hash and thus its URL.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx => ctx.Context.Response.Headers.CacheControl = "public, max-age=31536000, immutable"
});

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

public partial class Program
{
    // Status codes the public-api retry treats as transient. Beyond the usual set
    // (408 / 429 / 5xx), this includes the 403/404 a reverse proxy emits while the
    // downstream route is briefly unavailable.
    internal static bool IsTransientProxyBlip(HttpStatusCode status)
        => status is HttpStatusCode.Forbidden
            or HttpStatusCode.NotFound
            or HttpStatusCode.RequestTimeout
            or HttpStatusCode.TooManyRequests
            or >= HttpStatusCode.InternalServerError;
}
