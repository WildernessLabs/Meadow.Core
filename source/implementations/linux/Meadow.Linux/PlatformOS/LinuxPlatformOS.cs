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
using System.Threading;

namespace Meadow;

/// <summary>
/// Provides a general implementation for the Meadow platform to run on Posix/Linux based devices
/// </summary>
public class LinuxPlatformOS : IPlatformOS
{
    /// <summary>
    /// Event raised before a software reset
    /// </summary>
    public event PowerTransitionHandler BeforeReset = delegate { };
    /// <summary>
    /// Event raised before Sleep mode
    /// </summary>
    public event PowerTransitionHandler BeforeSleep = delegate { };
    /// <inheritdoc/>
    public event EventHandler<WakeSource>? AfterWake = null;
    /// <summary>
    /// Event raised when an external storage device event occurs.
    /// </summary>
    public event ExternalStorageEventHandler ExternalStorageEvent = delegate { };
    /// <inheritdoc/>
    public event EventHandler<int>? OsException;

    /// <summary>
    /// The command line arguments provided when the Meadow application was launched
    /// </summary>
    public string[]? LaunchArguments { get; private set; }

    /// <summary>
    /// Gets the OS version.
    /// </summary>
    /// <returns>OS version.</returns>
    public virtual string OSVersion { get; private set; }
    /// <summary>
    /// Gets the OS build date.
    /// </summary>
    /// <returns>OS build date.</returns>
    public virtual string OSBuildDate { get; private set; }
    /// <summary>
    /// Get the current .NET runtime version being used to execute the application.
    /// </summary>
    /// <returns>Mono version.</returns>
    public virtual string RuntimeVersion { get; }

    internal static CancellationTokenSource AppAbort = new();

    /// <inheritdoc/>
    public INtpClient NtpClient { get; private set; }
    /// <inheritdoc/>
    public IPlatformOS.FileSystemInfo FileSystem { get; }

    internal LinuxPlatformOS()
    {
        RuntimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        FileSystem = new LinuxFileSystemInfo();
    }

    /// <summary>
    /// Initialize the LinuxPlatformOS instance.
    /// </summary>
    /// <param name="capabilities"></param>
    /// <param name="args">The command line arguments provided when the Meadow application was launched</param>
    public void Initialize(DeviceCapabilities capabilities, string[]? args)
    {
        // TODO: deal with capabilities

        NtpClient = new LinuxNtpClient();

        try
        {
            var psi = new ProcessStartInfo("/bin/bash", "-c \"lsb_release -d\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var proc = Process.Start(psi);
            OSVersion = proc.StandardOutput.ReadToEnd().Trim();
        }
        catch (Exception ex)
        {
            Resolver.Log.Debug($"Unable to parse lsb_release: {ex.Message}");
        }

        try
        {
            var psi = new ProcessStartInfo("/bin/bash", "-c \"uname -v\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var proc = Process.Start(psi);
            OSBuildDate = proc.StandardOutput.ReadToEnd().Trim();
        }
        catch (Exception ex)
        {
            Resolver.Log.Debug($"Unable to parse uname: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the name of all available serial ports on the platform
    /// </summary>
    /// <returns></returns>
    public virtual SerialPortName[] GetSerialPortNames()
    {
        return SerialPort.GetPortNames().Select(n =>
            new SerialPortName(n, n, Resolver.Device))
        .ToArray();
    }

    public virtual Temperature GetCpuTemperature()
    {
        var fi = new FileInfo("/sys/class/thermal/thermal_zone0/temp");
        if (!fi.Exists)
        {
            throw new PlatformNotSupportedException("CPU temp not available on this OS");
        }

        using (var r = fi.OpenText())
        {
            var data = r.ReadToEnd().Trim();
            var temp = int.Parse(data);
            return new Temperature(temp / 1000d, Temperature.UnitType.Celsius);
        }
    }

    /// <summary>
    /// Sets the platform OS clock
    /// </summary>
    /// <param name="dateTime"></param>
    public void SetClock(DateTime dateTime)
    {
        throw new PlatformNotSupportedException();
    }

    /// <inheritdoc/>
    public byte[] RsaDecrypt(byte[] encryptedValue)
    {
        var rsa = RSA.Create();
        return rsa.Decrypt(encryptedValue, RSAEncryptionPadding.Pkcs1);
    }

    /// <inheritdoc/>
    public byte[] AesDecrypt(byte[] encryptedValue, byte[] key, byte[] iv)
    {
        // Create an Aes object
        // with the specified key and IV.
        using var aesAlg = System.Security.Cryptography.Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;

        // Create a decryptor to perform the stream transform.
        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for decryption.
        using var msDecrypt = new MemoryStream(encryptedValue);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        var plain = srDecrypt.ReadToEnd();

        return Encoding.UTF8.GetBytes(plain);
    }

    /// <inheritdoc/>
    public byte[] RsaDecrypt(byte[] encryptedValue, string privateKeyPem)
    {
        using var rsa = RSA.Create();

        rsa.ImportFromPem(privateKeyPem);

        return rsa.Decrypt(encryptedValue, RSAEncryptionPadding.Pkcs1);
    }

    public string? GetPublicKeyInPemFormat()
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
                pkFileContent = Linux.ExecuteCommandLine("ssh-keygen", $"-e -m pem -f {pkFile}");
            }
            return pkFileContent;
        }
    }

    /// <inheritdoc/>
    public DigitalStorage GetPrimaryDiskSpaceInUse()
    {
        var drive = FileSystem.Drives.FirstOrDefault(d => d.Name == "/");
        if (drive == null) return DigitalStorage.Zero;

        return drive.Size - drive.SpaceAvailable;
    }





    /// <inheritdoc/>
    public string[] NtpServers => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool RebootOnUnhandledException => false;

    /// <inheritdoc/>
    public uint InitializationTimeout => throw new NotImplementedException();

    /// <inheritdoc/>
    public IEnumerable<IExternalStorage> ExternalStorage => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool AutomaticallyStartNetwork => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPlatformOS.NetworkConnectionType SelectedNetwork => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool SdStorageSupported => throw new NotImplementedException();

    /// <inheritdoc/>
    public T GetConfigurationValue<T>(IPlatformOS.ConfigurationValues item) where T : struct
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetConfigurationValue<T>(IPlatformOS.ConfigurationValues item, T value) where T : struct
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Reset()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Sleep(TimeSpan duration)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void RegisterForSleep(ISleepAwarePeripheral peripheral)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public string ReservedPins => string.Empty;

    /// <inheritdoc/>
    public int[] GetProcessorUtilization()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IStorageInformation[] GetStorageInformation()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetServerCertificateValidationMode(ServerCertificateValidationMode authmode)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Sleep(IPin interruptPin, InterruptMode interruptMode, ResistorMode resistorMode = ResistorMode.Disabled)
    {
        throw new NotImplementedException();
    }
}
