using Meadow.Hardware;
using Microsoft.Win32;
using System;

namespace Meadow;

/// <summary>
/// Provides information about a Windows device, implementing <see cref="IDeviceInformation"/>.
/// </summary>
public class WindowsDeviceInformation : IDeviceInformation
{
    /// <inheritdoc/>
    public string DeviceName { get; set; }

    /// <summary>
    /// Gets the model name of the Windows device, typically the OS platform and version.
    /// </summary>
    public string Model { get; }

    /// <summary>
    /// Gets the <see cref="MeadowPlatform"/> for this device, which is always <see cref="MeadowPlatform.Windows"/>.
    /// </summary>
    public MeadowPlatform Platform => MeadowPlatform.Windows;

    /// <summary>
    /// Gets the processor type or name, as retrieved from the Windows Registry.
    /// </summary>
    public string ProcessorType { get; }

    /// <summary>
    /// Gets the processor serial number, which is unavailable on Windows. Always returns "Unknown".
    /// </summary>
    public string ProcessorSerialNumber => "Unknown";

    /// <summary>
    /// Gets a unique identifier (GUID) for this Windows device, retrieved from the Windows Registry.
    /// </summary>
    public string UniqueID { get; }

    /// <summary>
    /// Gets the coprocessor type. Windows devices do not have a Meadow coprocessor, so this is "None".
    /// </summary>
    public string CoprocessorType => "None";

    /// <summary>
    /// Gets the version of the coprocessor OS, which is not applicable in Windows. Always returns <see langword="null"/>.
    /// </summary>
    public string? CoprocessorOSVersion => null;

    /// <summary>
    /// Gets the operating system version string, as reported by <see cref="Environment.OSVersion"/>.
    /// </summary>
    public string OSVersion => Environment.OSVersion.ToString();

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsDeviceInformation"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor is internal to ensure instances are created only within the library. It reads
    /// device information from the Windows operating system and registry.
    /// </remarks>
    internal WindowsDeviceInformation()
    {
        DeviceName = Environment.MachineName;
        Model = $"{Environment.OSVersion.Platform} {Environment.OSVersion.Version.ToString(2)}";

        using (var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0\"))
        {
            ProcessorType = (key?.GetValue("ProcessorNameString")?.ToString() ?? "Unknown").Trim();
        }
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography\"))
        {
            UniqueID = (key?.GetValue("MachineGuid")?.ToString() ?? "Unknown").Trim();
        }
    }
}
