using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Testimonials;
using Portfolio.Application.Extensions;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Testimonials;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class TestimonialService(IUnitOfWork unitOfWork) : ITestimonialService
{
    public async Task<Result<IReadOnlyList<TestimonialDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var items = await unitOfWork.Testimonials.GetAllWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<TestimonialDto>>.Success(items.Select(x => MapToDto(x, language)).ToList());
    }

    public async Task<Result<IReadOnlyList<TestimonialDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var items = await unitOfWork.Testimonials.GetActiveWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<TestimonialDto>>.Success(items.Select(x => MapToDto(x, language)).ToList());
    }

    public async Task<Result<TestimonialDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.Testimonials.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (item is null)
            return Result<TestimonialDto>.Failure($"Testimonial with id '{id}' was not found.");
        return Result<TestimonialDto>.Success(MapToDto(item, ParseLanguage(languageCode)));
    }

    public async Task<Result<Guid>> CreateAsync(CreateTestimonialRequest request, CancellationToken cancellationToken = default)
    {
        var item = new Testimonial
        {
            Id = Guid.NewGuid(),
            Initials = request.Initials,
            AvatarColor = request.AvatarColor,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        foreach (var t in request.Translations)
            item.Translations.Add(BuildTranslation(item.Id, t));

        await unitOfWork.Testimonials.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateTestimonialRequest request, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.Testimonials.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (item is null)
            return Result<bool>.Failure($"Testimonial with id '{id}' was not found.");

        item.Initials = request.Initials;
        item.AvatarColor = request.AvatarColor;
        item.DisplayOrder = request.DisplayOrder;
        item.IsActive = request.IsActive;
        item.UpdatedAt = DateTime.UtcNow;

        item.Translations.SyncTranslations(
            request.Translations.Select(t => BuildTranslation(item.Id, t)).ToList(),
            (existing, incoming) =>
            {
                existing.Quote = incoming.Quote;
                existing.Name = incoming.Name;
                existing.Role = incoming.Role;
            });

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.Testimonials.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return Result<bool>.Failure($"Testimonial with id '{id}' was not found.");

        unitOfWork.Testimonials.Delete(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static TestimonialTranslation BuildTranslation(Guid testimonialId, TestimonialTranslationRequest t) => new()
    {
        Id = Guid.NewGuid(),
        TestimonialId = testimonialId,
        Language = ParseLanguage(t.LanguageCode),
        Quote = t.Quote,
        Name = t.Name,
        Role = t.Role
    };

    private static Language ParseLanguage(string languageCode)
        => Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language) ? language : Language.En;

    private static TestimonialDto MapToDto(Testimonial item, Language language)
    {
        var t = item.Translations.FirstOrDefault(x => x.Language == language);
        return new TestimonialDto
        {
            Id = item.Id,
            Initials = item.Initials,
            AvatarColor = item.AvatarColor,
            DisplayOrder = item.DisplayOrder,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt,
            Quote = t?.Quote ?? string.Empty,
            Name = t?.Name ?? string.Empty,
            Role = t?.Role ?? string.Empty
        };
    }
}
