namespace Portfolio.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEducationRepository Educations { get; }
    IExperienceRepository Experiences { get; }
    IProjectRepository Projects { get; }
    ISkillRepository Skills { get; }
    IArticleRepository Articles { get; }
    IMessageRepository Messages { get; }
    IProfileRepository Profiles { get; }
    IFaqRepository Faqs { get; }
    ITimelineEntryRepository TimelineEntries { get; }
    IInterestRepository Interests { get; }
    IStatCounterRepository StatCounters { get; }
    IImpactMetricRepository ImpactMetrics { get; }
    IPrincipleRepository Principles { get; }
    IProficiencyGroupRepository ProficiencyGroups { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
