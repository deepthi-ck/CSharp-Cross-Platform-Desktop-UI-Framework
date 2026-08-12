using DesktopUi.DesktopCore;

namespace DesktopUi.Backend;

public sealed class EvictionManager
{
    private readonly InMemoryUiStore _store;
    private readonly DesktopPolicy _policy;
    private readonly DesktopStatistics _stats;
    public EvictionManager(InMemoryUiStore store, DesktopPolicy policy, DesktopStatistics stats)
    { _store = store; _policy = policy; _stats = stats; }

    public string? EnsureCapacity()
    {
        if (_store.Count < _policy.MaxWindows) return null;
        var evicted = _store.EvictLeastRecentlyUsed();
        _stats.SetActiveWindows(_store.Count);
        return evicted;
    }
}
