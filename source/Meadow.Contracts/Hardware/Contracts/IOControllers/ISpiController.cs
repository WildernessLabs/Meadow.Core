using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for devices who expose `ISpiBus(es)`.
    /// </summary>
    public interface ISpiController : IDigitalOutputController
    {
        /// <summary>
        /// The default SPI Bus speed, in kHz, used when speed parameters are not provided
        /// </summary>
        public const int DefaultSpiBusSpeed = 375;

        /// <summary>
        /// Creates a SPI bus instance for the requested control pins and bus speed
        /// </summary>
        /// <param name="clock">The IPin instance to use as the bus clock</param>
        /// <param name="mosi">The IPin instance to use for data transmit (master out/slave in)</param>
        /// <param name="miso">The IPin instance to use for data receive (master in/slave out)</param>
        /// <param name="config">The bus clock configuration parameters</param>
        /// <returns>An instance of an IISpiBus</returns>
        public ISpiBus CreateSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso,
            SpiClockConfiguration config
        );

        /// <summary>
        /// Creates a SPI bus instance for the requested control pins and bus speed
        /// </summary>
        /// <param name="clock">The IPin instance to use as the bus clock</param>
        /// <param name="mosi">The IPin instance to use for data transmit (master out/slave in)</param>
        /// <param name="miso">The IPin instance to use for data receive (master in/slave out)</param>
        /// <param name="speed">The bus speed</param>
        /// <returns>An instance of an IISpiBus</returns>
        ISpiBus CreateSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso,
            Units.Frequency speed
        );

    }
}
