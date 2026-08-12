using DesktopUi.Shared;

namespace DesktopUi.Distribution;

public interface IDesktopBroadcaster
{
    Task BroadcastWindowUpdateAsync(string windowId, UiEntry entry, string eventName);
    Task BroadcastWindowClosedAsync(string windowId);
}

public sealed class SyncManager
{
    private readonly Dictionary<string, List<Action<UiEntry>>> _observers = new(StringComparer.Ordinal);
    private readonly Dictionary<string, UiEntry> _syncedMirror = new(StringComparer.Ordinal);
    private readonly object _gate = new();
    private IDesktopBroadcaster? _broadcaster;

    public void SetBroadcaster(IDesktopBroadcaster broadcaster) => _broadcaster = broadcaster;

    public IDisposable Subscribe(string windowId, Action<UiEntry> observer)
    {
        lock (_gate)
        {
            if (!_observers.TryGetValue(windowId, out var list))
            {
                list = new List<Action<UiEntry>>();
                _observers[windowId] = list;
            }
            list.Add(observer);
        }
        return new Sub(() => Unsubscribe(windowId, observer));
    }

    public void Unsubscribe(string windowId, Action<UiEntry> observer)
    {
        lock (_gate)
        {
            if (_observers.TryGetValue(windowId, out var list))
            {
                list.Remove(observer);
                if (list.Count == 0) _observers.Remove(windowId);
            }
        }
    }

    public async Task PublishAsync(string windowId, UiEntry entry, string eventName = "WindowUpdated")
    {
        List<Action<UiEntry>> snapshot;
        lock (_gate)
        {
            _syncedMirror[windowId] = entry.Clone();
            snapshot = _observers.TryGetValue(windowId, out var list) ? list.ToList() : new List<Action<UiEntry>>();
        }
        foreach (var obs in snapshot) obs(entry);
        if (_broadcaster is not null)
            await _broadcaster.BroadcastWindowUpdateAsync(windowId, entry, eventName);
    }

    public UiEntry? GetSynced(string windowId)
    {
        lock (_gate) { return _syncedMirror.TryGetValue(windowId, out var e) ? e.Clone() : null; }
    }

    public async Task PublishClosedAsync(string windowId)
    {
        lock (_gate)
        {
            _observers.Remove(windowId);
            _syncedMirror.Remove(windowId);
        }
        if (_broadcaster is not null)
            await _broadcaster.BroadcastWindowClosedAsync(windowId);
    }

    private sealed class Sub : IDisposable
    {
        private readonly Action _d;
        public Sub(Action d) => _d = d;
        public void Dispose() => _d();
    }
}
