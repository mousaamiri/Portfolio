namespace Portfolio.Web.Models.Admin;

/// <summary>One row in a generic admin list table.</summary>
public class AdminListRow
{
    public Guid Id { get; init; }
    public List<string> Cells { get; init; } = [];
    /// <summary>When set, renders a Published/Draft status pill in its own column.</summary>
    public bool? Published { get; init; }
    /// <summary>When set (and Published is null), renders an Active/Inactive pill.</summary>
    public bool? Active { get; init; }
}

/// <summary>
/// Drives the shared _AdminList view so every admin entity reuses one list table
/// instead of a bespoke view. Column headers align with each row's Cells.
/// </summary>
public class AdminListViewModel
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string EditAction { get; init; } = string.Empty;
    public string DeleteAction { get; init; } = string.Empty;
    public string CreateAction { get; init; } = string.Empty;
    public string CreateLabel { get; init; } = "New";
    public string SidebarKey { get; init; } = string.Empty;
    public List<string> Headers { get; init; } = [];
    public bool ShowStatusColumn { get; init; }
    public List<AdminListRow> Rows { get; init; } = [];
}
