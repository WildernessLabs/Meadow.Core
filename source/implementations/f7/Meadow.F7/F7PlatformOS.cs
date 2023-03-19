using Meadow.Hardware;
using Meadow.Units;
using System;

namespace Meadow;

/// <summary>
/// Provides a STM32F7-specific implementation for the Meadow platform
/// </summary>
public partial class F7PlatformOS : IPlatformOS
{
    /// <summary>
    /// The command line arguments provided when the Meadow application was launched
    /// </summary>
    public string[]? LaunchArguments { get; private set; }

    /// <summary>
    /// NTP client.
    /// </summary>
    public INtpClient NtpClient { get; }

    /// <summary>
    /// Default constructor for the F7PlatformOS objects.
    /// </summary>
    public F7PlatformOS()
    {
        NtpClient = new NtpClient();
    }

    /// <summary>
    /// Get the current CPU temperature (Not supported on F7).
    /// </summary>
    /// <exception cref="NotSupportedException">Method is not supported on the F7 platform.</exception>
    public Temperature GetCpuTemperature()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Initialize the F7PlatformOS instance.
    /// </summary>
    /// <param name="capabilities"></param>
    /// <param name="args">The command line arguments provided when the Meadow application was launched</param>
    public void Initialize(DeviceCapabilities capabilities, string[]? args)
    {
        LaunchArguments = args;
        InitializeStorage(capabilities.Storage);
    }

    /// <summary>
    /// Gets the name of all available serial ports on the platform
    /// </summary>
    /// <returns></returns>
    public SerialPortName[] GetSerialPortNames()
    {
        return new SerialPortName[]
        {
            new SerialPortName("COM1", "ttyS0"),
            new SerialPortName("COM4", "ttyS1")
        };
    }
}
