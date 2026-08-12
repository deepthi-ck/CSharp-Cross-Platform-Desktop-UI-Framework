namespace DesktopUi.DesktopCore;

public readonly record struct WindowKey(string Value)
{
    public override string ToString() => Value;
    public static WindowKey Of(string value) => new(value);
}
