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

        // Skills + Education come from Portfolio.API. The remaining About sections
        // (journey, footprint stats, interests, endorsements, hero stat values)
        // have no backend entity yet and stay mocked. TODO(E5-E9): wire these once
        // their entities exist.
        var about = MockDataService.GetAboutViewModel();

        var skills = await api.GetSkillsAsync(language, cancellationToken);
        var educations = await api.GetEducationsAsync(language, cancellationToken);

        var model = new AboutViewModel
        {
            RoleValue = about.RoleValue,
            ExperienceValue = about.ExperienceValue,
            DegreeValue = about.DegreeValue,
            PortraitUrl = about.PortraitUrl,
            PortraitAlt = about.PortraitAlt,
            Journey = about.Journey,
            Footprint = about.Footprint,
            Interests = about.Interests,
            Endorsements = about.Endorsements,
            Skills = skills.Select(ApiViewModelMapper.ToViewModel).ToList(),
            Education = educations.Select(ApiViewModelMapper.ToViewModel).ToList()
        };

        return View(model);
    }
}
