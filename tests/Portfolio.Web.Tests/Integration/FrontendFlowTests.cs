using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Web.Features;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Integration;

/// <summary>
/// End-to-end checks over the real HTTP pipeline (routing, middleware, Razor
/// compilation, the SEO endpoints) with the public API stubbed at the DI level.
/// These verify what the unit tests can't reach: that the flag actually flows all
/// the way into the rendered HTML / sitemap.xml / robots.txt that browsers and
/// crawlers receive, in both the Blog-enabled and Blog-disabled states.
/// </summary>
public abstract class FrontendFlowTestsBase : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    protected FrontendFlowTestsBase(WebApplicationFactory<Program> factory, bool blogEnabled)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Point the API client at a dead port so no outbound calls are made;
            // the stub below replaces it entirely before any request runs.
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Features:BlogEnabled"] = blogEnabled.ToString(),
                    ["PortfolioApi:BaseUrl"] = "http://localhost:1"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Swap the real typed client for a stub returning deterministic,
                // non-empty data so every public page renders without a live API.
                // Typed clients register as IPortfolioApiClient (implementation type
                // is null in the descriptor), so match on the service type alone.
                var apiDescriptor = services.Single(d => d.ServiceType == typeof(IPortfolioApiClient));
                services.Remove(apiDescriptor);
                services.AddSingleton<IPortfolioApiClient, StubPortfolioApiClient>();
            });
        });
    }

    protected HttpClient Client => _factory.CreateClient();

    protected static async Task<string> BodyAsync(HttpResponseMessage response)
        => await response.Content.ReadAsStringAsync();

    protected static async Task<string> GetAsync(HttpClient client, string uri)
    {
        var response = await client.GetAsync(uri);
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"GET {uri} should render");
        return await BodyAsync(response);
    }

    /// <summary>Stub API client: every endpoint returns one fixed item so the
    /// public pages have real content to render (no live API needed).</summary>
    private sealed class StubPortfolioApiClient : IPortfolioApiClient
    {
        private static readonly ProfileApiDto Profile = new()
        {
            FullName = "Mousa Amiri", JobTitle = "Backend Engineer", Bio = "bio"
        };

        public Task<IReadOnlyList<ProjectApiDto>> GetProjectsAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ProjectApiDto>>([new ProjectApiDto { Id = Guid.NewGuid(), Title = "Vitastic", ThumbnailUrl = "p.png" }]);

        public Task<IReadOnlyList<SkillApiDto>> GetSkillsAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<SkillApiDto>>([new SkillApiDto { Id = Guid.NewGuid(), Name = "C#", IconUrl = "csharp.svg" }]);

        public Task<IReadOnlyList<ExperienceApiDto>> GetExperiencesAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ExperienceApiDto>>([new ExperienceApiDto { Id = Guid.NewGuid(), CompanyName = "Acme", JobTitle = "Engineer" }]);

        public Task<IReadOnlyList<EducationApiDto>> GetEducationsAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<EducationApiDto>>([new EducationApiDto { Id = Guid.NewGuid(), InstitutionName = "MIT", Degree = "BSc" }]);

        public Task<IReadOnlyList<ArticleApiDto>> GetArticlesAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ArticleApiDto>>([new ArticleApiDto
            {
                Slug = "clean-architecture", Title = "Clean Architecture", Excerpt = "x",
                Category = ".NET", PublishDate = new DateTime(2026, 5, 26), ReadTimeMinutes = 3,
                Body = "<p>body</p>"
            }]);

        public Task<ProfileApiDto?> GetProfileAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<ProfileApiDto?>(Profile);

        public Task<IReadOnlyList<FaqApiDto>> GetFaqsAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<FaqApiDto>>([new FaqApiDto { Id = Guid.NewGuid(), Question = "Q", Answer = "A" }]);

        public Task<IReadOnlyList<TimelineEntryApiDto>> GetTimelineAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TimelineEntryApiDto>>([new TimelineEntryApiDto { Id = Guid.NewGuid(), Title = "T" }]);

        public Task<IReadOnlyList<InterestApiDto>> GetInterestsAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InterestApiDto>>([new InterestApiDto { Id = Guid.NewGuid(), Label = "Reading" }]);

        public Task<IReadOnlyList<StatCounterApiDto>> GetStatsAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<StatCounterApiDto>>([new StatCounterApiDto { Id = Guid.NewGuid(), CountTarget = 10 }]);

        public Task<IReadOnlyList<ImpactMetricApiDto>> GetImpactMetricsAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ImpactMetricApiDto>>([new ImpactMetricApiDto { Id = Guid.NewGuid(), Value = "99.9%" }]);

        public Task<IReadOnlyList<PrincipleApiDto>> GetPrinciplesAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PrincipleApiDto>>([new PrincipleApiDto { Id = Guid.NewGuid(), Title = "P", Description = "D" }]);

        public Task<IReadOnlyList<ProficiencyGroupApiDto>> GetProficienciesAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ProficiencyGroupApiDto>>([new ProficiencyGroupApiDto { Id = Guid.NewGuid(), Title = "Backend" }]);

        public Task<IReadOnlyList<TestimonialApiDto>> GetTestimonialsAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TestimonialApiDto>>([new TestimonialApiDto { Id = Guid.NewGuid(), Initials = "MA", Quote = "Q" }]);

        public Task<bool> SendMessageAsync(ContactMessageRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<IReadOnlyDictionary<string, string>> GetUiTranslationsAsync(string lang, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyDictionary<string, string>>(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
    }
}

/// <summary>Frontend flows with the Blog feature ENABLED (the historical default).</summary>
public class FrontendFlowsBlogEnabledTests(WebApplicationFactory<Program> factory)
    : FrontendFlowTestsBase(factory, blogEnabled: true)
{
    [Theory]
    [InlineData("/", "home")]
    [InlineData("/About", "about")]
    [InlineData("/Experience", "experience")]
    [InlineData("/Work", "work")]
    [InlineData("/Blog", "blog")]
    [InlineData("/Contact", "contact")]
    public async Task PublicPages_RenderSuccessfully(string uri, string expectedActivePage)
    {
        var html = await GetAsync(Client, uri);

        html.Should().Contain($"currentPage: '{expectedActivePage}'");
        // The flag reaches the client-built chrome.
        html.Should().Contain("blogEnabled: true");
    }

    [Fact]
    public async Task BlogPage_RendersAndKeepsWorking()
    {
        var html = await GetAsync(Client, "/Blog");

        html.Should().Contain("clean-architecture");
    }

    [Fact]
    public async Task Sitemap_IncludesTheBlog()
    {
        var xml = await GetAsync(Client, "/sitemap.xml");

        xml.Should().Contain("<loc>http://localhost/Blog</loc>");
    }

    [Fact]
    public async Task Robots_AllowsTheBlog()
    {
        var text = await GetAsync(Client, "/robots.txt");

        text.Should().NotContain("Disallow: /Blog");
        text.Should().Contain("Disallow: /Admin");
        text.Should().Contain("Sitemap: http://localhost/sitemap.xml");
    }
}

/// <summary>Frontend flows with the Blog feature DISABLED (the new default).</summary>
public class FrontendFlowsBlogDisabledTests(WebApplicationFactory<Program> factory)
    : FrontendFlowTestsBase(factory, blogEnabled: false)
{
    [Theory]
    [InlineData("/", "home")]
    [InlineData("/About", "about")]
    [InlineData("/Experience", "experience")]
    [InlineData("/Work", "work")]
    [InlineData("/Contact", "contact")]
    public async Task PublicPages_RenderSuccessfully_WithoutTheBlogFlag(string uri, string expectedActivePage)
    {
        var html = await GetAsync(Client, uri);

        html.Should().Contain($"currentPage: '{expectedActivePage}'");
        // Client chrome reads this and drops the blog link.
        html.Should().Contain("blogEnabled: false");
    }

    [Fact]
    public async Task BlogPage_StillRendersForAnyoneWithTheUrl()
    {
        // The page is de-linked and de-indexed, not deleted — it must keep working
        // so re-enabling is a pure config flip.
        var html = await GetAsync(Client, "/Blog");

        html.Should().Contain("clean-architecture");
    }

    [Fact]
    public async Task Sitemap_ExcludesTheBlogButKeepsEveryOtherPage()
    {
        var xml = await GetAsync(Client, "/sitemap.xml");

        xml.Should().NotContain("/Blog</loc>");
        foreach (var path in new[] { "/", "/Work", "/About", "/Experience", "/Contact" })
            xml.Should().Contain($"<loc>http://localhost{path}</loc>");
    }

    [Fact]
    public async Task Robots_DisallowsTheBlogAndStillsReferencesTheSitemap()
    {
        var text = await GetAsync(Client, "/robots.txt");

        text.Should().Contain("Disallow: /Blog");
        text.Should().Contain("Disallow: /Admin");
        text.Should().Contain("Sitemap: http://localhost/sitemap.xml");
    }

    [Fact]
    public async Task RobotsTxt_IsServedAsDynamicContent_NotA404()
    {
        var response = await Client.GetAsync("/robots.txt");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/plain");
    }

    [Theory]
    [InlineData("/About")]
    [InlineData("/Experience")]
    [InlineData("/Work")]
    [InlineData("/Contact")]
    public async Task ClosingCta_NoLongerLinksToTheBlog(string uri)
    {
        var html = await GetAsync(Client, uri);

        // The server-rendered "Read Article" card is dropped alongside the flag.
        // (The blog route string can still appear inside __navConfig — the client
        // chrome only renders a link for it when the flag is on — so assert on the
        // CTA markup itself, which is what visitors actually see.)
        html.Should().NotContain("cta.read_article", "the Read Article card must be hidden");
        html.Should().NotContain("Read Article");
        html.Should().NotContain("asp-controller=\"Blog\"")
            .And.NotContain("href=\"/Blog\"", "no clickable link to the blog may remain");
    }

    [Fact]
    public async Task HomePage_StillLinksToAllRemainingSections()
    {
        var html = await GetAsync(Client, "/");

        // Nothing else regressed: the other routes are still emitted for the chrome.
        html.Should().Contain("about:");
        html.Should().Contain("experience:");
        html.Should().Contain("work:");
        html.Should().Contain("contact:");
    }
}
