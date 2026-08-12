using DesktopUi.Shared;
using Xunit;

namespace DesktopUi.Shared.Tests;

public class UiEntryTest
{
    [Fact]
    public void Clone_CopiesIndependentPayload()
    {
        var entry = new UiEntry { WindowId = "window:1001", Title = "MainShell", Payload = { ["user"] = "Visvantha" } };
        var clone = entry.Clone();
        clone.Payload["user"] = "Other";
        Assert.Equal("Visvantha", entry.Payload["user"]);
        Assert.Equal("Other", clone.Payload["user"]);
    }
}

public class VersionInfoTest
{
    [Fact]
    public void FromEnvironment_SetsFields()
    {
        var v = VersionInfo.FromEnvironment("6", "8", "CSharp_FE6_BE8");
        Assert.Equal("6", v.FrontendDotnet);
        Assert.Equal("8", v.BackendDotnet);
        Assert.Equal("CSharp_FE6_BE8", v.Branch);
        Assert.Equal("ready", v.Desktop);
    }

    [Fact]
    public void ParseBranch_RejectsSameVersion() =>
        Assert.Throws<InvalidOperationException>(() => BuildContext.ParseBranch("CSharp_FE8_BE8"));

    [Fact]
    public void ParseBranch_MapsFeBeCorrectly()
    {
        var ctx = BuildContext.ParseBranch("CSharp_FE6_BE8");
        Assert.Equal(6, ctx.FrontendVersion);
        Assert.Equal(8, ctx.BackendVersion);
        Assert.Equal("net6.0", ctx.FrontendTfm);
        Assert.Equal("net8.0", ctx.BackendTfm);
        Assert.Equal(".NET 6 / .NET 8", ctx.CustomerVersion);
    }
}
