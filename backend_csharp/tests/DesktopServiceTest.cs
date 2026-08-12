using DesktopUi.Shared;
using Xunit;

namespace DesktopUi.Backend.Tests;

public class DesktopServiceTest
{
    [Fact]
    public async Task OpenUpdateClose_Flow()
    {
        var (svc, _, _, _) = TestHost.Create();
        var opened = await svc.OpenAsync(new DesktopRequest { WindowId = "window:1001", Title = "MainShell" });
        Assert.True(opened.Success);
        Assert.Contains("OPEN", opened.Message);

        var updated = await svc.UpdateAsync("window:1001", new DesktopRequest { Payload = "Visvantha" });
        Assert.True(updated.Success);
        Assert.Equal("Visvantha", updated.Window!.Payload["content"]);

        var closed = await svc.CloseAsync("window:1001");
        Assert.True(closed.Success);
        Assert.Equal("CLOSED", closed.Status);

        var get = svc.Get("window:1001");
        Assert.False(get.Success);
        Assert.Equal("CLOSED", get.Status);
    }
}
