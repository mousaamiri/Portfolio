using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Skills;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class SkillRepository(AppDbContext context) : Repository<Skill>(context), ISkillRepository;
