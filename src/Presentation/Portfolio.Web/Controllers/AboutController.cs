using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class AboutController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.ResolveFromRequest(HttpContext, lang);

        // Everything comes from Portfolio.API (database-backed). Hero stat badges and
        // portrait now live on the Profile entity; lists come from their endpoints.
        var profile = await api.GetProfileAsync(language, cancellationToken);

        var skills = await api.GetSkillsAsync(language, cancellationToken);
        var educations = await api.GetEducationsAsync(language, cancellationToken);
        var journey = await api.GetTimelineAsync(language, cancellationToken);
        var interests = await api.GetInterestsAsync(language, cancellationToken);
        var footprint = await api.GetStatsAsync(language, cancellationToken);
        var endorsements = await api.GetTestimonialsAsync(language, cancellationToken);

        var model = new AboutViewModel
        {
            RoleValue = profile?.RoleBadge ?? string.Empty,
            ExperienceValue = profile?.ExperienceBadge ?? string.Empty,
            DegreeValue = profile?.DegreeBadge ?? string.Empty,
            PortraitUrl = string.IsNullOrWhiteSpace(profile?.PortraitUrl) ? "/images/about-portrait.jpg" : profile!.PortraitUrl!,
            PortraitAlt = profile?.PortraitAlt ?? string.Empty,
            Journey = journey.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Footprint = footprint.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Interests = interests.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Endorsements = endorsements.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Skills = skills.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Education = educations.Select(ApiViewModelMapper.ToViewModel).ToList()
        };

        return View(model);
    }
}
