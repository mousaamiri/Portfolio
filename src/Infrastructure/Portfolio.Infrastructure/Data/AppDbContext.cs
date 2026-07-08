using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Admins;
using Portfolio.Domain.Entities.Articles;
using Portfolio.Domain.Entities.Educations;
using Portfolio.Domain.Entities.Experiences;
using Portfolio.Domain.Entities.Faqs;
using Portfolio.Domain.Entities.ImpactMetrics;
using Portfolio.Domain.Entities.Interests;
using Portfolio.Domain.Entities.Messages;
using Portfolio.Domain.Entities.Principles;
using Portfolio.Domain.Entities.Proficiencies;
using Portfolio.Domain.Entities.Profiles;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Domain.Entities.Skills;
using Portfolio.Domain.Entities.Stats;
using Portfolio.Domain.Entities.Timeline;

namespace Portfolio.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Admin> Admins => Set<Admin>();

    public DbSet<Education> Educations => Set<Education>();
    public DbSet<EducationTranslation> EducationTranslations => Set<EducationTranslation>();

    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<ExperienceTranslation> ExperienceTranslations => Set<ExperienceTranslation>();

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTranslation> ProjectTranslations => Set<ProjectTranslation>();

    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<SkillTranslation> SkillTranslations => Set<SkillTranslation>();

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleTranslation> ArticleTranslations => Set<ArticleTranslation>();

    public DbSet<Message> Messages => Set<Message>();

    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<ProfileTranslation> ProfileTranslations => Set<ProfileTranslation>();

    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<FaqTranslation> FaqTranslations => Set<FaqTranslation>();

    public DbSet<TimelineEntry> TimelineEntries => Set<TimelineEntry>();
    public DbSet<TimelineEntryTranslation> TimelineEntryTranslations => Set<TimelineEntryTranslation>();

    public DbSet<Interest> Interests => Set<Interest>();
    public DbSet<InterestTranslation> InterestTranslations => Set<InterestTranslation>();

    public DbSet<StatCounter> StatCounters => Set<StatCounter>();
    public DbSet<StatCounterTranslation> StatCounterTranslations => Set<StatCounterTranslation>();

    public DbSet<ImpactMetric> ImpactMetrics => Set<ImpactMetric>();
    public DbSet<ImpactMetricTranslation> ImpactMetricTranslations => Set<ImpactMetricTranslation>();

    public DbSet<Principle> Principles => Set<Principle>();
    public DbSet<PrincipleTranslation> PrincipleTranslations => Set<PrincipleTranslation>();

    public DbSet<ProficiencyGroup> ProficiencyGroups => Set<ProficiencyGroup>();
    public DbSet<ProficiencyGroupTranslation> ProficiencyGroupTranslations => Set<ProficiencyGroupTranslation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        modelBuilder.Entity<Article>()
            .HasIndex(a => a.Slug)
            .IsUnique();
    }
}
