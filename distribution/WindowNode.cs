using DesktopUi.DesktopCore;
using DesktopUi.Shared;

namespace DesktopUi.Distribution;

public sealed class WindowNode
{
    private readonly InMemoryUiStore _store;
    private readonly DesktopPolicy _policy;
    public int Slot { get; }

    public WindowNode(int slot, InMemoryUiStore store, DesktopPolicy policy)
    {
        Slot = slot; _store = store; _policy = policy;
    }

    public UiEntry Open(string windowId, string title, string viewName) =>
        _store.Open(windowId, title, viewName, TimeSpan.FromSeconds(_policy.WindowTtlSeconds), $"slot-{Slot}");

    public UiEntry Update(string windowId, string payload) => _store.Update(windowId, payload);
    public UiEntry? GetState(string windowId) => _store.GetState(windowId);
    public bool Close(string windowId) => _store.Close(windowId);
    public bool Contains(string windowId) => _store.Contains(windowId);
}
