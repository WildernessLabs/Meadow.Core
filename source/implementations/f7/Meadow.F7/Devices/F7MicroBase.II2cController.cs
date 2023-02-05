using Meadow.Hardware;

namespace Meadow.Devices
{
    public abstract partial class F7MicroBase
    {
        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <returns>An instance of an I2cBus</returns>
        public abstract II2cBus CreateI2cBus(
            int busNumber = 1,
            I2cBusSpeed busSpeed = I2cBusSpeed.Standard
        );

        /// <summary>
        /// Get the I2C bus number for a pair of I2C clock and data pins
        /// </summary>
        /// <param name="clock">The I2C clock pin</param>
        /// <param name="data">The I2C data pin</param>
        /// <returns></returns>
        protected abstract int GetI2CBusNumberForPins(IPin clock, IPin data);

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="pins">An array of two pins holding the I2C clock and data pins</param>
        /// <param name="busSpeed">The bus speed, defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            IPin[] pins,
            I2cBusSpeed busSpeed = I2cBusSpeed.Standard
        )
        {
            return CreateI2cBus(pins[0], pins[1], busSpeed);
        }

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="clock">The I2C clock pin</param>
        /// <param name="data">The I2C data pin</param>
        /// <param name="busSpeed">The bus speed, defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            IPin clock,
            IPin data,
            I2cBusSpeed busSpeed = I2cBusSpeed.Standard
        )
        {
            var bus = I2cBus.From(IoController, clock, data, busSpeed);
            bus.BusNumber = GetI2CBusNumberForPins(clock, data);
            return bus;
        }
    }
}
