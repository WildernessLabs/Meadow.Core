using Meadow.Hardware;
using Meadow.Units;
using System.Linq;

namespace Meadow.Devices
{
    public abstract partial class F7CoreComputeBase : F7MicroBase, IF7CoreComputeMeadowDevice
    {
        protected F7CoreComputeBase(
            IF7CoreComputePinout pins,
            IMeadowIOController ioController,
            AnalogCapabilities analogCapabilities,
            NetworkCapabilities networkCapabilities)
            : base(ioController, analogCapabilities, networkCapabilities)
        {
            Pins = pins;
        }

        public override IPin GetPin(string pinName)
        {
            return Pins.AllPins.FirstOrDefault(p => p.Name == pinName || p.Key.ToString() == p.Name);
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

        protected override int GetI2CBusNumberForPins(IPin clock, IPin data)
        {
            if (clock.Name == (Pins as F7CoreCompute.Pinout)?.I2C3_SCL.Name)
            {
                return 3;
            }
            if (clock.Name == (Pins as F7CoreCompute.Pinout)?.I2C1_SCL.Name)
            {
                return 1;
            }

            // this is an unsupported bus, but will get caught elsewhere
            return -1;
        }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        /// <value>The pins.</value>
        public IF7CoreComputePinout Pins { get; }

        public override ISpiBus CreateSpiBus(
            Units.Frequency speed
        )
        {
            return CreateSpiBus(Pins.SCK, Pins.COPI, Pins.CIPO, speed);
        }

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

        public override IPwmPort CreatePwmPort(
            IPin pin,
            float frequency = IPwmOutputController.DefaultPwmFrequency,
            float dutyCycle = IPwmOutputController.DefaultPwmDutyCycle,
            bool inverted = false)
        {
            return PwmPort.From(pin, this.IoController, frequency, dutyCycle, inverted, false);
        }
    }
}
