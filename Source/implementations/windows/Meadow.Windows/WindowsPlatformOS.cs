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
/// Windows implementation of the <see cref="IPlatformOS"/> interface.
/// </summary>
public class WindowsPlatformOS : IPlatformOS
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
    public event EventHandler<WakeSource>? AfterWake = null;

    /// <summary>
    /// Event raised when an external storage device event occurs.
    /// </summary>
    public event ExternalStorageEventHandler ExternalStorageEvent = default!;

    /// <inheritdoc/>
    public event EventHandler<MeadowSystemErrorInfo>? MeadowSystemError;

    /// <summary>
    /// Gets the current operating system version on Windows.
    /// </summary>
    public string OSVersion { get; }

    /// <summary>
    /// Gets the OS build date. This may be set to a default if unknown.
    /// </summary>
    public string OSBuildDate { get; }

    /// <summary>
    /// Gets the version of the .NET runtime in use.
    /// </summary>
    public string RuntimeVersion { get; }

    /// <summary>
    /// Gets or sets the command line arguments provided when the Meadow application was launched.
    /// </summary>
    public string[]? LaunchArguments { get; private set; }

    /// <inheritdoc/>
    public IPlatformOS.FileSystemInfo FileSystem { get; }

    /// <summary>
    /// Default constructor for the <see cref="WindowsPlatformOS"/> object.
    /// </summary>
    internal WindowsPlatformOS()
    {
        OSVersion = Environment.OSVersion.ToString();
        OSBuildDate = "Unknown";
        RuntimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        FileSystem = new WindowsFileSystemInfo();
    }

    /// <summary>
    /// Initialize the <see cref="WindowsPlatformOS"/> instance using the specified
    /// device capabilities and command line arguments.
    /// </summary>
    /// <param name="capabilities">
    /// A <see cref="DeviceCapabilities"/> object that may be used to configure or
    /// limit behavior based on device constraints.
    /// </param>
    /// <param name="args">
    /// The command line arguments provided when the Meadow application was launched.
    /// </param>
    public void Initialize(DeviceCapabilities capabilities, string[]? args)
    {
        // TODO: deal with capabilities
        LaunchArguments = args;
    }

    /// <summary>
    /// Gets the names of all available serial ports on the platform.
    /// </summary>
    /// <returns>An array of <see cref="SerialPortName"/> objects available on Windows.</returns>
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
                // convert to PEM if needed
                pkFileContent = ExecuteWindowsCommandLine("ssh-keygen", $"-e -m pem -f {pkFile}");
            }

            return pkFileContent;
        }
    }

    /// <summary>
    /// Executes a Windows command line process in the background and returns the standard output.
    /// </summary>
    /// <param name="command">The command or process to launch.</param>
    /// <param name="args">The arguments to pass to the command.</param>
    /// <returns>A string containing the command's standard output.</returns>
    private string ExecuteWindowsCommandLine(string command, string args)
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

    /// <inheritdoc/>
    public void Reset()
    {
        MeadowOS.TerminateRun();
    }


    // TODO: implement everything below here

    /// <inheritdoc/>
    public AllocationInfo GetMemoryAllocationInfo() => throw new NotImplementedException();

    /// <summary>
    /// Gets or sets the string that shows which pins are reserved, if any.
    /// </summary>
    /// <remarks>On Windows, this is always an empty string.</remarks>
    public string ReservedPins => string.Empty;

    /// <summary>
    /// Gets a collection of external storage devices.
    /// </summary>
    /// <remarks>This is not implemented on Windows.</remarks>
    public IEnumerable<IExternalStorage> ExternalStorage => throw new NotImplementedException();

    /// <summary>
    /// Gets the Network Time Protocol (NTP) client.
    /// </summary>
    /// <remarks>This is not implemented on Windows.</remarks>
    public INtpClient NtpClient => throw new NotImplementedException();

    /// <summary>
    /// Gets a value indicating whether the system should reboot on an unhandled exception.
    /// </summary>
    /// <remarks>This is not implemented on Windows.</remarks>
    public bool RebootOnUnhandledException => throw new NotImplementedException();

    /// <summary>
    /// Gets the initialization timeout in milliseconds.
    /// </summary>
    /// <remarks>This is not implemented on Windows.</remarks>
    public uint InitializationTimeout => throw new NotImplementedException();

    /// <summary>
    /// Gets a value indicating whether the network should start automatically.
    /// </summary>
    /// <remarks>This is not implemented on Windows.</remarks>
    public bool AutomaticallyStartNetwork => throw new NotImplementedException();

    /// <summary>
    /// Gets the type of network connection currently selected.
    /// </summary>
    /// <remarks>This is not implemented on Windows.</remarks>
    public IPlatformOS.NetworkConnectionType SelectedNetwork => throw new NotImplementedException();

    /// <summary>
    /// Gets a value indicating whether SD storage is supported.
    /// </summary>
    /// <remarks>This is not implemented on Windows.</remarks>
    public bool SdStorageSupported => throw new NotImplementedException();

    /// <summary>
    /// Gets an array of configured NTP servers.
    /// </summary>
    /// <remarks>This is not implemented on Windows.</remarks>
    public string[] NtpServers => throw new NotImplementedException();

    /// <summary>
    /// Gets the CPU temperature, if supported by the platform.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Always thrown on Windows.</exception>
    /// <returns>Temperature of the CPU, if supported.</returns>
    public Temperature GetCpuTemperature()
    {
        throw new PlatformNotSupportedException();
    }

    /// <inheritdoc/>
    public DigitalStorage GetPrimaryDiskSpaceInUse()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the platform OS clock to the specified <see cref="DateTime"/>.
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to which the clock will be set.
    /// </param>
    /// <exception cref="PlatformNotSupportedException">Always thrown on Windows.</exception>
    public void SetClock(DateTime dateTime)
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Gets a configuration value associated with the specified <see cref="IPlatformOS.ConfigurationValues"/> key.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the configuration value to return.
    /// </typeparam>
    /// <param name="item">
    /// An <see cref="IPlatformOS.ConfigurationValues"/> enum value identifying the configuration item.
    /// </param>
    /// <returns>The configuration value.</returns>
    /// <exception cref="NotImplementedException">Always thrown on Windows.</exception>
    public T GetConfigurationValue<T>(IPlatformOS.ConfigurationValues item) where T : struct
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Registers a peripheral for Sleep/Wake awareness.
    /// </summary>
    /// <param name="peripheral">The peripheral that requires sleep/wake notifications.</param>
    /// <exception cref="NotImplementedException">Always thrown on Windows.</exception>
    public void RegisterForSleep(ISleepAwarePeripheral peripheral)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets a configuration value associated with the specified <see cref="IPlatformOS.ConfigurationValues"/> key.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the configuration value to set.
    /// </typeparam>
    /// <param name="item">
    /// An <see cref="IPlatformOS.ConfigurationValues"/> enum value identifying the configuration item.
    /// </param>
    /// <param name="value">The value to store for this configuration key.</param>
    /// <exception cref="NotImplementedException">Always thrown on Windows.</exception>
    public void SetConfigurationValue<T>(IPlatformOS.ConfigurationValues item, T value) where T : struct
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Places the system into a low-power Sleep state for the specified duration.
    /// </summary>
    /// <param name="duration">
    /// The <see cref="TimeSpan"/> duration for which the system should sleep.
    /// </param>
    /// <exception cref="NotImplementedException">Always thrown on Windows.</exception>
    public void Sleep(TimeSpan duration)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves an array of processor utilization values.
    /// </summary>
    /// <returns>
    /// An array of integers representing processor load across cores.
    /// </returns>
    /// <exception cref="NotImplementedException">Always thrown on Windows.</exception>
    public int[] GetProcessorUtilization()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves an array of storage information objects representing attached or available storage devices.
    /// </summary>
    /// <returns>
    /// An array of <see cref="IStorageInformation"/> objects with storage details.
    /// </returns>
    /// <exception cref="NotImplementedException">Always thrown on Windows.</exception>
    public IStorageInformation[] GetStorageInformation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the server certificate validation mode for SSL/TLS connections.
    /// </summary>
    /// <param name="authmode">The desired validation mode.</param>
    /// <exception cref="NotImplementedException">Always thrown on Windows.</exception>
    public void SetServerCertificateValidationMode(ServerCertificateValidationMode authmode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Places the system into a low-power Sleep state, configured to wake on an interrupt.
    /// </summary>
    /// <param name="interruptPin">The pin to monitor for interrupts.</param>
    /// <param name="interruptMode">The condition(s) that will trigger a wake.</param>
    /// <param name="resistorMode">Pull-up, pull-down, or none.</param>
    /// <exception cref="NotImplementedException">Always thrown on Windows.</exception>
    public void Sleep(IPin interruptPin, InterruptMode interruptMode, ResistorMode resistorMode = ResistorMode.Disabled)
    {
        throw new NotImplementedException();
    }
}
