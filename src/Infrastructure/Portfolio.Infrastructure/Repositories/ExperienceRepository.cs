using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Experiences;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class ExperienceRepository(AppDbContext context) : Repository<Experience>(context), IExperienceRepository;
