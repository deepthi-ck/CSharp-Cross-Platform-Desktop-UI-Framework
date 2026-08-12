using DesktopUi.Shared;

namespace DesktopUi.DesktopCore;

public sealed class UiState
{
    public UiEntry Entry { get; }
    public DateTimeOffset LastActivityUtc { get; set; } = DateTimeOffset.UtcNow;
    public UiState(UiEntry entry) => Entry = entry;
    public void Touch() => LastActivityUtc = DateTimeOffset.UtcNow;
}
