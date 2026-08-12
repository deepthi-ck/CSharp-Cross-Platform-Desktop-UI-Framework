using DesktopUi.Shared;
using Xunit;

namespace DesktopUi.Backend.Tests;

public class SyncManagerTest
{
    [Fact]
    public async Task Publish_NotifiesAndMirrorsSyncedState()
    {
        var (_, manager, sync, _) = TestHost.Create();
        UiEntry? observed = null;
        using var sub = sync.Subscribe("window:sync", e => observed = e);
        await manager.OpenAsync("window:sync", "MainShell", "MainShell");
        await manager.UpdateAsync("window:sync", "Visvantha");
        Assert.NotNull(observed);
        Assert.Equal("Visvantha", observed!.Payload["content"]);
        var synced = sync.GetSynced("window:sync");
        Assert.NotNull(synced);
        Assert.Equal("Visvantha", synced!.Payload["content"]);
        var viaManager = manager.Get("window:sync", preferSynced: true);
        Assert.True(viaManager.Success);
        Assert.Equal("OK_SYNCED", viaManager.Message);
    }
}
