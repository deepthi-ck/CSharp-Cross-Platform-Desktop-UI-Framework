namespace DesktopUi.Distribution;

public sealed class WindowRegistry
{
    private readonly Dictionary<int, WindowNode> _nodes = new();
    public void Register(WindowNode node) => _nodes[node.Slot] = node;
    public WindowNode Get(int slot) =>
        _nodes.TryGetValue(slot, out var node) ? node : throw new KeyNotFoundException($"Window slot not registered: {slot}");
    public IReadOnlyCollection<WindowNode> All => _nodes.Values;
    public int Count => _nodes.Count;
}
