using Meadow.Hardware;
using Meadow.Units;
using System;
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
            int busNumber = 1
        )
        {
            return CreateI2cBus(busNumber, new Frequency((int)busSpeed, Frequency.UnitType.Hertz));
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
            if (busNumber == 3)
            {
                return CreateI2cBus(Pins.I2C3_SCL, Pins.I2C3_SDA, frequency);
            }
            return CreateI2cBus(Pins.I2C1_SCL, Pins.I2C1_SDA, frequency);
        }

        protected override int GetI2CBusNumberForPins(IPin clock, IPin data)
        {
            if (clock.Name == (Pins as F7CoreComputeV2.Pinout)?.I2C3_SCL.Name)
            {
                return 3;
            }
            if (clock.Name == (Pins as F7CoreComputeV2.Pinout)?.I2C1_SCL.Name)
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
            Units.Frequency speed,
            int busNumber = 3
        )
        {
            switch (busNumber)
            {
                case 2:
                    return CreateSpiBus(Pins.ESP_CLK, Pins.ESP_COPI, Pins.ESP_CIPO, speed);
                case 3:
                    return CreateSpiBus(Pins.SPI3_SCK, Pins.SPI3_COPI, Pins.SPI3_CIPO, speed);
                case 5:
                    return CreateSpiBus(Pins.SPI5_SCK, Pins.SPI5_COPI, Pins.SPI5_CIPO, speed);
            }

            throw new Exception("Unsupported SPI bus number");
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
            else if (clock.Name == (Pins as F7CoreComputeV2.Pinout)?.SPI3_SCK.Name)
            {
                return 3;
            }
            else if (clock.Name == (Pins as F7CoreComputeV2.Pinout)?.SPI5_SCK.Name)
            {
                return 5;
            }

            // this is an unsupported bus, but will get caught elsewhere
            return -1;
        }

        public override IPwmPort CreatePwmPort(
            IPin pin,
            Frequency frequency,
            float dutyCycle = IPwmOutputController.DefaultPwmDutyCycle,
            bool inverted = false)
        {
            return PwmPort.From(pin, this.IoController, frequency, dutyCycle, inverted, false);
        }
    }
}
