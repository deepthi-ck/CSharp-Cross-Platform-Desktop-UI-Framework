using DesktopUi.Frontend;
using DesktopUi.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5081/";
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBase) });
builder.Services.AddScoped<DesktopClient>();

var frontend = Environment.GetEnvironmentVariable("FRONTEND_DOTNET")
    ?? builder.Configuration["FrontendDotnet"]
    ?? "6";
var backend = Environment.GetEnvironmentVariable("BACKEND_DOTNET")
    ?? builder.Configuration["BackendDotnet"]
    ?? "8";
var branch = Environment.GetEnvironmentVariable("BRANCH_NAME")
    ?? builder.Configuration["BranchName"]
    ?? "CSharp_FE6_BE8";
builder.Services.AddSingleton(VersionInfo.FromEnvironment(frontend, backend, branch));

await builder.Build().RunAsync();
