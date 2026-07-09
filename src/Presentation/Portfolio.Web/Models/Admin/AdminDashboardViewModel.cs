namespace Portfolio.Web.Models.Admin;

/// <summary>Counts shown on the admin dashboard, fetched live from Portfolio.API.</summary>
public class AdminDashboardViewModel
{
    public int Projects { get; init; }
    public int Articles { get; init; }
    public int Experiences { get; init; }
    public int Education { get; init; }
    public int Skills { get; init; }
    public int Testimonials { get; init; }
    public int Messages { get; init; }
    public int UnreadMessages { get; init; }
}
