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

        return services;
    }
}