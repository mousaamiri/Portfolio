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
        // All calls are independent, so they run concurrently (one round-trip, not seven).
        var profileTask = api.GetProfileAsync(language, cancellationToken);
        var skillsTask = api.GetSkillsAsync(language, cancellationToken);
        var educationsTask = api.GetEducationsAsync(language, cancellationToken);
        var journeyTask = api.GetTimelineAsync(language, cancellationToken);
        var interestsTask = api.GetInterestsAsync(language, cancellationToken);
        var footprintTask = api.GetStatsAsync(language, cancellationToken);
        var endorsementsTask = api.GetTestimonialsAsync(language, cancellationToken);
        await Task.WhenAll(profileTask, skillsTask, educationsTask, journeyTask, interestsTask, footprintTask, endorsementsTask);

        var profile = profileTask.Result;
        var skills = skillsTask.Result;
        var educations = educationsTask.Result;
        var journey = journeyTask.Result;
        var interests = interestsTask.Result;
        var footprint = footprintTask.Result;
        var endorsements = endorsementsTask.Result;

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
