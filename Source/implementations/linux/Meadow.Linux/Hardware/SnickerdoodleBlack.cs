using Meadow.Hardware;
using Meadow.Pinouts;
using System.Linq;

namespace Meadow;

/// <summary>
/// Represents the Snickerdoodle Black single-board computer, providing access to its pinout and capabilities.
/// </summary>
public class SnickerdoodleBlack : Linux
{
    private readonly DeviceCapabilities _capabilities;

    /// <summary>
    /// Gets the pinout for the Snickerdoodle Black.
    /// </summary>
    public SnickerdoodleBlackPinout Pins { get; }

    /// <inheritdoc/>
    public override DeviceCapabilities Capabilities => _capabilities;

    /// <summary>
    /// Creates the Meadow on Linux infrastructure instance
    /// </summary>
    public SnickerdoodleBlack()
    {
        Pins = new SnickerdoodleBlackPinout();

        Pins.Controller = this;

        _capabilities = new DeviceCapabilities(
            new AnalogCapabilities(false, null),
            new NetworkCapabilities(false, true),
            new StorageCapabilities(true)
            );
    }

    /// <inheritdoc/>
    public override IPin GetPin(string pinName)
    {
        return Pins.AllPins.First(p => string.Compare(p.Name, pinName) == 0);
    }

    /// <inheritdoc/>
    public override II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
    {
        return new KrtklI2CBus(busSpeed);
    }
}
