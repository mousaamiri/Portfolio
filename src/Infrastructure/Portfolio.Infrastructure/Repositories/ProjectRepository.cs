using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class ProjectRepository(AppDbContext context) : Repository<Project>(context), IProjectRepository;
