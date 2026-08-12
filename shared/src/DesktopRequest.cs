namespace DesktopUi.Shared;

public sealed class DesktopRequest
{
    public string? WindowId { get; set; }
    public string? Title { get; set; }
    public string? ViewName { get; set; }
    public string? Payload { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}
