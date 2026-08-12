namespace DesktopUi.Shared;

public sealed class VersionInfo
{
    public string Application { get; set; } = "C# Cross-platform Desktop UI Framework";
    public string FrontendDotnet { get; set; } = string.Empty;
    public string BackendDotnet { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string Desktop { get; set; } = "ready";

    public static VersionInfo FromEnvironment(string frontend, string backend, string branch, string desktop = "ready") => new()
    {
        FrontendDotnet = frontend,
        BackendDotnet = backend,
        Branch = branch,
        Desktop = desktop
    };
}
