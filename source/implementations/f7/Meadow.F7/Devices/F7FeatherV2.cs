using Meadow.Hardware;
using System;

namespace Meadow.Devices;

/// <summary>
/// Represents an F7Micro2  device.
/// </summary>
[Obsolete("Use the F7FeatherV2 class instead.", true)]
public class F7Micro2 : F7FeatherV2
{
}

/// <summary>
/// Represents a Meadow F7 micro v2 device. Includes device-specific IO mapping,
/// capabilities and provides access to the various device-specific features.
/// </summary>
public partial class F7FeatherV2 : F7FeatherBase
{
    private Lazy<IAnalogInputPort?> _adc_bat;

    public F7FeatherV2()
        : base(new Pinout(),
              new F7FeatherGpioManager(),
              new AnalogCapabilities(true, DefaultA2DResolution),
              new NetworkCapabilities(true, false),
              new StorageCapabilities(false))
    {
        if (this.Information.Platform != MeadowPlatform.F7FeatherV2)
        {
            var message = $"Application is defined as F7FeatherV2, but running hardware is {this.Information.Platform}";
            Resolver.Log.Error(message);
            throw new UnsupportedPlatformException(this.Information.Platform, message);
        }

        _adc_bat = new Lazy<IAnalogInputPort?>(() =>
        {
            return this.CreateAnalogInputPort((Pins as F7FeatherV2.Pinout).BAT) ?? null;
        });

        Pins.Controller = this;
    }

    /// <summary>
    /// Retrieves the hardware bus number for the provided pins
    /// </summary>
    /// <param name="clock"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    protected override int GetI2CBusNumberForPins(IPin clock, IPin data)
    {
        if (clock.Name == (Pins as F7FeatherV2.Pinout)?.I2C_SCL.Name)
        {
            return 1;
        }

        // this is an unsupported bus, but will get caught elsewhere
        return -1;
    }

    /// <summary>
    /// Gets a BatteryInfo instance for the current state of the platform
    /// </summary>
    /// <returns>A BatteryInfo instance</returns>
    public override BatteryInfo? GetBatteryInfo()
    {
        return new BatteryInfo
        {
            // on V2 there is a voltage divider 2 x 499R and the voltage is taken from the center of the divider, effectively halving the input
            Voltage = _adc_bat?.Value?.Voltage * 2
        };
    }
}