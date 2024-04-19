using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;

namespace Meadow.Simulation;

public class SimulatedPlatformOS : IPlatformOS
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
    /// <inheritdoc/>
    public event EventHandler<int>? OsException;

    /// <summary>
    /// Event raised when an external storage device event occurs.
    /// </summary>
    public event ExternalStorageEventHandler ExternalStorageEvent = delegate { };

    public string OSVersion => "0.1";

    /// <summary>
    /// The command line arguments provided when the Meadow application was launched
    /// </summary>
    public string[]? LaunchArguments { get; private set; }

    /// <summary>
    /// Get the current .NET runtime version being used to execute the application.
    /// </summary>
    /// <returns>Mono version.</returns>
    public string RuntimeVersion { get; }

    public IPlatformOS.FileSystemInfo FileSystem { get; }

    internal SimulatedPlatformOS()
    {
        FileSystem = new SimulatedFileSystemInfo();
        RuntimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
    }

    public virtual SerialPortName[] GetSerialPortNames()
    {
        return SerialPort.GetPortNames().Select(n =>
            new SerialPortName(n, n, Resolver.Device))
        .ToArray();
    }

    /// <summary>
    /// Initialize the SimulatedPlatformOS instance.
    /// </summary>
    /// <param name="capabilities"></param>
    /// <param name="args">The command line arguments provided when the Meadow application was launched</param>
    public void Initialize(DeviceCapabilities capabilities, string[]? args)
    {
        // TODO: deal with capabilities

        LaunchArguments = args;
    }


    public string ReservedPins => string.Empty;

    public string FileSystemRoot => System.AppDomain.CurrentDomain.BaseDirectory;

    public string OSBuildDate => throw new NotImplementedException();

    public bool RebootOnUnhandledException => throw new NotImplementedException();

    public uint InitializationTimeout => throw new NotImplementedException();

    public INtpClient NtpClient => throw new NotImplementedException();

    public IEnumerable<IExternalStorage> ExternalStorage => throw new NotImplementedException();

    public bool AutomaticallyStartNetwork => throw new NotImplementedException();

    public IPlatformOS.NetworkConnectionType SelectedNetwork => throw new NotImplementedException();

    public bool SdStorageSupported => throw new NotImplementedException();

    public T GetConfigurationValue<T>(IPlatformOS.ConfigurationValues item) where T : struct
    {
        throw new NotImplementedException();
    }

    public Temperature GetCpuTemperature()
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

    public void RegisterForSleep(ISleepAwarePeripheral peripheral)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public byte[] AesDecrypt(byte[] encryptedValue, byte[] key, byte[] iv)
    {
        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            // Create a decryptor to perform the stream transform.
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (var msEncrypt = new MemoryStream(encryptedValue))
            using (var csDecrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
            using (var msDecrypt = new MemoryStream())
            {
                csDecrypt.CopyTo(msDecrypt);
                return msDecrypt.ToArray();
            }
        }
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

    public string? GetPublicKeyInPemFormat()
    {
        throw new NotImplementedException();
    }

    public byte[] RsaDecrypt(byte[] encryptedValue, string privateKeyPem)
    {
        throw new NotImplementedException();
    }

    public void Sleep(IPin interruptPin, InterruptMode interruptMode, ResistorMode resistorMode = ResistorMode.Disabled)
    {
        throw new NotImplementedException();
    }

    public DigitalStorage GetPrimaryDiskSpaceInUse()
    {
        throw new NotImplementedException();
    }

    public string[] NtpServers => throw new NotImplementedException();

}
