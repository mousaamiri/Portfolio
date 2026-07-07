var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Typed client over the public Portfolio.API read endpoints.
var apiBaseUrl = builder.Configuration["PortfolioApi:BaseUrl"] ?? "https://localhost:7003";
builder.Services.AddHttpClient<Portfolio.Web.Services.Api.IPortfolioApiClient, Portfolio.Web.Services.Api.PortfolioApiClient>(
    client => client.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program;
