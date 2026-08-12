using DesktopUi.Distribution;
using DesktopUi.Shared;
using Microsoft.AspNetCore.SignalR;

namespace DesktopUi.Backend;

public sealed class SignalRBroadcaster : IDesktopBroadcaster
{
    private readonly IHubContext<DesktopIpcBridge> _hub;
    public SignalRBroadcaster(IHubContext<DesktopIpcBridge> hub) => _hub = hub;
    public Task BroadcastWindowUpdateAsync(string windowId, UiEntry entry, string eventName) =>
        _hub.Clients.All.SendAsync(eventName, entry);
    public Task BroadcastWindowClosedAsync(string windowId) =>
        _hub.Clients.All.SendAsync("WindowClosed", new { windowId, status = "CLOSED" });
}
