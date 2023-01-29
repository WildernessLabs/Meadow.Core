using Meadow.Hardware;
using Meadow.Units;
using System.Linq;

namespace Meadow.Devices
{
    public abstract partial class F7FeatherBase : F7MicroBase, IF7FeatherMeadowDevice
    {
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
        /// Retrives the IPin for the given pin name
        /// </summary>
        /// <param name="pinName"></param>
        /// <returns></returns>
        public override IPin GetPin(string pinName)
        {
            return Pins.AllPins.FirstOrDefault(p => p.Name == pinName || p.Key.ToString() == p.Name);
        }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        /// <value>The pins.</value>
        public IF7FeatherPinout Pins { get; }

        /// <summary>
        /// Tests whether or not the pin passed in belongs to an onboard LED
        /// component. Used for a dirty dirty hack.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns>whether or no the pin belons to the onboard LED</returns>
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
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <returns>An instance of an I2cBus</returns>
        public override II2cBus CreateI2cBus(
            I2cBusSpeed busSpeed,
            int busNumber = 0
        )
        {
            return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, new Frequency((int)busSpeed, Frequency.UnitType.Hertz));
        }

        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <param name="frequency">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public override II2cBus CreateI2cBus(
            int busNumber,
            Frequency frequency
        )
        {
            return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, frequency);
        }

        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <param name="speed">The bus speed desired</param>
        /// <param name="busNumber">The hardware bus number</param>
        /// <returns>An instance of an I2cBus</returns>
        public override ISpiBus CreateSpiBus(
            Units.Frequency speed,
            int busNumber = 3
        )
        {
            return CreateSpiBus(Pins.SCK, Pins.COPI, Pins.CIPO, speed);
        }

        /// <summary>
        /// Retrieves the hardware bus number for the provided pins
        /// </summary>
        /// <param name="clock"></param>
        /// <param name="copi"></param>
        /// <param name="cipo"></param>
        /// <returns></returns>
        protected override int GetSpiBusNumberForPins(IPin clock, IPin copi, IPin cipo)
        {
            // we're only looking at clock pin.  
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
}