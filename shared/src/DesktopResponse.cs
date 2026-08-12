namespace DesktopUi.Shared;

public sealed class DesktopResponse
{
    public bool Success { get; set; }
    public string Status { get; set; } = "OK";
    public string Message { get; set; } = string.Empty;
    public UiEntry? Window { get; set; }
    public object? Stats { get; set; }
}
