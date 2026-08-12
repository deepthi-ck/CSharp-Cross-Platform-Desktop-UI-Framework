using System.Text.Json;
using DesktopUi.Backend;
using DesktopUi.DesktopCore;
using DesktopUi.Distribution;
using DesktopUi.Shared;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var configuration = LoadConfiguration();
var policy = DesktopPolicy.FromConfiguration(configuration);
var store = new InMemoryUiStore();
var stats = new DesktopStatistics();
stats.SetNodeSlotCount(policy.WindowSlotCount);

var registry = new WindowRegistry();
for (var i = 0; i < policy.WindowSlotCount; i++)
    registry.Register(new WindowNode(i, store, policy));

var router = new WindowRouter(registry, policy.WindowSlotCount);
var sync = new SyncManager();
var eviction = new EvictionManager(store, policy, stats);
var expiration = new ExpirationManager(store, stats);
var manager = new DesktopAppManager(router, sync, stats, eviction, expiration, store);
var service = new DesktopService(manager);

var frontend = Environment.GetEnvironmentVariable("FRONTEND_DOTNET") ?? DetectFromProps("FrontendDotnetVersion") ?? "6";
var backend = Environment.GetEnvironmentVariable("BACKEND_DOTNET") ?? DetectFromProps("BackendDotnetVersion") ?? ExtractTfmMajor() ?? "8";
var branch = Environment.GetEnvironmentVariable("BRANCH_NAME") ?? DetectFromProps("BranchName") ?? "CSharp_FE6_BE8";
var version = VersionInfo.FromEnvironment(frontend, backend, branch, "ready");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(configuration);
builder.Services.AddSingleton(policy);
builder.Services.AddSingleton(store);
builder.Services.AddSingleton(stats);
builder.Services.AddSingleton(registry);
builder.Services.AddSingleton(router);
builder.Services.AddSingleton(sync);
builder.Services.AddSingleton(eviction);
builder.Services.AddSingleton(expiration);
builder.Services.AddSingleton(manager);
builder.Services.AddSingleton(service);
builder.Services.AddSingleton(version);
builder.Services.AddSingleton<IDesktopBroadcaster, SignalRBroadcaster>();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true)));
builder.Services.AddSignalR();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("csharp-desktop-ui"))
    .WithTracing(t => t.AddAspNetCoreInstrumentation().AddConsoleExporter())
    .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddConsoleExporter());

var app = builder.Build();
sync.SetBroadcaster(app.Services.GetRequiredService<IDesktopBroadcaster>());
app.UseCors();
app.MapDesktopApi();
app.MapHub<DesktopIpcBridge>(configuration.IpcPath);
app.MapGet("/", () => Results.Json(new { application = version.Application, branch = version.Branch, status = "running" }));

if (string.Equals(Environment.GetEnvironmentVariable("SEED_SAMPLE_DATA"), "1", StringComparison.Ordinal))
    LoadSampleData(service);

var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? configuration.ApiBaseUrl;
app.Urls.Clear();
app.Urls.Add(urls);
Console.WriteLine($"Desktop platform listening on {urls} branch={branch} FE={frontend} BE={backend}");
app.Run();

static DesktopConfiguration LoadConfiguration()
{
    var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "config", "desktopsettings.json"));
    if (File.Exists(path))
    {
        return JsonSerializer.Deserialize<DesktopConfiguration>(File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new DesktopConfiguration();
    }
    return new DesktopConfiguration();
}

static void LoadSampleData(DesktopService service)
{
    var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "data", "sample-desktop-data.json"));
    if (!File.Exists(path)) return;
    using var doc = JsonDocument.Parse(File.ReadAllText(path));
    if (!doc.RootElement.TryGetProperty("seed_windows", out var windows)) return;
    foreach (var w in windows.EnumerateArray())
    {
        var id = w.GetProperty("windowId").GetString() ?? "window:1001";
        var title = w.GetProperty("title").GetString() ?? "MainShell";
        try { service.OpenAsync(new DesktopRequest { WindowId = id, Title = title }).GetAwaiter().GetResult(); }
        catch { /* already seeded */ }
    }
}

static string? DetectFromProps(string key)
{
    var props = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Directory.Build.props"));
    if (!File.Exists(props)) return null;
    var text = File.ReadAllText(props);
    var tag = $"<{key}>";
    var start = text.IndexOf(tag, StringComparison.Ordinal);
    if (start < 0) return null;
    start += tag.Length;
    var end = text.IndexOf('<', start);
    return end < 0 ? null : text[start..end].Trim();
}

static string? ExtractTfmMajor()
{
    var tfm = AppContext.TargetFrameworkName;
    if (tfm is null) return null;
    var idx = tfm.LastIndexOf('v');
    if (idx < 0) return null;
    return tfm[(idx + 1)..].Split('.')[0];
}

public partial class Program { }
