using DesktopUi.Shared;

namespace DesktopUi.DesktopCore;

public sealed class InMemoryUiStore
{
    private readonly Dictionary<string, UiState> _store = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    public UiEntry Open(string windowId, string title, string viewName, TimeSpan ttl, string slot)
    {
        lock (_gate)
        {
            if (_store.ContainsKey(windowId))
                throw new InvalidOperationException($"Window already open: {windowId}");
            var entry = new UiEntry
            {
                WindowId = windowId,
                Title = title,
                ViewName = viewName,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.Add(ttl),
                WindowSlot = slot,
                Status = "open",
                Payload = new Dictionary<string, string> { ["phase"] = "ready" }
            };
            _store[windowId] = new UiState(entry);
            return entry.Clone();
        }
    }

    public UiEntry Update(string windowId, string payload)
    {
        lock (_gate)
        {
            var state = Require(windowId);
            EnsureOpen(state);
            if (string.IsNullOrWhiteSpace(payload))
                throw new InvalidOperationException("Update rejected: empty payload.");
            state.Entry.Payload["content"] = payload;
            state.Entry.Payload["phase"] = "updated";
            state.Entry.Revision++;
            state.Touch();
            return state.Entry.Clone();
        }
    }

    public UiEntry? GetState(string windowId)
    {
        lock (_gate)
        {
            if (!_store.TryGetValue(windowId, out var state)) return null;
            if (IsExpired(state)) { _store.Remove(windowId); return null; }
            return state.Entry.Clone();
        }
    }

    public bool Close(string windowId)
    {
        lock (_gate) { return _store.Remove(windowId); }
    }

    public bool Contains(string windowId)
    {
        lock (_gate)
        {
            if (!_store.TryGetValue(windowId, out var state)) return false;
            if (IsExpired(state)) { _store.Remove(windowId); return false; }
            return true;
        }
    }

    public void Clear() { lock (_gate) { _store.Clear(); } }
    public int Count { get { lock (_gate) { return _store.Count; } } }

    public List<string> EvictExpired()
    {
        lock (_gate)
        {
            var expired = _store.Where(kv => IsExpired(kv.Value)).Select(kv => kv.Key).ToList();
            foreach (var id in expired) _store.Remove(id);
            return expired;
        }
    }

    public string? EvictLeastRecentlyUsed()
    {
        lock (_gate)
        {
            if (_store.Count == 0) return null;
            var victim = _store.OrderBy(kv => kv.Value.LastActivityUtc).First();
            _store.Remove(victim.Key);
            return victim.Key;
        }
    }

    private UiState Require(string windowId) =>
        _store.TryGetValue(windowId, out var state) ? state : throw new KeyNotFoundException($"Window not found: {windowId}");

    private static void EnsureOpen(UiState state)
    {
        if (IsExpired(state)) throw new InvalidOperationException("Window expired.");
        if (!string.Equals(state.Entry.Status, "open", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Window not open.");
    }

    private static bool IsExpired(UiState state) =>
        state.Entry.ExpiresAt is { } exp && exp <= DateTimeOffset.UtcNow;
}
