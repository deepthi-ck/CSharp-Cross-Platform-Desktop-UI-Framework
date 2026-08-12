using DesktopUi.DesktopCore;
using DesktopUi.Distribution;
using DesktopUi.Shared;

namespace DesktopUi.Backend;

public sealed class DesktopAppManager
{
    private readonly WindowRouter _router;
    private readonly SyncManager _sync;
    private readonly DesktopStatistics _stats;
    private readonly EvictionManager _eviction;
    private readonly ExpirationManager _expiration;
    private readonly InMemoryUiStore _store;

    public DesktopAppManager(WindowRouter router, SyncManager sync, DesktopStatistics stats,
        EvictionManager eviction, ExpirationManager expiration, InMemoryUiStore store)
    {
        _router = router; _sync = sync; _stats = stats;
        _eviction = eviction; _expiration = expiration; _store = store;
    }

    public InMemoryUiStore Store => _store;
    public DesktopStatistics Stats => _stats;
    public SyncManager Sync => _sync;
    public WindowRouter Router => _router;

    public async Task<DesktopResponse> OpenAsync(string windowId, string title, string viewName)
    {
        _expiration.Sweep();
        _eviction.EnsureCapacity();
        var node = new WindowNodeFacade(_router.Route(windowId));
        var entry = node.Open(windowId, title, viewName);
        _stats.RecordOpen();
        _expiration.RefreshStats();
        await _sync.PublishAsync(windowId, entry, "WindowOpened");
        return Ok(entry, "OPEN = SUCCESS");
    }

    public async Task<DesktopResponse> UpdateAsync(string windowId, string payload)
    {
        _expiration.Sweep();
        try
        {
            var node = new WindowNodeFacade(_router.Route(windowId));
            var entry = node.Update(windowId, payload);
            _stats.RecordUpdate();
            _stats.RecordAccepted();
            await _sync.PublishAsync(windowId, entry, "WindowUpdated");
            return Ok(entry, "UPDATE = SUCCESS");
        }
        catch (Exception ex)
        {
            _stats.RecordRejected();
            return Fail(ex.Message);
        }
    }

    public DesktopResponse Get(string windowId, bool preferSynced = false)
    {
        _expiration.Sweep();
        if (preferSynced)
        {
            var synced = _sync.GetSynced(windowId);
            if (synced is not null) return Ok(synced, "OK_SYNCED");
        }
        var node = new WindowNodeFacade(_router.Route(windowId));
        var entry = node.GetState(windowId);
        if (entry is null) return Fail("NOT_FOUND", "CLOSED");
        return Ok(entry, "OK");
    }

    public async Task<DesktopResponse> CloseAsync(string windowId)
    {
        var node = new WindowNodeFacade(_router.Route(windowId));
        var existed = node.Close(windowId);
        if (!existed) return Fail("NOT_FOUND", "CLOSED");
        _stats.RecordClose();
        _expiration.RefreshStats();
        await _sync.PublishClosedAsync(windowId);
        return new DesktopResponse { Success = true, Status = "CLOSED", Message = "CLOSE = SUCCESS" };
    }

    public object Health()
    {
        _expiration.Sweep();
        return new { status = "healthy", desktop = "available", windows = _store.Count };
    }

    private static DesktopResponse Ok(UiEntry entry, string message) => new()
    { Success = true, Status = "OK", Message = message, Window = entry };

    private static DesktopResponse Fail(string message, string status = "ERROR") => new()
    { Success = false, Status = status, Message = message };
}
