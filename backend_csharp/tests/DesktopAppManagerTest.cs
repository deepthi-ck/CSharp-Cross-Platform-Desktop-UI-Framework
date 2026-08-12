using Xunit;

namespace DesktopUi.Backend.Tests;

public class DesktopAppManagerTest
{
    [Fact]
    public async Task Statistics_ReflectOperations()
    {
        var (_, manager, _, _) = TestHost.Create();
        await manager.OpenAsync("window:stats", "MainShell", "MainShell");
        await manager.UpdateAsync("window:stats", "Visvantha");
        await manager.UpdateAsync("window:stats", "");
        await manager.CloseAsync("window:stats");
        Assert.Equal(1, manager.Stats.OpenCount);
        Assert.Equal(1, manager.Stats.UpdateCount);
        Assert.Equal(1, manager.Stats.AcceptedUpdateCount);
        Assert.Equal(1, manager.Stats.RejectedUpdateCount);
        Assert.Equal(1, manager.Stats.CloseCount);
    }

    [Fact]
    public async Task Ttl_ExpiresWindow()
    {
        var (_, manager, _, _) = TestHost.Create(ttlSeconds: 1);
        await manager.OpenAsync("window:ttl", "MainShell", "MainShell");
        await Task.Delay(1100);
        var get = manager.Get("window:ttl");
        Assert.False(get.Success);
        Assert.Equal("CLOSED", get.Status);
    }

    [Fact]
    public async Task Eviction_RespectsCapacity()
    {
        var (_, manager, _, store) = TestHost.Create(maxWindows: 2);
        await manager.OpenAsync("window:e1", "A", "A");
        await Task.Delay(20);
        await manager.OpenAsync("window:e2", "B", "B");
        await Task.Delay(20);
        await manager.OpenAsync("window:e3", "C", "C");
        Assert.True(store.Count <= 2);
        Assert.True(store.Contains("window:e3"));
    }
}
