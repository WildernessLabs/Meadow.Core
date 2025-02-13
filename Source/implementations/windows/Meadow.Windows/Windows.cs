using Meadow.Foundation.Displays;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using System;

namespace Meadow;

/// <summary>
/// Represents a Meadow device running on a Windows platform.
/// </summary>
public class Windows : IMeadowDevice, IPixelDisplayProvider
{
    private readonly Lazy<WindowsNetworkAdapterCollection> _networkAdapters;

    /// <inheritdoc/>
    public IPlatformOS PlatformOS { get; }

    /// <inheritdoc/>
    public DeviceCapabilities Capabilities { get; private set; }

    /// <inheritdoc/>
    public IDeviceInformation Information { get; private set; }

    /// <inheritdoc/>
    public INetworkAdapterCollection NetworkAdapters => _networkAdapters.Value;

    /// <summary>
    /// Creates a new instance of the <see cref="Windows"/> class.
    /// </summary>
    public Windows()
    {
        PlatformOS = new WindowsPlatformOS();
        _networkAdapters = new Lazy<WindowsNetworkAdapterCollection>(
            new WindowsNetworkAdapterCollection());
    }

    /// <inheritdoc/>
    public void Initialize(MeadowPlatform detectedPlatform)
    {
        // TODO: populate actual capabilities
        Capabilities = new DeviceCapabilities(
            new AnalogCapabilities(false, null),
            new NetworkCapabilities(false, false),
            new StorageCapabilities(false));

        // TODO: populate this with appropriate data
        Information = new WindowsDeviceInformation();
    }

    /// <summary>
    /// Creates a new resizable pixel display (window) for this Windows environment.
    /// </summary>
    /// <param name="width">Optional width of the display.</param>
    /// <param name="height">Optional height of the display.</param>
    /// <returns>
    /// An <see cref="IResizablePixelDisplay"/> for the specified dimensions,
    /// or 320x240 by default.
    /// </returns>
    public IResizablePixelDisplay CreateDisplay(int? width, int? height)
    {
        return new SilkDisplay(width ?? 320, height ?? 240);
    }

