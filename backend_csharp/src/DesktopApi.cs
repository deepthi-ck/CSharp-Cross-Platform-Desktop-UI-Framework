using DesktopUi.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DesktopUi.Backend;

public static class DesktopApi
{
    public static IEndpointRouteBuilder MapDesktopApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", (DesktopService svc) => Results.Json(svc.Health()));
        app.MapGet("/version", (VersionInfo version) => Results.Json(new
        {
            application = version.Application,
            frontend_dotnet = version.FrontendDotnet,
            backend_dotnet = version.BackendDotnet,
            branch = version.Branch,
            desktop = version.Desktop
        }));
        app.MapGet("/desktop/stats", (DesktopService svc) => Results.Json(svc.Stats()));
        app.MapPost("/desktop/windows", async (DesktopRequest request, DesktopService svc) =>
        {
            var response = await svc.OpenAsync(request);
            return response.Success ? Results.Json(response) : Results.BadRequest(response);
        });
        app.MapGet("/desktop/windows/{id}", (string id, DesktopService svc, bool synced = false) =>
        {
            var response = svc.Get(id, synced);
            return response.Success ? Results.Json(response) : Results.NotFound(response);
        });
        app.MapPut("/desktop/windows/{id}", async (string id, DesktopRequest request, DesktopService svc) =>
        {
            var response = await svc.UpdateAsync(id, request);
            return response.Success ? Results.Json(response) : Results.BadRequest(response);
        });
        app.MapDelete("/desktop/windows/{id}", async (string id, DesktopService svc) =>
            Results.Json(await svc.CloseAsync(id)));
        return app;
    }
}
