using Microsoft.AspNetCore.Mvc;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Controllers;

public class AboutController(IPortfolioApiClient api) : Controller
{
    public async Task<IActionResult> Index(string? lang, CancellationToken cancellationToken)
    {
        var language = WebLanguage.Resolve(lang);

        // Skills + Education + Journey + Interests + Footprint come from Portfolio.API.
        // The remaining About sections (endorsements, hero stat values) have no backend
        // entity yet and stay mocked. TODO(E9+): wire the rest.
        var about = MockDataService.GetAboutViewModel();

        var skills = await api.GetSkillsAsync(language, cancellationToken);
        var educations = await api.GetEducationsAsync(language, cancellationToken);
        var journey = await api.GetTimelineAsync(language, cancellationToken);
        var interests = await api.GetInterestsAsync(language, cancellationToken);
        var footprint = await api.GetStatsAsync(language, cancellationToken);

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
            Endorsements = about.Endorsements,
            Skills = skills.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Education = educations.Select(ApiViewModelMapper.ToViewModel).ToList()
        };

        return View(model);
    }
}
