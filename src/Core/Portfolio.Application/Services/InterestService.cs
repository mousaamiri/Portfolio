using Portfolio.Application.Common;
using Portfolio.Application.DTOs.Interests;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities.Interests;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services;

public class InterestService(IUnitOfWork unitOfWork) : IInterestService
{
    public async Task<Result<IReadOnlyList<InterestDto>>> GetAllAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var interests = await unitOfWork.Interests.GetAllWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<InterestDto>>.Success(interests.Select(i => MapToDto(i, language)).ToList());
    }

    public async Task<Result<IReadOnlyList<InterestDto>>> GetPublicAsync(string languageCode, CancellationToken cancellationToken = default)
    {
        var language = ParseLanguage(languageCode);
        var interests = await unitOfWork.Interests.GetActiveWithTranslationsAsync(cancellationToken);
        return Result<IReadOnlyList<InterestDto>>.Success(interests.Select(i => MapToDto(i, language)).ToList());
    }

    public async Task<Result<InterestDto>> GetByIdAsync(Guid id, string languageCode, CancellationToken cancellationToken = default)
    {
        var interest = await unitOfWork.Interests.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (interest is null)
            return Result<InterestDto>.Failure($"Interest with id '{id}' was not found.");

        return Result<InterestDto>.Success(MapToDto(interest, ParseLanguage(languageCode)));
    }

    public async Task<Result<Guid>> CreateAsync(CreateInterestRequest request, CancellationToken cancellationToken = default)
    {
        var interest = new Interest
        {
            Id = Guid.NewGuid(),
            Icon = request.Icon,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var t in request.Translations)
            interest.Translations.Add(BuildTranslation(interest.Id, t));

        await unitOfWork.Interests.AddAsync(interest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(interest.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateInterestRequest request, CancellationToken cancellationToken = default)
    {
        var interest = await unitOfWork.Interests.GetByIdWithTranslationsAsync(id, cancellationToken);
        if (interest is null)
            return Result<bool>.Failure($"Interest with id '{id}' was not found.");

        interest.Icon = request.Icon;
        interest.DisplayOrder = request.DisplayOrder;
        interest.IsActive = request.IsActive;
        interest.UpdatedAt = DateTime.UtcNow;

        interest.Translations.Clear();
        foreach (var t in request.Translations)
            interest.Translations.Add(BuildTranslation(interest.Id, t));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var interest = await unitOfWork.Interests.GetByIdAsync(id, cancellationToken);
        if (interest is null)
            return Result<bool>.Failure($"Interest with id '{id}' was not found.");

        unitOfWork.Interests.Delete(interest);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    private static InterestTranslation BuildTranslation(Guid interestId, InterestTranslationRequest t) => new()
    {
        Id = Guid.NewGuid(),
        InterestId = interestId,
        Language = ParseLanguage(t.LanguageCode),
        Label = t.Label
    };

    private static Language ParseLanguage(string languageCode)
        => Enum.TryParse<Language>(languageCode, ignoreCase: true, out var language) ? language : Language.En;

    private static InterestDto MapToDto(Interest interest, Language language)
    {
        var t = interest.Translations.FirstOrDefault(x => x.Language == language);
        return new InterestDto
        {
            Id = interest.Id,
            Icon = interest.Icon,
            DisplayOrder = interest.DisplayOrder,
            IsActive = interest.IsActive,
            CreatedAt = interest.CreatedAt,
            Label = t?.Label ?? string.Empty
        };
    }
}
