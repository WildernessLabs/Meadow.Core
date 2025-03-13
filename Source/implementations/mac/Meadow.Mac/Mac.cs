using Meadow.Devices;
using Meadow.Foundation.Displays;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Meadow;

/// <summary>
/// Represents a Mac device running the Meadow OS.
/// </summary>
public class Mac : IMeadowDevice, IPixelDisplayProvider
{
    private readonly Lazy<NativeNetworkAdapterCollection> _networkAdapters;

    /// <inheritdoc/>
    public IPlatformOS PlatformOS { get; }
    /// <inheritdoc/>
    public DeviceCapabilities Capabilities { get; private set; } = default!;
    /// <inheritdoc/>
    public IDeviceInformation Information { get; private set; } = default!;
    /// <inheritdoc/>
    public INetworkAdapterCollection NetworkAdapters => _networkAdapters.Value;

    /// <summary>
    /// Creates a new instance of the Mac class.
    /// </summary>
    public Mac()
    {
        PlatformOS = new MacPlatformOS();
        _networkAdapters = new Lazy<NativeNetworkAdapterCollection>(
            new NativeNetworkAdapterCollection());
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
        Information = new MacDeviceInformation();
    }

    /// <inheritdoc/>
    public IResizablePixelDisplay CreateDisplay(int? width = null, int? height = null)
    {
        return new SilkDisplay(width ?? 320, height ?? 240);
    }

    /// <inheritdoc/>
    public II2cBus CreateI2cBus(int busNumber = 0)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public II2cBus CreateI2cBus(int busNumber, I2cBusSpeed busSpeed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public II2cBus CreateI2cBus(IPin[] pins, I2cBusSpeed busSpeed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(int busNumber, Units.Frequency speed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, SpiClockConfiguration config)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, Frequency speed)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public IDigitalInputPort CreateDigitalInputPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public IObservableAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public IDigitalSignalAnalyzer CreateDigitalSignalAnalyzer(IPin pin, bool captureDutyCycle)
    {
        throw new NotSupportedException("Add an IO Expander to your platform");
    }

    /// <inheritdoc/>
    public ISerialPort CreateSerialPort(string portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
    {
        if (PlatformOS.GetSerialPortName(portName) is { } name)
        {
            return CreateSerialPort(name, baudRate, dataBits, parity, stopBits, readBufferSize);
        }

        throw new ArgumentException($"Port name '{portName}' not found");
    }

    /// <inheritdoc/>
    public ISerialPort CreateSerialPort(SerialPortName portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
    {
        return new MacSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
    }

    /// <inheritdoc/>
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] suffixDelimiter, bool preserveDelimiter, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
    {
        var port = CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        return SerialMessagePort.From(port, suffixDelimiter, preserveDelimiter);
    }

    /// <inheritdoc/>
    public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] prefixDelimiter, bool preserveDelimiter, int messageLength, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
    {
        var port = CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        return SerialMessagePort.From(port, prefixDelimiter, preserveDelimiter, messageLength);
    }






    // TODO: implement everything below here


    /// <inheritdoc/>
    public event NetworkConnectionHandler NetworkConnected = default!;
    /// <inheritdoc/>
    public event NetworkDisconnectionHandler NetworkDisconnected = default!;


    /// <inheritdoc/>
    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ICounter CreateCounter(IPin pin, InterruptMode edge)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Executes a shell command and returns its standard output as a string.
    /// </summary>
    /// <param name="command">The shell command to execute.</param>
    /// <returns>The standard output of the command.</returns>
    private static string ExecuteShellCommand(string command)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = "-c \"" + command + "\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = new Process { StartInfo = processInfo };
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output;
    }

    /// <summary>
    /// Parses the output of a shell command to extract battery information.
    /// </summary>
    /// <param name="output">The output string from the shell command.</param>
    /// <returns>A BatteryInfo object containing the parsed information.</returns>
    private BatteryInfo ParseBatteryInfo(string output)
    {
        BatteryInfo batteryInfo = new BatteryInfo();

        // Extract Voltage (assuming mV)
        var voltageMatch = Regex.Match(output, "\"AppleRawBatteryVoltage\"\\s*=\\s*(\\d+)");
        if (voltageMatch.Success)
        {
            double voltage = double.Parse(voltageMatch.Groups[1].Value) / 1000;
            batteryInfo.Voltage = new Meadow.Units.Voltage(voltage);
        }

        // Extract State of Charge
        var socMatch = Regex.Match(output, "\"CurrentCapacity\"\\s*=\\s*(\\d+)");
        if (socMatch.Success)
        {
            batteryInfo.StateOfCharge = int.Parse(socMatch.Groups[1].Value);
        }

        // Extract TimeToEmpty (assuming minutes)
        var timeToEmptyMatch = Regex.Match(output, "\"TimeRemaining\"\\s*=\\s*(\\d+)");
        if (timeToEmptyMatch.Success)
        {
            int timeToEmptyMinutes = int.Parse(timeToEmptyMatch.Groups[1].Value);
            batteryInfo.TimeToEmpty = TimeSpan.FromMinutes(timeToEmptyMinutes);
        }

        return batteryInfo;
    }

    /// <inheritdoc/>
    public BatteryInfo GetBatteryInfo()
    {
        // ToDo: Check if this command is compatible in Intel-based macOS models
        string command = "ioreg -r -c \"AppleSmartBattery\"";
        string output = ExecuteShellCommand(command);

        BatteryInfo batteryInfo = ParseBatteryInfo(output);

        return batteryInfo;
    }

    /// <inheritdoc/>
    public IPin GetPin(string name)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetClock(DateTime dateTime)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void WatchdogEnable(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void WatchdogReset()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
    {
        throw new NotImplementedException();
    }
}
