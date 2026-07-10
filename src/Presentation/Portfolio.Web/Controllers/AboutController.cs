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

        // Skills, Education, Journey, Interests, Footprint and Endorsements all come
        // from Portfolio.API. Only the hero stat *values* (role/experience/degree
        // badges) and portrait remain mock-backed. TODO: source those from Profile.
        var about = MockDataService.GetAboutViewModel();

        var skills = await api.GetSkillsAsync(language, cancellationToken);
        var educations = await api.GetEducationsAsync(language, cancellationToken);
        var journey = await api.GetTimelineAsync(language, cancellationToken);
        var interests = await api.GetInterestsAsync(language, cancellationToken);
        var footprint = await api.GetStatsAsync(language, cancellationToken);
        var endorsements = await api.GetTestimonialsAsync(language, cancellationToken);

        var model = new AboutViewModel
        {
            RoleValue = about.RoleValue,
            ExperienceValue = about.ExperienceValue,
            DegreeValue = about.DegreeValue,
            PortraitUrl = about.PortraitUrl,
            PortraitAlt = about.PortraitAlt,
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
