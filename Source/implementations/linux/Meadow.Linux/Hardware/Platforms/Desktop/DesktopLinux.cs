using Meadow.Pinouts;

namespace Meadow;

/// <summary>
/// Represents a generic desktop Linux operating system.
/// </summary>
public class DesktopLinux : Linux, IDesktopLinuxDevice
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
}
