using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Portfolio.Web.Localization;
using Portfolio.Web.Services;

namespace Portfolio.Web.Tests.Services;

public class WebLanguageTests
{
    private static HttpContext ContextWith(string? cookie = null, string? acceptLanguage = null)
    {
        var context = new DefaultHttpContext();
        if (cookie is not null)
            context.Request.Headers.Cookie = $"{WebLanguage.CookieName}={cookie}";
        if (acceptLanguage is not null)
            context.Request.Headers.AcceptLanguage = acceptLanguage;
        return context;
    }

    [Fact]
    public void ResolveFromRequest_QueryLang_WinsOverEverything()
    {
        var context = ContextWith(cookie: "en", acceptLanguage: "en-US");

        WebLanguage.ResolveFromRequest(context, "fa").Should().Be("fa");
    }

    [Fact]
    public void ResolveFromRequest_Cookie_WinsOverAcceptLanguage()
    {
        var context = ContextWith(cookie: "fa", acceptLanguage: "en-US,en;q=0.9");

        WebLanguage.ResolveFromRequest(context).Should().Be("fa");
    }

    [Fact]
    public void ResolveFromRequest_NoCookie_DetectsFromAcceptLanguage()
    {
        var context = ContextWith(acceptLanguage: "fa-IR,fa;q=0.9,en;q=0.8");

        WebLanguage.ResolveFromRequest(context).Should().Be("fa");
    }

    [Fact]
    public void ResolveFromRequest_AcceptLanguage_PicksFirstSupportedInOrder()
    {
        // Portuguese unsupported → falls through to the next supported tag (ar).
        var context = ContextWith(acceptLanguage: "pt-BR,ar;q=0.9,en;q=0.5");

        WebLanguage.ResolveFromRequest(context).Should().Be("ar");
    }

    [Fact]
    public void ResolveFromRequest_UnsupportedAcceptLanguage_FallsBackToDefault()
    {
        var context = ContextWith(acceptLanguage: "de-DE,de;q=0.9");

        WebLanguage.ResolveFromRequest(context).Should().Be(WebLanguage.Default);
    }

    [Fact]
    public void ResolveFromRequest_NoCookieNoHeader_ReturnsDefault()
    {
        WebLanguage.ResolveFromRequest(ContextWith()).Should().Be("en");
    }

    [Theory]
    [InlineData("fa", true)]
    [InlineData("ar", true)]
    [InlineData("en", false)]
    public void LocalizationState_IsRtl_ForRightToLeftLanguages(string language, bool expectedRtl)
    {
        var state = new LocalizationState { Language = language };

        state.IsRtl.Should().Be(expectedRtl);
        state.Dir.Should().Be(expectedRtl ? "rtl" : "ltr");
    }
}
