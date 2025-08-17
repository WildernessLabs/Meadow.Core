namespace Meadow.AspNetCore.Builder;

internal class ExtensionApp<D> : App<D>
    where D : class, IMeadowDevice
{
    public ExtensionApp()
    {
        var d = Device;
    }
}

