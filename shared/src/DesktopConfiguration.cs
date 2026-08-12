namespace DesktopUi.Shared;

public sealed class DesktopConfiguration
{
    public int WindowTtlSeconds { get; set; } = 300;
    public int MaxWindows { get; set; } = 32;
    public int WindowSlotCount { get; set; } = 3;
    public string ApiBaseUrl { get; set; } = "http://localhost:5081";
    public string IpcPath { get; set; } = "/ipc/desktop";
}
