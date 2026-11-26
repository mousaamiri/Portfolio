using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Portfolio.Web.Endpoints;
using Portfolio.Web.Features;

// Thin host for the React SPA (ClientApp -> `npm run build` -> wwwroot).
// It serves the built assets, forwards /api/* to Portfolio.API so the browser
// only ever talks to one origin, and exposes the SEO endpoints.
var builder = WebApplication.CreateBuilder(args);

// HTML/CSS/JS are highly compressible; the SPA bundle shrinks 60-80%.
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// Trust X-Forwarded-* from a reverse proxy so the API sees the visitor's real IP.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// Optional public sections (currently the Blog). A disabled section is dropped
// from robots.txt and sitemap.xml.
builder.Services.Configure<FeatureFlags>(builder.Configuration.GetSection("Features"));

var configuredBaseUrl = builder.Configuration["PortfolioApi:BaseUrl"];
var baseUrlMissing = string.IsNullOrWhiteSpace(configuredBaseUrl);
// In Development prefer the API's plain-HTTP port: it avoids the self-signed
// dev certificate, which otherwise makes every proxied /api call fail.
var apiBaseUrl = baseUrlMissing
    ? (builder.Environment.IsDevelopment() ? "http://localhost:5175" : "https://localhost:7003")
    : configuredBaseUrl!;

// Same-origin proxy used by the React SPA: the browser calls /api/* on this host
// and the Web server forwards to Portfolio.API. That removes browser CORS and
// dev-certificate issues and keeps the SPA working on any host/port.
builder.Services.AddHttpClient("api-proxy", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(100);
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
    app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
    {
        if (context.Response.HasStarted)
            return;

        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var isUpstreamDown = feature?.Error is HttpRequestException or TaskCanceledException;

        context.Response.Clear();
        context.Response.StatusCode = isUpstreamDown
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(isUpstreamDown
            ? "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"><title>Service unavailable</title></head>" +
              "<body><h1>Service temporarily unavailable</h1><p>The server cannot be reached right now. Please try again shortly.</p></body></html>"
            : "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"><title>Error</title></head>" +
              "<body><h1>Something went wrong</h1><p>An unexpected error occurred. Please try again.</p></body></html>");
    }));
    app.UseHsts();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();

// Before UseStaticFiles so served files are compressed.
app.UseResponseCompression();

// Forward /api/* to Portfolio.API so the SPA can use same-origin requests.
app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/api"))
    {
        await next();
        return;
    }

    var client = context.RequestServices
        .GetRequiredService<IHttpClientFactory>()
        .CreateClient("api-proxy");

    using var upstream = new HttpRequestMessage(
        new HttpMethod(context.Request.Method),
        context.Request.Path.Value + context.Request.QueryString.Value);

    if (context.Request.ContentLength is > 0
        || context.Request.Headers.ContainsKey("Transfer-Encoding"))
    {
        upstream.Content = new StreamContent(context.Request.Body);
    }

    foreach (var header in context.Request.Headers)
    {
        if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase)
            || header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)
            || header.Key.Equals("Accept-Encoding", StringComparison.OrdinalIgnoreCase)
            || header.Key.Equals("Connection", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        if (!upstream.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
        {
            upstream.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }

    // The API's "messages" limiter partitions by client IP; keep the visitor's.
    var remoteIp = context.Connection.RemoteIpAddress?.ToString();
    if (!string.IsNullOrEmpty(remoteIp))
    {
        upstream.Headers.Remove("X-Forwarded-For");
        upstream.Headers.TryAddWithoutValidation("X-Forwarded-For", remoteIp);
        upstream.Headers.TryAddWithoutValidation("X-Forwarded-Proto", context.Request.Scheme);
    }

    try
    {
        using var response = await client.SendAsync(
            upstream, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);

        context.Response.StatusCode = (int)response.StatusCode;

        foreach (var header in response.Headers.Concat(response.Content.Headers))
        {
            if (header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase)
                || header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
    }
    catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
    {
        if (context.Response.HasStarted) throw;
        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status502BadGateway;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsync(
            "{\"success\":false,\"message\":\"Portfolio.API is unreachable.\"}");
    }
});

// Serve index.html for every HTML navigation that is not an API/utility route so
// client-side routing works on deep links, refreshes and unknown paths (the SPA
// then renders its own 404 page). Static assets (/app, /images, /fonts, /resumes)
// and the SEO/health endpoints fall through to the middleware/endpoints below.
var spaExcludedPrefixes = new[] { "/api", "/health", "/robots.txt", "/sitemap.xml" };
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var accept = context.Request.Headers.Accept.ToString();
    var wantsHtml = string.IsNullOrEmpty(accept) || accept.Contains("text/html", StringComparison.OrdinalIgnoreCase);
    var isExcluded = spaExcludedPrefixes.Any(prefix =>
        path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase));

    if (HttpMethods.IsGet(context.Request.Method)
        && wantsHtml
        && !isExcluded
        && !Path.HasExtension(path.Value))
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.Headers.CacheControl = "no-cache";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
        return;
    }

    await next();
});

// Vite emits content-hashed filenames under /app, so those can be cached
// immutably; the remaining assets (images, fonts, resumes) keep a short TTL.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var isHashed = ctx.Context.Request.Path.StartsWithSegments("/app", StringComparison.OrdinalIgnoreCase);
        ctx.Context.Response.Headers.CacheControl = isHashed
            ? "public, max-age=31536000, immutable"
            : "public, max-age=3600";
    }
});

app.MapSeoEndpoints();

app.Run();

public partial class Program;
