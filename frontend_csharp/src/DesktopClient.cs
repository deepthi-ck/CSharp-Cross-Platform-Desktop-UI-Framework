using System.Net.Http.Json;
using DesktopUi.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace DesktopUi.Frontend;

public sealed class DesktopClient : IAsyncDisposable
{
    private readonly HttpClient _http;
    private HubConnection? _hub;
    public DesktopClient(HttpClient http) => _http = http;
    public string? LastError { get; private set; }
    public UiEntry? LastWindow { get; private set; }
    public event Action? StateChanged;

    public async Task<DesktopResponse?> OpenAsync(string windowId, string title)
    {
        var response = await _http.PostAsJsonAsync("/desktop/windows", new DesktopRequest { WindowId = windowId, Title = title });
        return await ReadAsync(response);
    }

    public async Task<DesktopResponse?> UpdateAsync(string windowId, string payload)
    {
        var response = await _http.PutAsJsonAsync($"/desktop/windows/{Uri.EscapeDataString(windowId)}",
            new DesktopRequest { Payload = payload });
        return await ReadAsync(response);
    }

    public async Task<DesktopResponse?> GetAsync(string windowId, bool synced = false)
    {
        var q = synced ? "?synced=true" : "";
        var response = await _http.GetAsync($"/desktop/windows/{Uri.EscapeDataString(windowId)}{q}");
        return await ReadAsync(response);
    }

    public async Task<DesktopResponse?> CloseAsync(string windowId)
    {
        var response = await _http.DeleteAsync($"/desktop/windows/{Uri.EscapeDataString(windowId)}");
        return await ReadAsync(response);
    }

    public Task<object?> StatsAsync() => _http.GetFromJsonAsync<object>("/desktop/stats");
    public Task<object?> HealthAsync() => _http.GetFromJsonAsync<object>("/health");
    public Task<object?> VersionAsync() => _http.GetFromJsonAsync<object>("/version");

    public async Task ConnectIpcAsync(string ipcUrl)
    {
        _hub = new HubConnectionBuilder().WithUrl(ipcUrl).WithAutomaticReconnect().Build();
        _hub.On<UiEntry>("WindowUpdated", entry => { LastWindow = entry; StateChanged?.Invoke(); });
        await _hub.StartAsync();
    }

    private async Task<DesktopResponse?> ReadAsync(HttpResponseMessage response)
    {
        try
        {
            var payload = await response.Content.ReadFromJsonAsync<DesktopResponse>();
            if (payload is null) { LastError = $"Empty response ({(int)response.StatusCode})"; return null; }
            LastError = payload.Success ? null : payload.Message;
            LastWindow = payload.Window ?? LastWindow;
            StateChanged?.Invoke();
            return payload;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            StateChanged?.Invoke();
            return null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub is not null) await _hub.DisposeAsync();
    }
}
