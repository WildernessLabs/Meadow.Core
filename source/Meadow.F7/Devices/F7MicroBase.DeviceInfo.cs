using System;
using System.Runtime.InteropServices;
using Meadow.Core;
using System.Text;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public abstract partial class F7MicroBase
    {
        /// <summary>
        /// Describes the device information values
        /// </summary>
        public struct DeviceInformation
        {
          public string DeviceName;
          public Configuration.HardwareVersion Product;
          public string Model;
          public string OsVersion;
          public string BuildDate;
          public string ProcessorType;
          public string UniqueId;
          public string SerialNumber;
          public string CoprocessorType;
          public string CoprocessorFirmwareVersion;
          public string MonoVersion;
        };

        /// <summary>
        /// Get the device information.
        /// </summary>
        /// <returns>DeviceInformation structure.</returns>
        public DeviceInformation GetDeviceInformation()
        {
            var devInfo = new DeviceInformation();

            devInfo.DeviceName = Configuration.GetDeviceName();
            devInfo.Product = Configuration.GetProduct();
            devInfo.Model = Configuration.GetModel();
            devInfo.OsVersion = Configuration.GetOsVersion();
            devInfo.BuildDate = Configuration.GetBuildDate();
            devInfo.ProcessorType = Configuration.GetProcessorType();
            devInfo.UniqueId = Configuration.GetUniqueId();
            devInfo.SerialNumber = Configuration.GetSerialNumber();
            devInfo.CoprocessorType = Configuration.GetCoprocessorType();
            devInfo.CoprocessorFirmwareVersion = Configuration.GetCoprocessorFirmwareVersion();
            devInfo.MonoVersion = Configuration.GetMonoVersion();

            return devInfo;
        }
    }
}