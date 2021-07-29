using System;
using System.Runtime.InteropServices;
using Meadow.Core;
using System.Text;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public partial class F7Micro
    {
        /// <summary>
        /// Class allowing access to the configuration of the F7 Micro.
        /// </summary>
        public static class Configuration
        {
            /// <summary>
            /// Enumeration indicating the possible configuration items that can be read / written.
            /// </summary>
            /// <remarks>It is critical that this enum matches the enum in the NuttX file meadow-upd.h.</remarks>
            private enum ConfigurationValues
            {
                DeviceName = 0, Product, Model, OsVersion, BuildDate, ProcessorType, UniqueId, SerialNumber, CoprocessorType,
                CoprocessorFirmwareVersion, MonoVersion
            };

            /// <summary>
            /// Indicate if a read or write operation is to be executed.
            /// </summary>
            private enum Direction { Get = 0, Set = 1 };

            /// <summary>
            /// Get or Set the specified value in the OS configuration.
            /// </summary>
            /// <param name="item">Item to retrieve.</param>
            /// <param name="direction">Indicate if a get or a set operation should be performed.</param>
            /// <param name="buffer">Byte buffer holding the value.</param>
            /// <returns>True if successful, false if there was a problem.</returns>
            private static (bool Result, int Length) GetSetValue(ConfigurationValues item, Direction direction, byte[] buffer)
            {
                bool result = true;
                int length = 0;
                var bufferHandle = default(GCHandle);

                try
                {
                    bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    var request = new Interop.Nuttx.UpdConfigurationValue()
                    {
                        Item = (int) item,
                        Direction = (byte) direction,
                        ValueBufferSize = buffer.Length,
                        ValueBuffer = bufferHandle.AddrOfPinnedObject(),
                        ReturnDataLength = 0
                    };
                    int updResult = UPD.Ioctl(Interop.Nuttx.UpdIoctlFn.GetSetConfigurationValue, ref request);
                    if (updResult == 0)
                    {
                        length = request.ReturnDataLength;
                    }
                    else
                    {
                        Console.WriteLine($"Configuration ioctl failed, result code: {updResult}");
                        result = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Configuration ioctl failed: {ex.Message}");
                    result = false;
                }
                finally
                {
                    if (bufferHandle.IsAllocated)
                    {
                        bufferHandle.Free();
                    }
                }
                return (result, length);
            }

            /// <summary>
            /// Get a string configuration item.
            /// </summary>
            /// <param name="value">Configuration item to read.</param>
            /// <returns>Configuration value if present, String.Empty if no item could be found.</returns>
            private static string GetString(ConfigurationValues item)
            {
                byte[] buffer = new byte[1024];
                string str = String.Empty;

                (bool result, int length) = GetSetValue(item, Direction.Get, buffer);
                if (result && (length > 0))
                {
                    str = Encoding.ASCII.GetString(buffer, 0, length);
                }

                return (str);
            }

            /// <summary>
            /// Set the specified configuration item to the string value.
            /// </summary>
            /// <param name="item">Item to set.</param>
            /// <param name="value"Value to be used.></param>
            /// <returns>True if the configuration value was set, false if there is a problem.</returns>
            private static bool SetString(ConfigurationValues item, string value)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(value);

                (bool result, int length) = GetSetValue(item, Direction.Set, buffer);

                return (result);
            }

            /// <summary>
            /// Get the current device name.
            /// </summary>
            /// <returns>Name of the device.</returns>
            public static string GetDeviceName()
            {
                return (GetString(ConfigurationValues.DeviceName));
            }

            /// <summary>
            /// Set the device name.
            /// </summary>
            /// <param name="deviceName">Name of the device.</param>
            public static void SetDeviceName(string deviceName)
            {
                SetString(ConfigurationValues.DeviceName, deviceName);
            }

            /// <summary>
            /// Get the current product name.
            /// </summary>
            /// <returns>Product name.</returns>
            public static string GetProduct()
            {
                return (GetString(ConfigurationValues.Product));
            }

            /// <summary>
            /// Get the current model name.
            /// </summary>
            /// <returns>Model name.</returns>
            public static string GetModel()
            {
                return (GetString(ConfigurationValues.Model));
            }

            /// <summary>
            /// Get the OS version.
            /// </summary>
            /// <returns>OS version.</returns>
            public static string GetOsVersion()
            {
                return (GetString(ConfigurationValues.OsVersion));
            }

            /// <summary>
            /// Get the OS build date
            /// </summary>
            /// <returns>OS build date.</returns>
            public static string GetBuildDate()
            {
                return (GetString(ConfigurationValues.BuildDate));
            }

            /// <summary>
            /// Get the processor type.
            /// </summary>
            /// <returns>Processor type.</returns>
            public static string GetProcessorType()
            {
                return (GetString(ConfigurationValues.ProcessorType));
            }

            /// <summary>
            /// Get the serial number of the device.
            /// </summary>
            /// <returns>Serial number of the device.</returns>
            public static string GetSerialNumber()
            {
                return (GetString(ConfigurationValues.SerialNumber));
            }

            /// <summary>
            /// Get the unique ID of the micrcontroller.
            /// </summary>
            /// <returns>Unique ID of the microcontroller.</returns>
            public static string GetUniqueId()
            {
                return (GetString(ConfigurationValues.UniqueId));
            }

            /// <summary>
            /// Get the coprocessor type.
            /// </summary>
            /// <returns>Coprocessor type.</returns>
            public static string GetCoprocessorType()
            {
                return (GetString(ConfigurationValues.CoprocessorType));
            }

            /// <summary>
            /// Get the version of the firmware flashed to the coprocessor.
            /// </summary>
            /// <returns>Coprocessor firmware version..</returns>
            public static string GetCoprocessorFirmwareVersion()
            {
                return (GetString(ConfigurationValues.CoprocessorFirmwareVersion));
            }

            /// <summary>
            /// Get the mono version on the device..
            /// </summary>
            /// <returns>Mono version.</returns>
            public static string GetMonoVersion()
            {
                return (GetString(ConfigurationValues.MonoVersion));
            }
        }
    }
}
