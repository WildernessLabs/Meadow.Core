using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;

namespace Meadow
{
    public class WindowsPlatformOS : IPlatformOS
    {
        public event ExternalStorageEventHandler ExternalStorageEvent;
        public event PowerTransitionHandler BeforeReset;
        public event PowerTransitionHandler BeforeSleep;
        public event PowerTransitionHandler AfterWake;

        public string FileSystemRoot { get; private set; }

        public string OSVersion { get; }
        public string OSBuildDate { get; }
        public string RuntimeVersion { get; }

        internal WindowsPlatformOS()
        {
            OSVersion = Environment.OSVersion.ToString();
            OSBuildDate = "Unknown";
            RuntimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        }

        public void Initialize(DeviceCapabilities capabilities)
        {
            // create the Meadow root folder
            var di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Meadow"));
            if (!di.Exists)
            {
                di.Create();
            }

            FileSystemRoot = di.FullName;
        }

        public SerialPortName[] GetSerialPortNames()
        {
            return SerialPort.GetPortNames().Select(n =>
                new SerialPortName(n, n))
            .ToArray();
        }

        public Temperature GetCpuTemperature()
        {
            throw new PlatformNotSupportedException();
        }








        // TODO: implement everything below here

        public IEnumerable<IExternalStorage> ExternalStorage => throw new NotImplementedException();
        public INtpClient NtpClient => throw new NotImplementedException();
        public bool RebootOnUnhandledException => throw new NotImplementedException();
        public uint InitializationTimeout => throw new NotImplementedException();
        public bool AutomaticallyStartNetwork => throw new NotImplementedException();
        public IPlatformOS.NetworkConnectionType SelectedNetwork => throw new NotImplementedException();
        public bool SdStorageSupported => throw new NotImplementedException();

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
    }
}
