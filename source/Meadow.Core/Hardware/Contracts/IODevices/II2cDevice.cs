using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for devices that expose an `II2cBus`.
    /// </summary>
    public interface II2cDevice
    {
        /// <summary>
        /// The default I2C Bus speed, in Hz, used when speed parameters are not provided
        /// </summary>
        public const int DefaultI2cBusSpeed = 100000;

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz)</param>
        /// <returns>An instance of an I2cBus</returns>
        II2cBus CreateI2cBus(
            IPin[] pins,
            int frequencyHz = DefaultI2cBusSpeed
        );

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz)</param>
        /// <returns>An instance of an I2cBus</returns>
        II2cBus CreateI2cBus(
            IPin clock,
            IPin data,
            int frequencyHz = DefaultI2cBusSpeed
        );
    }
}
