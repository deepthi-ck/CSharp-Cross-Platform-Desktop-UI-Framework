using DesktopUi.Shared;

namespace DesktopUi.Backend;

public sealed class DesktopService
{
    private readonly DesktopAppManager _manager;
    public DesktopService(DesktopAppManager manager) => _manager = manager;
    public DesktopAppManager Manager => _manager;

    public Task<DesktopResponse> OpenAsync(DesktopRequest request)
    {
        var id = string.IsNullOrWhiteSpace(request.WindowId) ? $"window:{Guid.NewGuid():N}" : request.WindowId!;
        var title = string.IsNullOrWhiteSpace(request.Title) ? "MainShell" : request.Title!;
        var view = string.IsNullOrWhiteSpace(request.ViewName) ? "MainShell" : request.ViewName!;
        return _manager.OpenAsync(id, title, view);
    }

    public Task<DesktopResponse> UpdateAsync(string windowId, DesktopRequest request) =>
        _manager.UpdateAsync(windowId, request.Payload ?? string.Empty);

    public DesktopResponse Get(string windowId, bool preferSynced = false) =>
        _manager.Get(windowId, preferSynced);

    public Task<DesktopResponse> CloseAsync(string windowId) => _manager.CloseAsync(windowId);
    public object Stats() => _manager.Stats.Snapshot();
    public object Health() => _manager.Health();
}
