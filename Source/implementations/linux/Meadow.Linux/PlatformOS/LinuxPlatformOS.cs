using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using static Meadow.Resolver;

namespace Meadow;

/// <summary>
/// Provides a general implementation for the Meadow platform to run on Posix/Linux based devices
/// </summary>
public class LinuxPlatformOS : IPlatformOS
{
#pragma warning disable CS0067 // The event 'NmCliNetworkAdapter.NetworkConnecting' is never used
    /// <summary>
    /// Event raised before a software reset
    /// </summary>
    public event PowerTransitionHandler BeforeReset = delegate { };
    /// <summary>
    /// Event raised before Sleep mode
    /// </summary>
    public event PowerTransitionHandler BeforeSleep = delegate { };
    /// <inheritdoc/>
    public event EventHandler<WakeSource>? AfterWake;
    /// <summary>
    /// Event raised when an external storage device event occurs.
    /// </summary>
    public event ExternalStorageEventHandler ExternalStorageEvent = delegate { };
    /// <inheritdoc/>
    public event EventHandler<MeadowSystemErrorInfo>? MeadowSystemError;
#pragma warning restore CS0067 // The event 'NmCliNetworkAdapter.NetworkConnecting' is never used

    /// <summary>
    /// The command line arguments provided when the Meadow application was launched
    /// </summary>
    public string[]? LaunchArguments { get; private set; }

    /// <summary>
    /// Gets the OS version.
    /// </summary>
    /// <returns>OS version.</returns>
    public virtual string OSVersion { get; private set; } = string.Empty;
    /// <summary>
    /// Gets the OS build date.
    /// </summary>
    /// <returns>OS build date.</returns>
    public virtual string OSBuildDate { get; private set; } = string.Empty;
    /// <summary>
    /// Get the current .NET runtime version being used to execute the application.
    /// </summary>
    /// <returns>Mono version.</returns>
    public virtual string RuntimeVersion { get; }

    internal static CancellationTokenSource AppAbort = new();

    /// <inheritdoc/>
    public INtpClient NtpClient { get; private set; } = default!;

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
            OSVersion = proc!.StandardOutput.ReadToEnd().Trim();
        }
        catch (Exception ex)
        {
            Log.Debug($"Unable to parse lsb_release: {ex.Message}");
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
            OSBuildDate = proc!.StandardOutput.ReadToEnd().Trim();
        }
        catch (Exception ex)
        {
            Log.Debug($"Unable to parse uname: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the name of all available serial ports on the platform
    /// </summary>
    /// <returns></returns>
    public virtual SerialPortName[] GetSerialPortNames()
    {
        return SerialPort.GetPortNames().Select(n =>
            new SerialPortName(n.Substring(n.LastIndexOf('/') + 1), n, Resolver.Device))
        .ToArray();
    }

    /// <inheritdoc/>
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
        var ts = Interop.Timespec.From(dateTime);
        var result = Interop.clock_settime(Interop.Clock.REALTIME, ref ts);

        if (result != 0)
        {
            var err = Marshal.GetLastWin32Error();

            string errorMessage;

            if (err == 1)
            {
                // this is a permissions issue
                errorMessage = "Application does not have permissions to set the clock.  Try running \"sudo setcap 'cap_sys_time=ep' $DOTNET_ROOT/dotnet\"";
            }
            else
            {
                errorMessage = $"Failed to set clock.  Error code {err}";
            }

            throw new NativeException(errorMessage, err);
        }

        // synchronize the system time to the hardware clock (not necessarily an RTC)
        SetHwClock(dateTime);
    }

    private unsafe void SetHwClock(DateTime dateTime)
    {
        var rtcTime = Interop.Rtc_time.From(dateTime);
        var fd = Interop.open("/dev/rtc", Interop.DriverFlags.O_RDONLY);
        if (fd != 0)
        {
            // possible the platform doesn't have an hwclock, if so we just abandon the call
            Log.Debug($"Call to open /dev/rtc yielded {Marshal.GetLastWin32Error()}");
            return;
        }

        var gch = GCHandle.Alloc(rtcTime, GCHandleType.Pinned);
        try
        {
            var result = Interop.ioctl(fd, Interop.Ioctl.RTC_SET_TIME, gch.AddrOfPinnedObject());

            if (result != 0)
            {
                var err = Marshal.GetLastWin32Error();

                string errorMessage;

                if (err == 1)
                {
                    // this is a permissions issue
                    errorMessage = "Application does not have permissions to set the hwclock.  Try running \"sudo setcap 'cap_sys_time=ep' $DOTNET_ROOT/dotnet\"";
                }
                else
                {
                    errorMessage = $"Failed to set hwclock.  Error code {err}";
                }

                throw new NativeException(errorMessage, err);
            }
        }
        finally
        {
            gch.Free();
            Interop.close(fd);
        }
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

    /// <inheritdoc/>
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
    public AllocationInfo GetMemoryAllocationInfo() => throw new NotImplementedException();

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
