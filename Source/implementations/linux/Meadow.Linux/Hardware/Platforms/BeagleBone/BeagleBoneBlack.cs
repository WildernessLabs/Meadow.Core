using Meadow.Hardware;
using Meadow.Pinouts;
using Meadow.Units;
using System;
using System.Linq;

namespace Meadow;

/// <summary>
/// Represents a BeagleBoneBlack running the Linux operating system
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

    /// <summary>
    /// Initializes the specified pin to be an AnalogInput and returns the
    /// port used to sample the port value.
    /// </summary>
    /// <param name="pin">The pin to create the port on.</param>
    /// <param name="sampleCount">The number of samples to use for input averaging</param>
    /// <param name="sampleInterval">The interval between readings</param>
    public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval)
    {
        return CreateAnalogInputPort(pin, sampleCount, sampleInterval, 1.8.Volts());
    }

    /// <inheritdoc/>
    public override IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
    {
        // TODO: verify the vRef (1.8V on the BBB)
        var channelInfo = pin.SupportedChannels!.OfType<IAnalogChannelInfo>().FirstOrDefault();
        if (channelInfo == null)
        {
            throw new NotSupportedException($"Pin {pin.Name} is not Analog Input capable");
        }

        return new BeagleBoneAnalogInputPort(pin, channelInfo, sampleCount, sampleInterval);
    }

    /// <inheritdoc/>
    public override IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool inverted = false)
    {
        var channelInfo = pin.SupportedChannels!.OfType<IPwmChannelInfo>().FirstOrDefault();
        if (channelInfo == null)
        {
            throw new NotSupportedException($"Pin {pin.Name} is not PWM capable");
        }

        return new BeagleBonePwmPort(pin, channelInfo, frequency, dutyCycle, inverted);
    }

    /// <inheritdoc/>
    public override II2cBus CreateI2cBus(int busNumber = 2)
    {
        return base.CreateI2cBus(busNumber);
    }
}