    /// <summary>
    /// Creates an I2C bus with the specified bus number.
    /// </summary>
    /// <param name="busNumber">The bus number.</param>
    /// <returns>This method always throws <see cref="NotSupportedException"/> on Windows.</returns>
    /// <exception cref="NotSupportedException">Thrown on all Windows platforms without an IO Expander.</exception>
    public II2cBus CreateI2cBus(int busNumber = 0)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <summary>
    /// Creates an I2C bus with the specified bus number and speed.
    /// </summary>
    /// <param name="busNumber">The bus number.</param>
    /// <param name="busSpeed">The I2C bus speed.</param>
    /// <returns>This method always throws <see cref="NotSupportedException"/> on Windows.</returns>
    /// <exception cref="NotSupportedException">Thrown on all Windows platforms without an IO Expander.</exception>
    public II2cBus CreateI2cBus(int busNumber, I2cBusSpeed busSpeed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <summary>
    /// Creates an I2C bus with the specified pins and bus speed.
    /// </summary>
    /// <param name="pins">An array of pins for the bus.</param>
    /// <param name="busSpeed">The I2C bus speed.</param>
    /// <returns>This method always throws <see cref="NotSupportedException"/> on Windows.</returns>
    /// <exception cref="NotSupportedException">Thrown on all Windows platforms without an IO Expander.</exception>
    public II2cBus CreateI2cBus(IPin[] pins, I2cBusSpeed busSpeed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <summary>
    /// Creates an I2C bus with the specified clock and data pins.
    /// </summary>
    /// <param name="clock">Clock pin.</param>
    /// <param name="data">Data pin.</param>
    /// <param name="busSpeed">The I2C bus speed.</param>
    /// <returns>This method always throws <see cref="NotSupportedException"/> on Windows.</returns>
    /// <exception cref="NotSupportedException">Thrown on all Windows platforms without an IO Expander.</exception>
    public II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(int busNumber, Frequency speed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <summary>
    /// Creates an SPI bus using the specified clock, MOSI, and MISO pins.
    /// </summary>
    /// <param name="clock">Clock pin.</param>
    /// <param name="mosi">MOSI (Master Out, Slave In) pin.</param>
    /// <param name="miso">MISO (Master In, Slave Out) pin.</param>
    /// <param name="config">SPI clock configuration.</param>
    /// <returns>This method always throws <see cref="NotSupportedException"/> on Windows.</returns>
    /// <exception cref="NotSupportedException">Thrown on all Windows platforms without an IO Expander.</exception>
    public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, SpiClockConfiguration config)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <summary>
    /// Creates an SPI bus using the specified clock, MOSI, and MISO pins.
    /// </summary>
    /// <param name="clock">Clock pin.</param>
    /// <param name="mosi">MOSI (Master Out, Slave In) pin.</param>
    /// <param name="miso">MISO (Master In, Slave Out) pin.</param>
    /// <param name="speed">The desired bus speed.</param>
    /// <returns>This method always throws <see cref="NotSupportedException"/> on Windows.</returns>
    /// <exception cref="NotSupportedException">Thrown on all Windows platforms without an IO Expander.</exception>
    public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, Frequency speed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <summary>
    /// Creates a digital input port using the specified pin, interrupt mode, resistor mode, and debounce settings.
    /// </summary>
    /// <param name="pin">The pin for the digital input.</param>
    /// <param name="interruptMode">The interrupt mode (e.g., change, rising, etc.).</param>
    /// <param name="resistorMode">The internal resistor mode (pull-up, pull-down, or none).</param>
    /// <param name="debounceDuration">The amount of time for debouncing.</param>
    /// <param name="glitchDuration">The glitch filter duration.</param>
    /// <returns>This method always throws <see cref="NotSupportedException"/> on Windows.</returns>
    /// <exception cref="NotSupportedException">Thrown on all Windows platforms without an IO Expander.</exception>
    public IDigitalInputPort CreateDigitalInputPort(
        IPin pin,
        InterruptMode interruptMode,
        ResistorMode resistorMode,
        TimeSpan debounceDuration,
        TimeSpan glitchDuration)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <summary>
    /// Creates an analog input port on the specified pin.
    /// </summary>
    /// <param name="pin">The pin for the analog input.</param>
    /// <param name="sampleCount">Number of samples to average per reading.</param>
    /// <param name="sampleInterval">The interval between samples.</param>
    /// <param name="voltageReference">Reference voltage for the input measurement.</param>
    /// <returns>This method always throws <see cref="NotSupportedException"/> on Windows.</returns>
    /// <exception cref="NotSupportedException">Thrown on all Windows platforms without an IO Expander.</exception>
    public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <summary>
    /// Creates a digital output port on the specified pin.
    /// </summary>
    /// <param name="pin">The pin for the output.</param>
    /// <param name="initialState">The initial state of the output (true = high, false = low).</param>
    /// <param name="initialOutputType">The output type (e.g., push-pull, open-drain).</param>
    /// <returns>This method always throws <see cref="NotSupportedException"/> on Windows.</returns>
    /// <exception cref="NotSupportedException">Thrown on all Windows platforms without an IO Expander.</exception>
    public IDigitalOutputPort CreateDigitalOutputPort(
        IPin pin,
        bool initialState = false,
        OutputType initialOutputType = OutputType.PushPull)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public IDigitalSignalAnalyzer CreateDigitalSignalAnalyzer(IPin pin, bool captureDutyCycle)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <summary>
    /// Creates a serial port with the specified configuration parameters, using a port name string.
    /// </summary>
    /// <param name="portName">The name of the serial port (e.g., "COM3").</param>
    /// <param name="baudRate">The baud rate for communication.</param>
    /// <param name="dataBits">Number of data bits in each character.</param>
    /// <param name="parity">The parity scheme.</param>
    /// <param name="stopBits">Number of stop bits.</param>
    /// <param name="readBufferSize">The size of the read buffer in bytes.</param>
    /// <returns>An <see cref="ISerialPort"/> for communication.</returns>
    /// <exception cref="ArgumentException">Thrown if the specified port name cannot be found.</exception>
    public ISerialPort CreateSerialPort(
        string portName,
        int baudRate = 9600,
        int dataBits = 8,
        Parity parity = Parity.None,
        StopBits stopBits = StopBits.One,
        int readBufferSize = 1024)
    {
        if (PlatformOS.GetSerialPortName(portName) is { } name)
        {
            return CreateSerialPort(name, baudRate, dataBits, parity, stopBits, readBufferSize);
        }

        throw new ArgumentException($"Port name '{portName}' not found");
    }

    /// <summary>
    /// Creates a serial port with the specified configuration parameters, using a <see cref="SerialPortName"/>.
    /// </summary>
    /// <param name="portName">The <see cref="SerialPortName"/> identifying the port.</param>
    /// <param name="baudRate">The baud rate for communication.</param>
    /// <param name="dataBits">Number of data bits in each character.</param>
    /// <param name="parity">The parity scheme.</param>
    /// <param name="stopBits">Number of stop bits.</param>
    /// <param name="readBufferSize">The size of the read buffer in bytes.</param>
    /// <returns>An <see cref="ISerialPort"/> for communication.</returns>
    public ISerialPort CreateSerialPort(
        SerialPortName portName,
        int baudRate = 9600,
        int dataBits = 8,
        Parity parity = Parity.None,
        StopBits stopBits = StopBits.One,
        int readBufferSize = 1024)
    {
        return new WindowsSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
    }

    /// <summary>
    /// Creates a serial message port using a suffix delimiter to separate messages.
    /// </summary>
    /// <param name="portName">The <see cref="SerialPortName"/> identifying the port.</param>
    /// <param name="suffixDelimiter">The delimiter sequence marking the end of a message.</param>
    /// <param name="preserveDelimiter">True to keep the delimiter in the resulting message data.</param>
    /// <param name="baudRate">The baud rate for communication.</param>
    /// <param name="dataBits">Number of data bits in each character.</param>
    /// <param name="parity">The parity scheme.</param>
    /// <param name="stopBits">Number of stop bits.</param>
    /// <param name="readBufferSize">The size of the read buffer in bytes.</param>
    /// <returns>An <see cref="ISerialMessagePort"/> configured with suffix-delimited messages.</returns>
    public ISerialMessagePort CreateSerialMessagePort(
        SerialPortName portName,
        byte[] suffixDelimiter,
        bool preserveDelimiter,
        int baudRate = 9600,
        int dataBits = 8,
        Parity parity = Parity.None,
        StopBits stopBits = StopBits.One,
        int readBufferSize = 512)
    {
        var port = CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        return SerialMessagePort.From(port, suffixDelimiter, preserveDelimiter);
    }

    /// <summary>
    /// Creates a serial message port using a prefix delimiter and fixed message length to separate messages.
    /// </summary>
    /// <param name="portName">The <see cref="SerialPortName"/> identifying the port.</param>
    /// <param name="prefixDelimiter">The delimiter sequence marking the start of a message.</param>
    /// <param name="preserveDelimiter">True to keep the delimiter in the resulting message data.</param>
    /// <param name="messageLength">The expected fixed length of each message.</param>
    /// <param name="baudRate">The baud rate for communication.</param>
    /// <param name="dataBits">Number of data bits in each character.</param>
    /// <param name="parity">The parity scheme.</param>
    /// <param name="stopBits">Number of stop bits.</param>
    /// <param name="readBufferSize">The size of the read buffer in bytes.</param>
    /// <returns>An <see cref="ISerialMessagePort"/> configured with prefix-delimited, fixed-length messages.</returns>
    public ISerialMessagePort CreateSerialMessagePort(
        SerialPortName portName,
        byte[] prefixDelimiter,
        bool preserveDelimiter,
        int messageLength,
        int baudRate = 9600,
        int dataBits = 8,
        Parity parity = Parity.None,
        StopBits stopBits = StopBits.One,
        int readBufferSize = 512)
    {
        var port = CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        return SerialMessagePort.From(port, prefixDelimiter, preserveDelimiter, messageLength);
    }

    // TODO: implement everything below here




    /// <summary>
    /// Event raised when a network connection is established.
    /// </summary>
    public event NetworkConnectionHandler NetworkConnected = default!;

    /// <summary>
    /// Event raised when a network connection is disconnected or lost.
    /// </summary>
    public event NetworkDisconnectionHandler NetworkDisconnected = default!;

    /// <summary>
    /// Creates a bi-directional port with interrupt capability.
    /// </summary>
    /// <param name="pin">The pin for the port.</param>
    /// <param name="initialState">The initial state (true = high, false = low).</param>
    /// <param name="interruptMode">The interrupt mode.</param>
    /// <param name="resistorMode">The internal resistor mode (pull-up, pull-down, or none).</param>
    /// <param name="initialDirection">Initial port direction (input or output).</param>
    /// <param name="debounceDuration">Debounce duration.</param>
    /// <param name="glitchDuration">Glitch filter duration.</param>
    /// <param name="output">Output type (push-pull, open-drain, etc.).</param>
    /// <returns>This method always throws <see cref="NotImplementedException"/> in Windows.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public IBiDirectionalPort CreateBiDirectionalPort(
        IPin pin,
        bool initialState,
        InterruptMode interruptMode,
        ResistorMode resistorMode,
        PortDirectionType initialDirection,
        TimeSpan debounceDuration,
        TimeSpan glitchDuration,
        OutputType output = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a counter on the specified pin.
    /// </summary>
    /// <param name="pin">The pin for counting pulses.</param>
    /// <param name="edge">The edge detection mode (rising, falling, etc.).</param>
    /// <returns>This method always throws <see cref="NotImplementedException"/> in Windows.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public ICounter CreateCounter(IPin pin, InterruptMode edge)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a PWM port on the specified pin.
    /// </summary>
    /// <param name="pin">The pin to use for PWM output.</param>
    /// <param name="frequency">The frequency of the PWM signal.</param>
    /// <param name="dutyCycle">The duty cycle of the PWM signal (0.0 - 1.0).</param>
    /// <param name="invert">Indicates whether the signal should be inverted.</param>
    /// <returns>This method always throws <see cref="NotImplementedException"/> in Windows.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves information about the battery, if applicable.
    /// </summary>
    /// <returns>This method always throws <see cref="NotImplementedException"/> in Windows.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public BatteryInfo GetBatteryInfo()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves the specified pin by name.
    /// </summary>
    /// <param name="name">The name of the pin.</param>
    /// <returns>This method always throws <see cref="NotImplementedException"/> in Windows.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public IPin GetPin(string name)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the system clock to the specified date/time.
    /// </summary>
    /// <param name="dateTime">The new date/time value.</param>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public void SetClock(DateTime dateTime)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Enables the watchdog timer for the specified timeout period.
    /// </summary>
    /// <param name="timeout">The watchdog timeout period.</param>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public void WatchdogEnable(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Resets or "kicks" the watchdog timer.
    /// </summary>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public void WatchdogReset()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a digital input port using the specified pin and resistor mode.
    /// </summary>
    /// <param name="pin">The pin to configure as a digital input.</param>
    /// <param name="resistorMode">Pull-up, pull-down, or none.</param>
    /// <returns>This method always throws <see cref="NotImplementedException"/> in Windows.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a digital interrupt port using the specified settings.
    /// </summary>
    /// <param name="pin">The pin to configure.</param>
    /// <param name="interruptMode">The interrupt condition(s) to detect.</param>
    /// <param name="resistorMode">Pull-up, pull-down, or none.</param>
    /// <param name="debounceDuration">Debounce duration to filter out noise.</param>
    /// <param name="glitchDuration">Glitch filter duration.</param>
    /// <returns>This method always throws <see cref="NotImplementedException"/> in Windows.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a bi-directional interrupt port using the specified settings.
    /// </summary>
    /// <param name="pin">The pin to configure.</param>
    /// <param name="initialState">Initial output state (true = high, false = low).</param>
    /// <param name="interruptMode">Interrupt condition(s) to detect.</param>
    /// <param name="resistorMode">Pull-up, pull-down, or none.</param>
    /// <param name="initialDirection">Initial port direction (input or output).</param>
    /// <param name="debounceDuration">Debounce duration to filter out noise.</param>
    /// <param name="glitchDuration">Glitch filter duration.</param>
    /// <param name="output">The output type (push-pull, open-drain, etc.).</param>
    /// <returns>This method always throws <see cref="NotImplementedException"/> in Windows.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(
        IPin pin,
        bool initialState,
        InterruptMode interruptMode,
        ResistorMode resistorMode,
        PortDirectionType initialDirection,
        TimeSpan debounceDuration,
        TimeSpan glitchDuration,
        OutputType output = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a simple bi-directional port with an initial state.
    /// </summary>
    /// <param name="pin">The pin to configure.</param>
    /// <param name="initialState">Initial output state (true = high, false = low).</param>
    /// <returns>This method always throws <see cref="NotImplementedException"/> in Windows.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates an analog input array using the specified pins.
    /// </summary>
    /// <param name="pins">The pins to configure for analog input.</param>
    /// <returns>This method always throws <see cref="NotImplementedException"/> in Windows.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
    {
        throw new NotImplementedException();
    }
}
