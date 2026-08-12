namespace DesktopUi.Shared;

public sealed class UiEntry
{
    public string WindowId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ViewName { get; set; } = "MainShell";
    public Dictionary<string, string> Payload { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresAt { get; set; }
    public int Revision { get; set; }
    public string Status { get; set; } = "open";
    public string WindowSlot { get; set; } = string.Empty;

    public UiEntry Clone() => new()
    {
        WindowId = WindowId,
        Title = Title,
        ViewName = ViewName,
        Payload = new Dictionary<string, string>(Payload),
        CreatedAt = CreatedAt,
        ExpiresAt = ExpiresAt,
        Revision = Revision,
        Status = Status,
        WindowSlot = WindowSlot
    };
}
