using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities.Educations;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class EducationRepository(AppDbContext context) : Repository<Education>(context), IEducationRepository;
