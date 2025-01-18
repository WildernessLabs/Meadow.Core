using Meadow.Hardware;
using Meadow.Pinouts;
using System.Linq;

namespace Meadow;

/// <summary>
/// Represents an NVIDIA Jetson Xavier AGX running the Linux operating system.
/// </summary>
public class JetsonXavierAgx : Linux
{
    private readonly DeviceCapabilities _capabilities;
    private IPlatformOS _platformOS;

    /// <inheritdoc/>
    public JetsonXavierAGXPinout Pins { get; }

    /// <inheritdoc/>
    public override DeviceCapabilities Capabilities => _capabilities;
    /// <inheritdoc/>
    public override IPlatformOS PlatformOS => _platformOS;

    /// <summary>
    /// Creates the Meadow on Linux infrastructure instance
    /// </summary>
    public JetsonXavierAgx()
    {
        _platformOS = new JetsonPlatformOS();

        Pins = new JetsonXavierAGXPinout();

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
}
