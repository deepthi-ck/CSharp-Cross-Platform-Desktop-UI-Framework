using DesktopUi.Distribution;
using DesktopUi.Shared;

namespace DesktopUi.Backend;

/// <summary>Backend facade over a routed primary WindowNode (canonical node lives in distribution/).</summary>
public sealed class WindowNodeFacade
{
    private readonly WindowNode _node;
    public WindowNodeFacade(WindowNode node) => _node = node;
    public int Slot => _node.Slot;
    public UiEntry Open(string id, string title, string view) => _node.Open(id, title, view);
    public UiEntry Update(string id, string payload) => _node.Update(id, payload);
    public UiEntry? GetState(string id) => _node.GetState(id);
    public bool Close(string id) => _node.Close(id);
}
