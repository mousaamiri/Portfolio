using Microsoft.AspNetCore.Authentication.Cookies;
using Portfolio.Web.Services.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Typed clients over Portfolio.API. The public client hits anonymous read
// endpoints; the admin client hits the authenticated api/admin/* endpoints.
var apiBaseUrl = builder.Configuration["PortfolioApi:BaseUrl"] ?? "https://localhost:7003";
builder.Services.AddHttpClient<IPortfolioApiClient, PortfolioApiClient>(
    client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>(
    client => client.BaseAddress = new Uri(apiBaseUrl));

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

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program;
