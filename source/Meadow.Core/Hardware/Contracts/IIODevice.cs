

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
            float voltageReference = 3.3f
        );

        IPwmPort CreatePwmPort(
            IPin pin,
            float frequency = 100, 
            float dutyCycle = 0.5f,
            bool invert = false
        );

        ISerialPort CreateSerialPort(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096);

        ISpiBus CreateSpiBus(
            IPin[] pins,
            long speed
        );

        ISpiBus CreateSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso,
            long speed
        );

        II2cBus CreateI2cBus(
            IPin[] pins,
            int speed
        );

        II2cBus CreateI2cBus(
            IPin clock,
            IPin data,
            int speed
        );
    }
}
