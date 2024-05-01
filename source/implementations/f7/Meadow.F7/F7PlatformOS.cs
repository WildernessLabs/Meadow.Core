using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.IO;

namespace Meadow;

/// <summary>
/// Provides a STM32F7-specific implementation for the Meadow platform
/// </summary>
public partial class F7PlatformOS : IPlatformOS
{
    private readonly F7GPIOManager _ioController;

    /// <summary>
    /// The command line arguments provided when the Meadow application was launched
    /// </summary>
    public string[]? LaunchArguments { get; private set; }

    /// <summary>
    /// NTP client.
    /// </summary>
    public INtpClient NtpClient { get; }

    /// <summary>
    /// Default constructor for the F7PlatformOS object.
    /// </summary>
    internal F7PlatformOS(F7GPIOManager ioController)
    {
        _ioController = ioController;

        NtpClient = new NtpClient();
        Resolver.Services.Add(NtpClient);
    }

    /// <summary>
    /// Get the current CPU temperature (Not supported on F7).
    /// </summary>
    /// <exception cref="NotSupportedException">Method is not supported on the F7 platform.</exception>
    public Temperature GetCpuTemperature()
    {
        if (Resolver.Device is F7MicroBase f7)
        {
            return f7.GetProcessorTemperature();
        }

        // should never occur, but makes the compiler happy
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
        FileSystem = new F7FileSystemInfo(capabilities.Storage, SdStorageSupported);
    }

    /// <summary>
    /// Gets the name of all available serial ports on the platform
    /// </summary>
    /// <returns>A list of available serial port names</returns>
    public SerialPortName[] GetSerialPortNames()
    {
        return new SerialPortName[]
        {
            new SerialPortName("COM1", "ttyS0", Resolver.Device),
            new SerialPortName("COM4", "ttyS1", Resolver.Device)
        };
    }

    /// <inheritdoc/>
    public void SetClock(DateTime dateTime)
    {
        var ts = new Core.Interop.Nuttx.timespec
        {
            tv_sec = new DateTimeOffset(dateTime).ToUnixTimeSeconds()
        };

        Core.Interop.Nuttx.clock_settime(Core.Interop.Nuttx.clockid_t.CLOCK_REALTIME, ref ts);
    }

    /// <summary>
    /// Retrieves memory allocation statistics from the OS
    /// </summary>
    public AllocationInfo GetMemoryAllocationInfo()
    {
        return Core.Interop.Nuttx.mallinfo();
    }

    /// <inheritdoc/>
    public int[] GetProcessorUtilization()
    {
        return new[] { 100 - Core.Interop.Nuttx.meadow_idle_monitor_get_value() };
    }


    /// <summary>
    /// Enum representing different server certificate validation error returns.
    /// </summary>
    public enum ServerCertificateValidationError
    {
        /// <summary>
        /// Invalid mode
        /// </summary>
        InvalidMode = -1,
        /// <summary>
        /// Cannot change after initialization
        /// </summary>
        CannotChangeAfterInitialization = -2
    }

    /// <inheritdoc/>
    public void SetServerCertificateValidationMode(ServerCertificateValidationMode authmode)
    {
        Resolver.Log.Trace($"Attempting to set the server certificate validation mode to {authmode}");

        int authModeInt = (int)authmode;
        if (authModeInt < 0 || authModeInt > Enum.GetNames(typeof(ServerCertificateValidationMode)).Length - 1)
        {
            string errorMessage = $"Invalid validation mode: {authModeInt}";
            Resolver.Log.Error($"Invalid validation mode: {authModeInt}");
            throw new ArgumentException(errorMessage);
        }

        int ret = Core.Interop.Nuttx.mono_mbedtls_set_server_cert_authmode(authModeInt);
        if (ret == (int)ServerCertificateValidationError.InvalidMode)
        {
            string errorMessage = $"Invalid validation mode: {authModeInt}";
            Resolver.Log.Error($"Invalid validation mode: {authModeInt}");
            throw new ArgumentException(errorMessage);
        }
        else if (ret == (int)ServerCertificateValidationError.CannotChangeAfterInitialization)
        {
            string errorMessage = $"The server certificate validation mode cannot be changed after the TLS initialization.";
            Resolver.Log.Error(errorMessage);
            throw new Exception(errorMessage);
        }
        else if (ret < 0)
        {
            string errorMessage = $"Error setting validation mode.";
            Resolver.Log.Error(errorMessage);
            throw new Exception(errorMessage);
        }

        Resolver.Log.Trace($"Server certificate validation mode set to {authmode} successfully!");

        return;
    }

    /// <inheritdoc/>
    public DigitalStorage GetPrimaryDiskSpaceInUse()
    {
        DirectoryInfo di = new(Resolver.Device.PlatformOS.FileSystem.FileSystemRoot);

        var usedDiskSpace = DirSize(di);

        return new DigitalStorage(usedDiskSpace, DigitalStorage.UnitType.Bytes);
    }

    private long DirSize(DirectoryInfo d)
    {
        long size = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            size += fi.Length;
        }

        // Add subdirectory sizes.
        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
        {
            size += DirSize(di);
        }

        return size;
    }
}
