using System;
using System.Runtime.InteropServices;
using Meadow.Core;
using System.Text;

namespace Meadow.Devices
{
    public partial class F7Micro
    {
        /// <summary>
        /// Describes the device information values
        /// </summary>
        public struct DeviceInfoValues
        {
          public string DeviceName;
          public string Product;
          public string Model;
          public string OsVersion;
          public string BuildDate;
          public string ProcessorType;
          public string McuUniqueId;
          public string McuSerialNumb;
          public string CoProcType;
          public string CoProcFirmwareVersion;
          public string MonoVersion;
        };
        
        // Keep in sync with 
        /// <summary>
        /// Describes the Elements in the Device Info string[]
        /// </summary>
        // This must remain in sync with the 'C' code in
        // Meadow.OS\nuttx\configs\stm32f777zit6-meadow\src\meadow-upd.c
        // function upd_handle_dev_info_request()
        private enum DeviceInfoOffset
        {
          DeviceName,
          Product,
          Model,
          OsVersion,
          BuildDate,
          ProcessorType,
          McuUniqueId,
          McuSerialNumb,
          CoProcType,
          CoProcFirmwareVersion,
          MonoVersion
        };

        public DeviceInfoValues GetDeviceInformation()
        {
            // Make the request
            var devInfo = new DeviceInfoValues();
            byte[] strBuffer = new byte[512];
            GCHandle returnGcHandle = GCHandle.Alloc(strBuffer, GCHandleType.Pinned);

            // Object to contain data
            Interop.Nuttx.UpdDeviceInfo rqst = new Interop.Nuttx.UpdDeviceInfo()
            {
               devInfoBuffer = returnGcHandle.AddrOfPinnedObject(),
               devInfoBufLen = strBuffer.Length,
               devInfoRetLen = 0
            };

            //  Make the request
            int ret = UPD.Ioctl(Interop.Nuttx.UpdIoctlFn.GetDeviceInfo, ref rqst);
            if(ret < 0)
              return devInfo;   // Error

            // Returns a single long string containing the device information
            string infoStr = Encoding.ASCII.GetString(strBuffer, 0, rqst.devInfoRetLen);

            // Split the ETX (0x03) delimited string
            string[] splitStr = infoStr.Split((char) 0x03);

            // Populate the structure
            devInfo.DeviceName            = splitStr[(int)DeviceInfoOffset.DeviceName];
            devInfo.Product               = splitStr[(int)DeviceInfoOffset.Product];
            devInfo.Model                 = splitStr[(int)DeviceInfoOffset.Model];
            devInfo.OsVersion             = splitStr[(int)DeviceInfoOffset.OsVersion];
            devInfo.BuildDate             = splitStr[(int)DeviceInfoOffset.BuildDate];
            devInfo.ProcessorType         = splitStr[(int)DeviceInfoOffset.ProcessorType];
            devInfo.McuUniqueId           = splitStr[(int)DeviceInfoOffset.McuUniqueId];
            devInfo.McuSerialNumb         = splitStr[(int)DeviceInfoOffset.McuSerialNumb];
            devInfo.CoProcType            = splitStr[(int)DeviceInfoOffset.CoProcType];
            devInfo.CoProcFirmwareVersion = splitStr[(int)DeviceInfoOffset.CoProcFirmwareVersion];
            devInfo.MonoVersion           = splitStr[(int)DeviceInfoOffset.MonoVersion];

            returnGcHandle.Free();
            return devInfo;
        }
    }
}