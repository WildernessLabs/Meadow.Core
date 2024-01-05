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

public class MacPlatformOS : IPlatformOS
{
    /// <summary>
    /// Event raised before a software reset
    /// </summary>
    public event PowerTransitionHandler BeforeReset = default!;
    /// <summary>
    /// Event raised before Sleep mode
    /// </summary>
    public event PowerTransitionHandler BeforeSleep = default!;
    /// <summary>
    /// Event raised after returning from Sleep mode
    /// </summary>
    public event PowerTransitionHandler AfterWake = default!;
    /// <summary>
    /// Event raised when an external storage device event occurs.
    /// </summary>
    public event ExternalStorageEventHandler ExternalStorageEvent = default!;

    /// <summary>
    /// Gets the OS version.
    /// </summary>
    /// <returns>OS version.</returns>
    public string OSVersion { get; }
    /// <summary>
    /// Gets the OS build date.
    /// </summary>
    /// <returns>OS build date.</returns>
    public string OSBuildDate { get; }
    /// <summary>
    /// Get the current .NET runtime version being used to execute the application.
    /// </summary>
    /// <returns>Mono version.</returns>
    public string RuntimeVersion { get; }

    /// <summary>
    /// The command line arguments provided when the Meadow application was launched
    /// </summary>
    public string[]? LaunchArguments { get; private set; }

    /// <inheritdoc/>
    public IPlatformOS.FileSystemInfo FileSystem { get; }

    /// <summary>
    /// Default constructor for the WindowsPlatformOS object.
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
    /// </summary>
    /// <param name="capabilities"></param>
    /// <param name="args">The command line arguments provided when the Meadow application was launched</param>
    public void Initialize(DeviceCapabilities capabilities, string[]? args)
    {
        // TODO: deal with capabilities
    }

    /// <summary>
    /// Gets the name of all available serial ports on the platform
    /// </summary>
    /// <returns>A list of available serial port names</returns>
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
                pkFileContent = ExecuteWindowsCommandLine("ssh-keygen", $"-e -m pem -f {pkFile}");
            }

            return pkFileContent;
        }
    }

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
        // Create an Aes object
        // with the specified key and IV.
        using Aes aesAlg = Aes.Create();
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








    // TODO: implement everything below here

    public string ReservedPins => string.Empty;
    public IEnumerable<IExternalStorage> ExternalStorage => throw new NotImplementedException();
    public INtpClient NtpClient => throw new NotImplementedException();
    public bool RebootOnUnhandledException => throw new NotImplementedException();
    public uint InitializationTimeout => throw new NotImplementedException();
    public bool AutomaticallyStartNetwork => throw new NotImplementedException();
    public IPlatformOS.NetworkConnectionType SelectedNetwork => throw new NotImplementedException();
    public bool SdStorageSupported => throw new NotImplementedException();

    public Temperature GetCpuTemperature()
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Sets the platform OS clock
    /// </summary>
    /// <param name="dateTime"></param>
    public void SetClock(DateTime dateTime)
    {
        throw new PlatformNotSupportedException();
    }

    public T GetConfigurationValue<T>(IPlatformOS.ConfigurationValues item) where T : struct
    {
        throw new NotImplementedException();
    }


    public void RegisterForSleep(ISleepAwarePeripheral peripheral)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void SetConfigurationValue<T>(IPlatformOS.ConfigurationValues item, T value) where T : struct
    {
        throw new NotImplementedException();
    }

    public void Sleep(TimeSpan duration)
    {
        throw new NotImplementedException();
    }

    public int[] GetProcessorUtilization()
    {
        throw new NotImplementedException();
    }

    public IStorageInformation[] GetStorageInformation()
    {
        throw new NotImplementedException();
    }

    public void SetServerCertificateValidationMode(ServerCertificateValidationMode authmode)
    {
        throw new NotImplementedException();
    }

    public void Sleep(IPin interruptPin, InterruptMode interruptMode, ResistorMode resistorMode = ResistorMode.Disabled)
    {
        throw new NotImplementedException();
    }
}
