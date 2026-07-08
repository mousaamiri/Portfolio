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

    public IEducationRepository Educations => _educations ??= new EducationRepository(context);
    public IExperienceRepository Experiences => _experiences ??= new ExperienceRepository(context);
    public IProjectRepository Projects => _projects ??= new ProjectRepository(context);
    public ISkillRepository Skills => _skills ??= new SkillRepository(context);
    public IArticleRepository Articles => _articles ??= new ArticleRepository(context);
    public IMessageRepository Messages => _messages ??= new MessageRepository(context);
    public IProfileRepository Profiles => _profiles ??= new ProfileRepository(context);
    public IFaqRepository Faqs => _faqs ??= new FaqRepository(context);

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
