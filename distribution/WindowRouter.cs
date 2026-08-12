using System.Security.Cryptography;
using System.Text;

namespace DesktopUi.Distribution;

public sealed class WindowRouter
{
    private readonly WindowRegistry _registry;
    private readonly int _slotCount;

    public WindowRouter(WindowRegistry registry, int slotCount)
    {
        _registry = registry;
        _slotCount = Math.Max(1, slotCount);
    }

    public int ResolveSlot(string windowId)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(windowId));
        return (int)(BitConverter.ToUInt32(hash, 0) % (uint)_slotCount);
    }

    public WindowNode Route(string windowId) => _registry.Get(ResolveSlot(windowId));
}
