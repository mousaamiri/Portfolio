namespace Portfolio.Web.Services.Api;

/// <summary>
/// Frontend-owned mapping of a technology name to its pill accent color.
/// Per the Phase 2 decision, tech-pill colors are a presentation concern of the
/// Web app, not modeled in the backend. Unknown names fall back to blue.
/// </summary>
public static class TechColor
{
    private const string Default = "#3b82f6";

    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        // amber
        ["Java"] = "#f0a63b", ["Java 21"] = "#f0a63b", ["Python"] = "#f0a63b", ["JWT"] = "#f0a63b",
        // green
        ["Spring Boot"] = "#22c55e", ["Spring AI"] = "#22c55e", ["Spring Security"] = "#22c55e",
        ["JWT Auth"] = "#22c55e", [".NET"] = "#22c55e", ["ASP.NET Core"] = "#22c55e",
        // teal
        ["Tailwind CSS"] = "#14b8a6",
        // pink
        ["Apache PDFBox"] = "#ec4899", ["Chart.js"] = "#ec4899", ["Redis"] = "#ec4899", ["TMDB API"] = "#ec4899",
        // purple
        ["pgvector"] = "#a855f7", ["WebSocket"] = "#a855f7", ["Pandas"] = "#a855f7",
    };

    public static string For(string name)
        => Map.TryGetValue(name.Trim(), out var color) ? color : Default;
}
