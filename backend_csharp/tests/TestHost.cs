using DesktopUi.DesktopCore;
using DesktopUi.Distribution;

namespace DesktopUi.Backend.Tests;

internal static class TestHost
{
    public static (DesktopService Service, DesktopAppManager Manager, SyncManager Sync, InMemoryUiStore Store) Create(
        int maxWindows = 32, int ttlSeconds = 300, int slots = 3)
    {
        var policy = new DesktopPolicy { MaxWindows = maxWindows, WindowTtlSeconds = ttlSeconds, WindowSlotCount = slots };
        var store = new InMemoryUiStore();
        var stats = new DesktopStatistics();
        stats.SetNodeSlotCount(slots);
        var registry = new WindowRegistry();
        for (var i = 0; i < slots; i++) registry.Register(new WindowNode(i, store, policy));
        var router = new WindowRouter(registry, slots);
        var sync = new SyncManager();
        var eviction = new EvictionManager(store, policy, stats);
        var expiration = new ExpirationManager(store, stats);
        var manager = new DesktopAppManager(router, sync, stats, eviction, expiration, store);
        return (new DesktopService(manager), manager, sync, store);
    }
}
