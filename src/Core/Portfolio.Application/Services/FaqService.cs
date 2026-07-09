using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Faqs;
using Portfolio.Application.Extensions;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Faqs;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class FaqService(IUnitOfWork unitOfWork) : IFaqService
{
    public async Task<Result<IReadOnlyList<FaqDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var faqs = await unitOfWork.Faqs.GetAllWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<FaqDto>>.Success(faqs.Select(f => MapToDto(f, language)).ToList());
    }

    public async Task<Result<IReadOnlyList<FaqDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var faqs = await unitOfWork.Faqs.GetActiveWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<FaqDto>>.Success(faqs.Select(f => MapToDto(f, language)).ToList());
    }

    public async Task<Result<FaqDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var faq = await unitOfWork.Faqs.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (faq is null)
            return Result<FaqDto>.Failure($"Faq with id '{id}' was not found.");

        return Result<FaqDto>.Success(MapToDto(faq, ParseLanguage(languageCode)));
    }

    public async Task<Result<Guid>> CreateAsync(CreateFaqRequest request, CancellationToken cancellationToken = default)
    {
        var faq = new Faq
        {
            Id = Guid.NewGuid(),
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var t in request.Translations)
            faq.Translations.Add(BuildTranslation(faq.Id, t));

        await unitOfWork.Faqs.AddAsync(faq, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(faq.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateFaqRequest request, CancellationToken cancellationToken = default)
    {
        var faq = await unitOfWork.Faqs.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (faq is null)
            return Result<bool>.Failure($"Faq with id '{id}' was not found.");

        faq.DisplayOrder = request.DisplayOrder;
        faq.IsActive = request.IsActive;
        faq.UpdatedAt = DateTime.UtcNow;

        faq.Translations.SyncTranslations(
            request.Translations.Select(t => BuildTranslation(faq.Id, t)).ToList(),
            (existing, incoming) =>
            {
                existing.Question = incoming.Question;
                existing.Answer = incoming.Answer;
            });

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var faq = await unitOfWork.Faqs.GetByIdAsync(id, cancellationToken);
        if (faq is null)
            return Result<bool>.Failure($"Faq with id '{id}' was not found.");

        unitOfWork.Faqs.Delete(faq);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static FaqTranslation BuildTranslation(Guid faqId, FaqTranslationRequest t) => new()
    {
        Id = Guid.NewGuid(),
        FaqId = faqId,
        Language = ParseLanguage(t.LanguageCode),
        Question = t.Question,
        Answer = t.Answer
    };

    private static Language ParseLanguage(string languageCode)
        => Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language) ? language : Language.En;

    private static FaqDto MapToDto(Faq faq, Language language)
    {
        var t = faq.Translations.FirstOrDefault(x => x.Language == language);
        return new FaqDto
        {
            Id = faq.Id,
            DisplayOrder = faq.DisplayOrder,
            IsActive = faq.IsActive,
            CreatedAt = faq.CreatedAt,
            Question = t?.Question ?? string.Empty,
            Answer = t?.Answer ?? string.Empty
        };
    }
}
