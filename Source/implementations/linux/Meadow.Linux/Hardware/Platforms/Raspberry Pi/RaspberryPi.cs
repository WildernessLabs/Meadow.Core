using Meadow.Hardware;
using Meadow.Pinouts;
using Meadow.Units;
using System;
using System.IO;
using System.Linq;

namespace Meadow;

/// <summary>
/// Represents a Raspberry Pi running the Linux operating system.
/// </summary>
public partial class RaspberryPi : Linux
{
    private readonly DeviceCapabilities _capabilities;
    private bool? _isPi5;

    /// <inheritdoc/>
    public RaspberryPiPinout Pins { get; }

    /// <inheritdoc/>
    public override DeviceCapabilities Capabilities => _capabilities;

    /// <summary>
    /// Returns true if the current platform is a Raspberry Pi 5
    /// </summary>
    public bool IsRaspberryPi5 => _isPi5 ??= CheckIfPi5();

    /// <summary>
    /// Initializes a new instance of the <see cref="RaspberryPi"/> class.
    /// </summary>
    public RaspberryPi()
    {
        // are we a Pi5?  if so, the GPIOs have moved
        if (IsRaspberryPi5)
        {
            Console.WriteLine("We're on a Raspberry Pi 5");
        }

        Pins = new RaspberryPiPinout(
            IsRaspberryPi5 ?
                RaspberryPiPinout.GpiodChipPi5 :
                RaspberryPiPinout.GpiodChipPi4,
            IsRaspberryPi5 ? 53 : 0
            );

        Pins.Controller = this;

        _capabilities = new DeviceCapabilities(
            new AnalogCapabilities(false, null),
            new NetworkCapabilities(false, true),
            new StorageCapabilities(true)
            );
    }

    private bool CheckIfPi5()
    {
        // TODO: is there a better way to find this?
        try
        {
            var model = File.ReadAllText("/proc/device-tree/model");
            return model.Contains("Raspberry Pi 5");
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public override IPin GetPin(string pinName)
    {
        return Pins.AllPins.First(p => string.Compare(p.Name, pinName) == 0);
    }

    /// <inheritdoc/>
    public override II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
    {
        if (clock == Pins["PIN05"] && data == Pins["PIN03"])
        {
            return new I2CBus(1, busSpeed);
        }

        throw new ArgumentOutOfRangeException("Requested pins are not I2C bus pins");
    }

    /// <inheritdoc/>
    public override ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, SpiClockConfiguration.Mode mode, Frequency speed)
    {
        // TODO: validate pins for both buses

        // just switch on the clock, assume they did the rest right
        if (clock.Key.ToString() == "PIN40")
        {
            Resolver.Log.Info($"EQUAL");
            return new SpiBus(1, 0, (SpiBus.SpiMode)mode, speed);
        }

        Resolver.Log.Info($"NOT {clock.Key.ToString()}");
        return new SpiBus(0, 0, (SpiBus.SpiMode)mode, speed);
    }

    /// <inheritdoc/>
    public override IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
    {
        // TODO
        // We need to call mmap to map the PWM registers. this is a good ref:
        // https://github.com/WiringPi/WiringPi/blob/8960cc91b911db8ec0c272781edf34b8aedb60d9/wiringPi/wiringPi.c#L2894

        return base.CreatePwmPort(pin, frequency, dutyCycle, invert);
    }
}
