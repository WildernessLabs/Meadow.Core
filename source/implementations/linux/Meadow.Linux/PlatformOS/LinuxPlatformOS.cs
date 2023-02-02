using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Meadow
{
    public class LinuxPlatformOS : IPlatformOS
    {
        public virtual string OSVersion { get; private set; }
        public virtual string OSBuildDate { get; private set; }
        public virtual string RuntimeVersion { get; }

        internal static CancellationTokenSource AppAbort = new();

        public event PowerTransitionHandler BeforeReset;
        public event PowerTransitionHandler BeforeSleep;
        public event PowerTransitionHandler AfterWake;
        public event ExternalStorageEventHandler ExternalStorageEvent;

        public INtpClient NtpClient { get; private set; }

        internal LinuxPlatformOS()
        {
            RuntimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        }

        public void Initialize()
        {
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

        public virtual SerialPortName[] GetSerialPortNames()
        {
            return SerialPort.GetPortNames().Select(n =>
                new SerialPortName(n, n))
            .ToArray();
        }

        public void Initialize(DeviceCapabilities capabilities)
        {
            // TODO: deal with capabilities
        }

        public string FileSystemRoot => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".meadow");


        public virtual Temperature GetCpuTemperature()
        {
            throw new PlatformNotSupportedException();
        }

        public bool RebootOnUnhandledException => false;

        public uint InitializationTimeout => throw new NotImplementedException();

        public IEnumerable<IExternalStorage> ExternalStorage => throw new NotImplementedException();

        public bool AutomaticallyStartNetwork => throw new NotImplementedException();

        public IPlatformOS.NetworkConnectionType SelectedNetwork => throw new NotImplementedException();

        public bool SdStorageSupported => throw new NotImplementedException();

        public T GetConfigurationValue<T>(IPlatformOS.ConfigurationValues item) where T : struct
        {
            throw new NotImplementedException();
        }

        public void SetConfigurationValue<T>(IPlatformOS.ConfigurationValues item, T value) where T : struct
        {
            throw new NotImplementedException();
        }

        public void Reset()
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
    }
}
