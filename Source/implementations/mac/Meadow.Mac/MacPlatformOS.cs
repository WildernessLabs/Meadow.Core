using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Meadow;

/// <summary>
/// Represents the macOS platform operating system.
/// </summary>
public class MacPlatformOS : IPlatformOS
{
    /// <summary>
    /// Event raised before a software reset.
    /// </summary>
    public event PowerTransitionHandler BeforeReset = default!;

    /// <summary>
    /// Event raised before Sleep mode.
    /// </summary>
    public event PowerTransitionHandler BeforeSleep = default!;

    /// <inheritdoc/>
    public event EventHandler<WakeSource> AfterWake = default!;

    /// <summary>
    /// Event raised when an external storage device event occurs.
    /// </summary>
    public event ExternalStorageEventHandler ExternalStorageEvent = default!;

    /// <inheritdoc/>
    public event EventHandler<MeadowSystemErrorInfo>? MeadowSystemError;

    /// <summary>
    /// Gets the OS version on macOS as a string, typically from <see cref="Environment.OSVersion"/>.
    /// </summary>
    public string OSVersion { get; }

    /// <summary>
    /// Gets the OS build date. If unknown, it defaults to "Unknown".
    /// </summary>
    public string OSBuildDate { get; }

    /// <summary>
    /// Gets the .NET runtime version in use on macOS.
    /// </summary>
    public string RuntimeVersion { get; }

    /// <summary>
    /// The command line arguments provided when the Meadow application was launched.
    /// </summary>
    public string[]? LaunchArguments { get; private set; }

    /// <inheritdoc/>
    public IPlatformOS.FileSystemInfo FileSystem { get; }

    /// <summary>
    /// Default constructor for the WindowsPlatformOS object.
    /// (Note: This is actually the macOS implementation; the documentation label is kept for consistency.)
    /// </summary>
    internal MacPlatformOS()
    {
        OSVersion = Environment.OSVersion.ToString();
        OSBuildDate = "Unknown";
        RuntimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        FileSystem = new MacFileSystemInfo();
    }

    /// <summary>
    /// Initialize the WindowsPlatformOS instance.
    /// (Note: This is actually the macOS implementation; the documentation label is kept for consistency.)
    /// </summary>
    /// <param name="capabilities">Device capabilities (unused on macOS).</param>
    /// <param name="args">The command line arguments provided when the Meadow application was launched.</param>
    public void Initialize(DeviceCapabilities capabilities, string[]? args)
    {
        // TODO: deal with capabilities
        LaunchArguments = args;
    }

    /// <summary>
    /// Gets the names of all available serial ports on macOS.
    /// </summary>
    /// <returns>
    /// A list of <see cref="SerialPortName"/> objects representing available serial ports.
    /// </returns>
    public SerialPortName[] GetSerialPortNames()
    {
        return SerialPort.GetPortNames().Select(n =>
            new SerialPortName(n, n, Resolver.Device))
        .ToArray();
    }

    /// <inheritdoc/>
    public string GetPublicKeyInPemFormat()
    {
        var sshFolder = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh"));

        if (!sshFolder.Exists)
        {
            throw new Exception("SSH folder not found");
        }
        else
        {
            var pkFile = Path.Combine(sshFolder.FullName, "id_rsa.pub");
            if (!File.Exists(pkFile))
            {
                throw new Exception("Public key not found");
            }

            var pkFileContent = File.ReadAllText(pkFile);
            if (!pkFileContent.Contains("BEGIN RSA PUBLIC KEY", StringComparison.OrdinalIgnoreCase))
            {
                // need to convert
                pkFileContent = ExecuteCommandLine("ssh-keygen", $"-e -m pem -f {pkFile}");
            }

            return pkFileContent;
        }
    }

