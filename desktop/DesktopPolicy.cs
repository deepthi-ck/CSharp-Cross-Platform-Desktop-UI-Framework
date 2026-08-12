namespace DesktopUi.DesktopCore;

public sealed class DesktopPolicy
{
    public int WindowTtlSeconds { get; init; } = 300;
    public int MaxWindows { get; init; } = 32;
    public int WindowSlotCount { get; init; } = 3;

    public static DesktopPolicy FromConfiguration(DesktopUi.Shared.DesktopConfiguration cfg) => new()
    {
        WindowTtlSeconds = cfg.WindowTtlSeconds,
        MaxWindows = cfg.MaxWindows,
        WindowSlotCount = cfg.WindowSlotCount
    };
}
