using Meadow.Hardware;

namespace Meadow;

/// <summary>
/// Defines the pinout configuration for an Orange Pi.
/// </summary>
public class OrangePiPinout : PinDefinitionBase, IPinDefinitions
{
    internal const string GpiodChip0 = "gpiochip0";
    internal int SysFsOffset { get; } = 0;

    internal OrangePiPinout() { }

    /// <summary>
    /// Represents the PI3 pin.
    /// </summary>
    public IPin PI3 => new LinuxFlexiPin(Controller, "PI3", "PIN40", 2 + SysFsOffset, GpiodChip0, 2,
        new IChannelInfo[]
        {
            new GpiodDigitalChannelInfo("PI3")
        });

    /// <summary>
    /// Represents Pin 40, which corresponds to PI3.
    /// </summary>
    public IPin Pin40 => PI3;
}

/// <summary>
/// Represents an Orange Pi running the Debian Linux operating system.
/// </summary>
public partial class OrangePi : Linux
{
    private readonly DeviceCapabilities _capabilities;

    /// <inheritdoc/>
    public OrangePiPinout Pins { get; }

    /// <inheritdoc/>
    public override DeviceCapabilities Capabilities => _capabilities;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrangePi"/> class.
    /// </summary>
    public OrangePi()
    {
        Pins = new OrangePiPinout();

        Pins.Controller = this;

        _capabilities = new DeviceCapabilities(
            new AnalogCapabilities(false, null),
            new NetworkCapabilities(false, true),
            new StorageCapabilities(true)
            );
    }

}
