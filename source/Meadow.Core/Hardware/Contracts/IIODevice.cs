

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for Meadow devices.
    /// </summary>
    public interface IIODevice//<P> where P : IPinDefinitions
    {
        /// <summary>
        /// Gets the device capabilities.
        /// </summary>
        DeviceCapabilities Capabilities { get; }

        // TODO: consider specializing IIODevice
        IDigitalOutputPort CreateDigitalOutputPort(
            IPin pin,bool initialState = false);

        IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            int debounceDuration = 0,
            int glitchFilterCycleCount = 0
        );

        IBiDirectionalPort CreateBiDirectionalPort(
            IPin pin,
            bool initialState = false,
            bool glitchFilter = false,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input
        );

        IAnalogInputPort CreateAnalogInputPort(
            IPin pin,
            float voltageReference
        );

        IPwmPort CreatePwmPort(
            IPin pin,
            float frequency,
            float dutyCycle,
            bool invert = false
        );

        ISerialPort CreateSerialPort(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096);

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
        /// <param name="speedkHz">The bus speed (in kHz)</param>
        /// <returns>An instance of an IISpiBus</returns>
        ISpiBus CreateSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso,
            long speedkHz
        );

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz)</param>
        /// <returns>An instance of an I2cBus</returns>
        II2cBus CreateI2cBus(
            IPin[] pins,
            int frequencyHz
        );

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz)</param>
        /// <returns>An instance of an I2cBus</returns>
        II2cBus CreateI2cBus(
            IPin clock,
            IPin data,
            int frequencyHz
        );
    }
}
