namespace Portfolio.API.Common;

public static class LanguageResolver
{
    private const string DefaultLanguage = "en";

    public static string Resolve(string? queryLang, HttpRequest request)
    {
        if (!string.IsNullOrWhiteSpace(queryLang))
            return queryLang.Trim().ToLowerInvariant();

        var acceptLanguage = request.Headers.AcceptLanguage.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(acceptLanguage))
        {
            var primary = acceptLanguage.Split(',', ';')[0].Trim().ToLowerInvariant();
            if (primary.Length >= 2)
                return primary[..2];
        }

        return DefaultLanguage;
    }
}
