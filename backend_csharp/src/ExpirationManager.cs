using DesktopUi.DesktopCore;

namespace DesktopUi.Backend;

public sealed class ExpirationManager
{
    private readonly InMemoryUiStore _store;
    private readonly DesktopStatistics _stats;
    public ExpirationManager(InMemoryUiStore store, DesktopStatistics stats) { _store = store; _stats = stats; }
    public IReadOnlyList<string> Sweep()
    {
        var expired = _store.EvictExpired();
        RefreshStats();
        return expired;
    }
    public void RefreshStats() => _stats.SetActiveWindows(_store.Count);
}
