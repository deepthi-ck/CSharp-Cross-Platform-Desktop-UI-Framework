using System.Text.Json;
using DesktopUi.Frontend;
using DesktopUi.Shared;
using RichardSzalay.MockHttp;
using Xunit;

namespace DesktopUi.Frontend.Tests;

public class DesktopClientTest
{
    [Fact]
    public async Task OpenAsync_PostsToDesktopApi()
    {
        var mock = new MockHttpMessageHandler();
        mock.When(HttpMethod.Post, "http://localhost/desktop/windows")
            .Respond("application/json", JsonSerializer.Serialize(new DesktopResponse
            {
                Success = true,
                Status = "OK",
                Message = "OPEN = SUCCESS",
                Window = new UiEntry { WindowId = "window:1001", Title = "MainShell" }
            }));
        var http = mock.ToHttpClient();
        http.BaseAddress = new Uri("http://localhost/");
        var client = new DesktopClient(http);
        var result = await client.OpenAsync("window:1001", "MainShell");
        Assert.NotNull(result);
        Assert.True(result!.Success);
        Assert.Equal("window:1001", result.Window!.WindowId);
    }
}
