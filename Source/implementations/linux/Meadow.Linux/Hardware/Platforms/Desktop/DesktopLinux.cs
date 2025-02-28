using Meadow.Foundation.Displays;
using Meadow.Peripherals.Displays;

namespace Meadow.Pinouts;

/// <summary>
/// Represents a generic desktop Linux operating system.
/// </summary>
public class DesktopLinux : Linux, IPixelDisplayProvider
{
    /// <inheritdoc/>
    public EmptyPinout Pins { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopLinux"/> class.
    /// </summary>
    public DesktopLinux()
    {
        Pins = new EmptyPinout();
        Pins.Controller = this;
    }

    /// <inheritdoc/>
    public IResizablePixelDisplay CreateDisplay(int? width = null, int? height = null)
    {
        return new SilkDisplay(width ?? 320, height ?? 240);
    }
}
