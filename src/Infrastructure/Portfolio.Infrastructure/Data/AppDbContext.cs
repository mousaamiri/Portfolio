using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Admins;
using Portfolio.Domain.Entities.Educations;
using Portfolio.Domain.Entities.Experiences;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Domain.Entities.Skills;

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
}