    /// <summary>
    /// Executes the specified command-line process on macOS, capturing the standard output.
    /// </summary>
    /// <param name="command">The shell command to run.</param>
    /// <param name="args">The command arguments or flags to pass.</param>
    /// <returns>A string containing the standard output of the executed command.</returns>
    private string ExecuteCommandLine(string command, string args)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);

        process?.WaitForExit();

        return process?.StandardOutput.ReadToEnd() ?? string.Empty;
    }

    /// <inheritdoc/>
    public byte[] RsaDecrypt(byte[] encryptedValue, string privateKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);
        return rsa.Decrypt(encryptedValue, RSAEncryptionPadding.Pkcs1);
    }

    /// <inheritdoc/>
    public byte[] AesDecrypt(byte[] encryptedValue, byte[] key, byte[] iv)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(encryptedValue);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        var plain = srDecrypt.ReadToEnd();

        return Encoding.UTF8.GetBytes(plain);
    }

    // TODO: implement everything below here

    /// <inheritdoc/>
    public AllocationInfo GetMemoryAllocationInfo() => throw new NotImplementedException();

    /// <summary>
    /// Gets a string representing the pins reserved by the system. On macOS, this is always an empty string.
    /// </summary>
    public string ReservedPins => string.Empty;

    /// <summary>
    /// Gets a collection of external storage devices. This is not implemented on macOS.
    /// </summary>
    public IEnumerable<IExternalStorage> ExternalStorage => throw new NotImplementedException();

    /// <summary>
    /// Gets an <see cref="INtpClient"/> for network time operations. Not implemented on macOS.
    /// </summary>
    public INtpClient NtpClient => throw new NotImplementedException();

    /// <summary>
    /// Indicates whether the system should reboot on an unhandled exception. Not implemented on macOS.
    /// </summary>
    public bool RebootOnUnhandledException => throw new NotImplementedException();

    /// <summary>
    /// Gets the initialization timeout in milliseconds. Not implemented on macOS.
    /// </summary>
    public uint InitializationTimeout => throw new NotImplementedException();

    /// <summary>
    /// Gets a value indicating whether the network should start automatically. Not implemented on macOS.
    /// </summary>
    public bool AutomaticallyStartNetwork => throw new NotImplementedException();

    /// <summary>
    /// Gets the type of network connection currently selected. Not implemented on macOS.
    /// </summary>
    public IPlatformOS.NetworkConnectionType SelectedNetwork => throw new NotImplementedException();

    /// <summary>
    /// Indicates if SD storage is supported. Not implemented on macOS.
    /// </summary>
    public bool SdStorageSupported => throw new NotImplementedException();

    /// <summary>
    /// Gets an array of configured NTP servers. Not implemented on macOS.
    /// </summary>
    public string[] NtpServers => throw new NotImplementedException();

    /// <summary>
    /// Attempts to retrieve the CPU temperature on macOS. 
    /// Due to limitations on macOS, this method cannot directly access the hardware sensor.
    /// It currently returns AbsoluteZero to indicate the temperature is unavailable. 
    /// </summary>
    /// <returns>A <see cref="Temperature"/> object set to AbsoluteZero.</returns>
    public Temperature GetCpuTemperature()
    {
        return Temperature.AbsoluteZero;
    }

    /// <summary>
    /// Sets the platform OS clock to the specified <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="dateTime">The date/time value to set.</param>
    /// <exception cref="PlatformNotSupportedException">Always thrown on macOS.</exception>
    public void SetClock(DateTime dateTime)
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Retrieves a configuration value of type <typeparamref name="T"/> associated with 
    /// the given <see cref="IPlatformOS.ConfigurationValues"/> item. Not implemented on macOS.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="item">The configuration key.</param>
    /// <returns>The requested value.</returns>
    /// <exception cref="NotImplementedException">Always thrown on macOS.</exception>
    public T GetConfigurationValue<T>(IPlatformOS.ConfigurationValues item) where T : struct
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Registers a peripheral for sleep/wake awareness. Not implemented on macOS.
    /// </summary>
    /// <param name="peripheral">The peripheral that needs to be sleep-aware.</param>
    /// <exception cref="NotImplementedException">Always thrown on macOS.</exception>
    public void RegisterForSleep(ISleepAwarePeripheral peripheral)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Resets the system. Not implemented on macOS.
    /// </summary>
    /// <exception cref="NotImplementedException">Always thrown on macOS.</exception>
    public void Reset()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets a configuration value of type <typeparamref name="T"/> for the given 
    /// <see cref="IPlatformOS.ConfigurationValues"/> item. Not implemented on macOS.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="item">The configuration key.</param>
    /// <param name="value">The value to store.</param>
    /// <exception cref="NotImplementedException">Always thrown on macOS.</exception>
    public void SetConfigurationValue<T>(IPlatformOS.ConfigurationValues item, T value) where T : struct
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Places the system into a low-power Sleep state for the specified duration. Not implemented on macOS.
    /// </summary>
    /// <param name="duration">The amount of time to sleep.</param>
    /// <exception cref="NotImplementedException">Always thrown on macOS.</exception>
    public void Sleep(TimeSpan duration)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves an array of processor utilization metrics. Not implemented on macOS.
    /// </summary>
    /// <returns>An array of integers representing processor utilization.</returns>
    /// <exception cref="NotImplementedException">Always thrown on macOS.</exception>
    public int[] GetProcessorUtilization()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves information about available storage on the system. Not implemented on macOS.
    /// </summary>
    /// <returns>An array of <see cref="IStorageInformation"/> objects.</returns>
    /// <exception cref="NotImplementedException">Always thrown on macOS.</exception>
    public IStorageInformation[] GetStorageInformation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the server certificate validation mode for network connections. Not implemented on macOS.
    /// </summary>
    /// <param name="authmode">The desired validation mode.</param>
    /// <exception cref="NotImplementedException">Always thrown on macOS.</exception>
    public void SetServerCertificateValidationMode(ServerCertificateValidationMode authmode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Places the system into a low-power Sleep state, configured to wake on an interrupt. Not implemented on macOS.
    /// </summary>
    /// <param name="interruptPin">The pin to monitor for a wake-triggering interrupt.</param>
    /// <param name="interruptMode">The interrupt condition(s) to detect.</param>
    /// <param name="resistorMode">Specifies pull-up, pull-down, or none.</param>
    /// <exception cref="NotImplementedException">Always thrown on macOS.</exception>
    public void Sleep(IPin interruptPin, InterruptMode interruptMode, ResistorMode resistorMode = ResistorMode.Disabled)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public DigitalStorage GetPrimaryDiskSpaceInUse()
    {
        var drives = DriveInfo.GetDrives();

        // Assuming the primary drive is of type 'Fixed'
        DriveInfo? primaryDrive = drives.FirstOrDefault(drive => drive.DriveType == DriveType.Fixed);

        if (primaryDrive != null)
        {
            long bytesAvailable = primaryDrive.TotalSize - primaryDrive.AvailableFreeSpace;
            return new DigitalStorage(bytesAvailable, DigitalStorage.UnitType.Bytes);
        }

        // If the primary drive is not found, returns 0 bytes.
        return new DigitalStorage(0, DigitalStorage.UnitType.Bytes);
    }
}
