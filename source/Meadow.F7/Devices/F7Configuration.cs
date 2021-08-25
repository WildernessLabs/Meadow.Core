using System;
using System.Runtime.InteropServices;
using Meadow.Core;
using System.Text;
using Meadow.Devices.Esp32.MessagePayloads;
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
            public enum ConfigurationValues
            {
                DeviceName = 0, Product, Model, OsVersion, BuildDate, ProcessorType, UniqueId, SerialNumber, CoprocessorType,
                CoprocessorFirmwareVersion, MonoVersion,
                AutomaticallyStartNetwork, AutomaticallyReconnect, MaximumNetworkRetryCount, GetTimeAtStartup,
                NtpServer, MacAddress, SoftApMacAddress, DefaultAccessPoint
            };

            /// <summary>
            /// Indicate if a read or write operation is to be executed.
            /// </summary>
            private enum Direction { Get = 0, Set = 1 };

            /// <summary>
            /// Hardware version (product).
            /// </summary>
            public enum HardwareVersion { Unknown = 0, MeadowF7V1 = 1, MeadowF7V2 = 2 };

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
            public static string GetString(ConfigurationValues item)
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
            /// Get an array of bytes from the board configuration
            /// </summary>
            /// <param name="item">Configuration item to read.</param>
            /// <param name="buffer">Byte buffer to hold the data.</param>
            public static void GetByteArray(ConfigurationValues item, byte[] buffer)
            {
                (bool result, int length) = GetSetValue(item, Direction.Get, buffer);

                return;
            }

            /// <summary>
            /// Get an unsigned integer configuration item.
            /// </summary>
            /// <param name="value">Configuration item to read.</param>
            /// <returns>Configuration value if present, 0 if it could not be found.</returns>
            public static UInt32 GetUInt32(ConfigurationValues item)
            {
                byte[] buffer = new byte[4];
                UInt32 ui = 0;

                (bool result, int length) = GetSetValue(item, Direction.Get, buffer);
                if (result && (length == 4))
                {
                    ui = Encoders.ExtractUInt32(buffer, 0);
                }

                return (ui);
            }

            /// <summary>
            /// Get a byte configuration item.
            /// </summary>
            /// <param name="value">Configuration item to read.</param>
            /// <returns>Configuration value if present, 0 if the item count not be found..</returns>
            public static byte GetByte(ConfigurationValues item)
            {
                byte[] buffer = new byte[1];
                byte b = 0;

                (bool result, int length) = GetSetValue(item, Direction.Get, buffer);
                if (result && (length == 1))
                {
                    b = buffer[0];
                }

                return (b);
            }

            /// <summary>
            /// Get a boolean configuration item.
            /// </summary>
            /// <param name="value">Configuration item to read.</param>
            /// <returns>Configuration value if present, false if the item could not be found.</returns>
            public static bool GetBoolean(ConfigurationValues item)
            {
                return (GetByte(item) == 1 ? true : false);
            }

            /// <summary>
            /// Set the specified configuration item to the string value.
            /// </summary>
            /// <param name="item">Item to set.</param>
            /// <param name="value"Value to be used.></param>
            /// <returns>True if the configuration value was set, false if there is a problem.</returns>
            public static bool SetString(ConfigurationValues item, string value)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(value);

                //
                //  We put the terminating NULL at the end of the string in order to help the
                //  underlying C code in the OS.
                //
                byte[] stringBuffer = new byte[buffer.Length + 1];
                Array.Clear(stringBuffer, 0, stringBuffer.Length);
                Array.Copy(buffer, stringBuffer, buffer.Length);

                (bool result, int length) = GetSetValue(item, Direction.Set, stringBuffer);

                return (result);
            }

            /// <summary>
            /// Set the specified configuration item to the given array of bytes.
            /// </summary>
            /// <param name="item">Item to set.</param>
            /// <param name="buffer">Byte buffer to hold the data.</param>
            /// <returns>True if the configuration value was set, false if there is a problem.</returns>
            public static bool SetByteArray(ConfigurationValues item, byte[] buffer)
            {
                (bool result, int length) = GetSetValue(item, Direction.Set, buffer);

                return (result);
            }

            /// <summary>
            /// Set an unsigned integer configuration item.
            /// </summary>
            /// <param name="item">Item to set.</param>
            /// <param name="value"Value to be used.></param>
            /// <returns>True if the configuration value was set, false if there is a problem.</returns>
            public static bool SetUInt32(ConfigurationValues item, UInt32 value)
            {
                byte[] buffer = BitConverter.GetBytes(value);

                (bool result, int length) = GetSetValue(item, Direction.Set, buffer);

                return (result);
            }

            /// <summary>
            /// Set a configuration to the specified byte.
            /// </summary>
            /// <param name="item">Item to set.</param>
            /// <param name="value"Value to be used.></param>
            /// <returns>True if the configuration value was set, false if there is a problem.</returns>
            public static bool SetByte(ConfigurationValues item, byte value)
            {
                byte[] buffer = new byte[1];
                buffer[0] = value;

                (bool result, int length) = GetSetValue(item, Direction.Set, buffer);

                return (result);
            }

            /// <summary>
            /// Set a boolean configuration item.
            /// </summary>
            /// <param name="item">Item to set.</param>
            /// <param name="value"Value to be used.></param>
            /// <returns>True if the configuration value was set, false if there is a problem.</returns>
            public static bool SetBoolean(ConfigurationValues item, bool value)
            {
                byte b = (byte) ((value ? 1 : 0) & 0xff);
                return (SetByte(item, b));
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
            /// Set the name of the device as it will appear on the network.
            /// </summary>
            /// <param name="value">Name of the device.</param>
            /// <returns>True if successful, false otherwise.</returns>
            public static bool SetDeviceName(string value)
            {
                return (SetString(ConfigurationValues.DeviceName, value));
            }

            /// <summary>
            /// Get the current product name.
            /// </summary>
            /// <returns>Product name.</returns>
            public static HardwareVersion GetProduct()
            {
                return((HardwareVersion) GetUInt32(ConfigurationValues.Product));
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
