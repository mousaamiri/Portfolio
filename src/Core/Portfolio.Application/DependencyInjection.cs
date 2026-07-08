using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Application.Services;

namespace Portfolio.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<IExperienceService, ExperienceService>();
        services.AddScoped<IEducationService, EducationService>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IFaqService, FaqService>();
        services.AddScoped<ITimelineEntryService, TimelineEntryService>();
        services.AddScoped<IInterestService, InterestService>();
        services.AddScoped<IStatCounterService, StatCounterService>();
        services.AddScoped<IImpactMetricService, ImpactMetricService>();
        services.AddScoped<IPrincipleService, PrincipleService>();
        services.AddScoped<IProficiencyGroupService, ProficiencyGroupService>();
        services.AddScoped<ITestimonialService, TestimonialService>();

        return services;
    }
}