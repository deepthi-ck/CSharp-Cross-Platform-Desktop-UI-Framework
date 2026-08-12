using DesktopUi.Shared;
using Microsoft.AspNetCore.SignalR;

namespace DesktopUi.Backend;

/// <summary>Electron.NET-inspired host↔UI IPC bridge via SignalR.</summary>
public sealed class DesktopIpcBridge : Hub
{
    private readonly DesktopService _service;
    public DesktopIpcBridge(DesktopService service) => _service = service;

    public async Task OpenWindow(string windowId, string title)
    {
        var response = await _service.OpenAsync(new DesktopRequest { WindowId = windowId, Title = title });
        await Clients.Caller.SendAsync("OpenResult", response);
        if (response.Success && response.Window is not null)
            await Clients.All.SendAsync("WindowOpened", response.Window);
    }

    public async Task UpdateWindow(string windowId, string payload)
    {
        var response = await _service.UpdateAsync(windowId, new DesktopRequest { Payload = payload });
        await Clients.Caller.SendAsync("UpdateResult", response);
        if (response.Success && response.Window is not null)
            await Clients.All.SendAsync("WindowUpdated", response.Window);
    }

    public async Task CloseWindow(string windowId)
    {
        var response = await _service.CloseAsync(windowId);
        await Clients.All.SendAsync("WindowClosed", new { windowId, response.Status });
        await Clients.Caller.SendAsync("CloseResult", response);
    }
}
