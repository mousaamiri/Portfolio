namespace Portfolio.Web.Models.ViewModels;

/// <summary>One FAQ accordion item on the Contact page (single-open).
/// English text in the view + data-i18n keys for client-side Farsi.</summary>
public class FaqItemViewModel
{
    public string QuestionKey { get; init; } = string.Empty;
    public string Question { get; init; } = string.Empty;
    public string AnswerKey { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
}
