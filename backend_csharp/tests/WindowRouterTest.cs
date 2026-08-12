using DesktopUi.DesktopCore;
using DesktopUi.Distribution;
using Xunit;

namespace DesktopUi.Backend.Tests;

public class WindowRouterTest
{
    [Fact]
    public void SameWindowId_RoutesConsistently()
    {
        var policy = new DesktopPolicy { WindowSlotCount = 3 };
        var store = new InMemoryUiStore();
        var registry = new WindowRegistry();
        for (var i = 0; i < 3; i++) registry.Register(new WindowNode(i, store, policy));
        var router = new WindowRouter(registry, 3);
        var a = router.ResolveSlot("window:1001");
        var b = router.ResolveSlot("window:1001");
        Assert.Equal(a, b);
        Assert.InRange(a, 0, 2);
        Assert.Equal(router.Route("window:1001").Slot, a);
    }
}
