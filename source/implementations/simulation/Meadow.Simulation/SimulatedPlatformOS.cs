using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;

namespace Meadow.Simulation
{
    public class SimulatedFileSystemInfo : IPlatformOS.FileSystemInfo
    {
        public override IEnumerable<IExternalStorage> ExternalStorage => throw new NotImplementedException();

        public override string FileSystemRoot { get; }

        internal SimulatedFileSystemInfo()
        {
            // create the Meadow root folder
            var di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Meadow"));
            if (!di.Exists)
            {
                di.Create();
            }

            FileSystemRoot = di.FullName;
        }
    }

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
        /// <summary>
        /// Event raised after returning from Sleep mode
        /// </summary>
        public event PowerTransitionHandler AfterWake = delegate { };
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

        public byte[] RsaDecrypt(byte[] encryptedValue)
        {
            var rsa = RSA.Create();
            return rsa.Decrypt(encryptedValue, RSAEncryptionPadding.Pkcs1);
        }

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
                using (var msDecrypt = new MemoryStream(encryptedValue))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    var buffer = new byte[csDecrypt.Length];
                    csDecrypt.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
        }
    }
}
