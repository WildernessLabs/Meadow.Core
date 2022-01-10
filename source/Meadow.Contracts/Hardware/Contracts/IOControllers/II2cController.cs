using Meadow.Units;
using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for devices that expose an `II2cBus`.
    /// </summary>
    public interface II2cController
    {
        /// <summary>
        /// The default I2C Bus speed, in Hz, used when speed parameters are not provided
        /// </summary>
        public static Frequency DefaultI2cBusSpeed = new Frequency(100000, Frequency.UnitType.Hertz);

        /// <summary>
        /// Creates an I2C bus instance for the default pins.
        /// </summary>
        /// <returns>An instance of an I2cBus</returns>
        II2cBus CreateI2cBus(
            int busNumber = 0);

        /// <summary>
        /// Creates an I2C bus instance for the default pins and the requested bus speed
        /// </summary>
        /// <param name="frequency">The bus speed in (in Hz).</param>
        /// <returns>An instance of an I2cBus</returns>
        II2cBus CreateI2cBus(
            int busNumber,
            Frequency frequency
        );

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequency">The bus speed in (in Hz)</param>
        /// <returns>An instance of an I2cBus</returns>
        II2cBus CreateI2cBus(
            IPin[] pins,
            Frequency frequency
        );

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequency">The bus speed in (in Hz)</param>
        /// <returns>An instance of an I2cBus</returns>
        II2cBus CreateI2cBus(
            IPin clock,
            IPin data,
            Frequency frequency
        );
    }
}