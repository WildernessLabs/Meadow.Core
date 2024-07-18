using Meadow.Pinouts;

namespace Meadow;

/// <summary>
/// Represents a BeagleBoneBlack running the Linux operating system.
/// </summary>
public partial class BeagleBoneBlack : Linux
{
    private readonly DeviceCapabilities _capabilities;

    /// <inheritdoc/>
    public BeagleBoneBlackPinout Pins { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaspberryPi"/> class.
    /// </summary>
    public BeagleBoneBlack()
    {
        Pins = new BeagleBoneBlackPinout();
        Pins.Controller = this;

        _capabilities = new DeviceCapabilities(
            new AnalogCapabilities(true, 12), // TODO: check resolution
            new NetworkCapabilities(true, true),
            new StorageCapabilities(false)
            );
    }
}
