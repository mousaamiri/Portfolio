namespace Portfolio.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEducationRepository Educations { get; }
    IExperienceRepository Experiences { get; }
    IProjectRepository Projects { get; }
    ISkillRepository Skills { get; }
    IArticleRepository Articles { get; }
    IMessageRepository Messages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
