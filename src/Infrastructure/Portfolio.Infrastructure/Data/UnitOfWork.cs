using Portfolio.Application.Interfaces;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IEducationRepository? _educations;
    private IExperienceRepository? _experiences;
    private IProjectRepository? _projects;
    private ISkillRepository? _skills;
    private IArticleRepository? _articles;
    private IMessageRepository? _messages;
    private IProfileRepository? _profiles;
    private IFaqRepository? _faqs;
    private ITimelineEntryRepository? _timelineEntries;
    private IInterestRepository? _interests;
    private IStatCounterRepository? _statCounters;
    private IImpactMetricRepository? _impactMetrics;
    private IPrincipleRepository? _principles;
    private IProficiencyGroupRepository? _proficiencyGroups;
    private ITestimonialRepository? _testimonials;

    public IEducationRepository Educations => _educations ??= new EducationRepository(context);
    public IExperienceRepository Experiences => _experiences ??= new ExperienceRepository(context);
    public IProjectRepository Projects => _projects ??= new ProjectRepository(context);
    public ISkillRepository Skills => _skills ??= new SkillRepository(context);
    public IArticleRepository Articles => _articles ??= new ArticleRepository(context);
    public IMessageRepository Messages => _messages ??= new MessageRepository(context);
    public IProfileRepository Profiles => _profiles ??= new ProfileRepository(context);
    public IFaqRepository Faqs => _faqs ??= new FaqRepository(context);
    public ITimelineEntryRepository TimelineEntries => _timelineEntries ??= new TimelineEntryRepository(context);
    public IInterestRepository Interests => _interests ??= new InterestRepository(context);
    public IStatCounterRepository StatCounters => _statCounters ??= new StatCounterRepository(context);
    public IImpactMetricRepository ImpactMetrics => _impactMetrics ??= new ImpactMetricRepository(context);
    public IPrincipleRepository Principles => _principles ??= new PrincipleRepository(context);
    public IProficiencyGroupRepository ProficiencyGroups => _proficiencyGroups ??= new ProficiencyGroupRepository(context);
    public ITestimonialRepository Testimonials => _testimonials ??= new TestimonialRepository(context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}
