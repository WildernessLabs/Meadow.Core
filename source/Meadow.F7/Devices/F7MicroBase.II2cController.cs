using System;
using Meadow.Hardware;
using Meadow.Units;

namespace Meadow.Devices
{
    public abstract partial class F7MicroBase
    {
        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(int busNumber = 0)
        {
            return CreateI2cBus(busNumber, IMeadowDevice.DefaultI2cBusSpeed);
        }

        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
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
        public II2cBus CreateI2cBus(
            int busNumber,
            Frequency frequency
        )
        {
            return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, frequency);
        }

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequency">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            IPin[] pins,
            Frequency frequency
        )
        {
            return CreateI2cBus(pins[0], pins[1], frequency);
        }

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequency">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            IPin clock,
            IPin data,
            Frequency frequency
        )
        {
            return I2cBus.From(this.IoController, clock, data, frequency);
        }
    }
}
