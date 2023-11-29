using Meadow.Hardware;
using Meadow.Units;
using System.Linq;

namespace Meadow.Devices;

/// <summary>
/// Represents a Base Meadow F7 Feather device. Includes device-specific IO mapping,
/// capabilities and provides access to the various device-specific features.
/// </summary>
public abstract partial class F7FeatherBase : F7MicroBase, IF7FeatherMeadowDevice
{
    /// <summary>
    /// Initializes a new instance of the <see cref="F7FeatherBase"/> class.
    /// </summary>
    /// <param name="pins">The pinout for the Meadow F7 Feather device.</param>
    /// <param name="ioController">The Meadow IO controller.</param>
    /// <param name="analogCapabilities">The analog capabilities.</param>
    /// <param name="networkCapabilities">The network capabilities.</param>
    /// <param name="storageCapabilities">The storage capabilities.</param>
    protected F7FeatherBase(
        IF7FeatherPinout pins,
        IMeadowIOController ioController,
        AnalogCapabilities analogCapabilities,
        NetworkCapabilities networkCapabilities,
        StorageCapabilities storageCapabilities)
        : base(ioController, analogCapabilities, networkCapabilities, storageCapabilities)
    {
        Pins = pins;
    }

    /// <summary>
    /// Creates a PWM port for the specified pin, frequency, duty cycle, and inversion.
    /// </summary>
    /// <param name="pin">The pin to create the PWM port on.</param>
    /// <param name="frequency">The PWM frequency.</param>
    /// <param name="dutyCycle">The duty cycle.</param>
    /// <param name="inverted">If set to <c>true</c>, the PWM signal is inverted.</param>
    /// <returns>An instance of the PWM port.</returns>
    public override IPwmPort CreatePwmPort(
        IPin pin,
        Frequency frequency,
        float dutyCycle = IPwmOutputController.DefaultPwmDutyCycle,
        bool inverted = false)
    {
        bool isOnboard = IsOnboardLed(pin);
        return PwmPort.From(pin, this.IoController, frequency, dutyCycle, inverted, isOnboard);
    }

    /// <summary>
    /// Retrieves the IPin for the given pin name.
    /// </summary>
    /// <param name="pinName">The name of the pin.</param>
    /// <returns>The IPin instance.</returns>
    public override IPin GetPin(string pinName)
    {
        return Pins.AllPins.FirstOrDefault(p => p.Name == pinName || p.Key.ToString() == p.Name);
    }

    /// <summary>
    /// Gets the pins for the Meadow F7 Feather device.
    /// </summary>
    /// <value>The pins.</value>
    public IF7FeatherPinout Pins { get; }

    /// <summary>
    /// Tests whether or not the pin passed in belongs to an onboard LED
    /// component. Used for a dirty, dirty hack.
    /// </summary>
    /// <param name="pin">The pin to test.</param>
    /// <returns>Whether or not the pin belongs to the onboard LED.</returns>
    protected bool IsOnboardLed(IPin pin)
    {
        // HACK NOTE: can't compare directly here, so we're comparing the name.
        // might be able to cast and compare?
        return (
            pin.Name == Pins.OnboardLedBlue.Name ||
            pin.Name == Pins.OnboardLedGreen.Name ||
            pin.Name == Pins.OnboardLedRed.Name
        );
    }

    /// <summary>
    /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed.
    /// </summary>
    /// <returns>An instance of an I2cBus.</returns>
    public II2cBus CreateI2cBus(
        I2cBusSpeed busSpeed = I2cBusSpeed.Standard
    )
    {
        return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, busSpeed);
    }

    /// <summary>
    /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed.
    /// </summary>
    /// <param name="busNumber">The hardware bus number.</param>
    /// <param name="busSpeed">The bus speed desired.</param>
    /// <returns>An instance of an I2cBus.</returns>
    public override II2cBus CreateI2cBus(
        int busNumber = 1,
        I2cBusSpeed busSpeed = I2cBusSpeed.Standard
    )
    {
        return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, busSpeed);
    }

    /// <summary>
    /// Creates an SPI bus instance for the default Meadow F7 pins and the requested bus speed.
    /// </summary>
    /// <param name="speed">The bus speed desired.</param>
    /// <param name="busNumber">The hardware bus number.</param>
    /// <returns>An instance of an SpiBus.</returns>
    public override ISpiBus CreateSpiBus(
        Units.Frequency speed,
        int busNumber = 3
    )
    {
        return CreateSpiBus(Pins.SCK, Pins.COPI, Pins.CIPO, speed);
    }

    /// <summary>
    /// Retrieves the hardware bus number for the provided pins.
    /// </summary>
    /// <param name="clock">The clock pin.</param>
    /// <param name="copi">The COPI pin.</param>
    /// <param name="cipo">The CIPO pin.</param>
    /// <returns>The hardware bus number.</returns>
    protected override int GetSpiBusNumberForPins(IPin clock, IPin copi, IPin cipo)
    {
        // we're only looking at the clock pin.  
        // For the F7 meadow it's enough to know and any attempt to use other pins will get caught by other sanity checks
        // HACK NOTE: can't compare directly here, so we're comparing the name.
        // might be able to cast and compare?
        if (clock.Name == (Pins as IF7FeatherPinout)?.ESP_CLK.Name)
        {
            return 2;
        }
        else if (clock.Name == (Pins as IF7FeatherPinout)?.SCK.Name)
        {
            return 3;
        }

        // this is an unsupported bus, but will get caught elsewhere
        return -1;
    }
}