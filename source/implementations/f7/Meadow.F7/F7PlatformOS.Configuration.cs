﻿using Meadow.Core;
using Meadow.Devices;
using Meadow.Devices.Esp32.MessagePayloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static Meadow.IPlatformOS;

namespace Meadow;

public partial class F7PlatformOS
{
    private string[]? _ntpServers;

    /// <inheritdoc/>
    public string[] NtpServers
    {
        get
        {
            if (_ntpServers == null)
            {
                var list = new List<string>();

                // The ESP provides no way to get these, so we have to manually parse
                // not using a YAML parser as this is faster
                var path = Path.Combine(FileSystem.FileSystemRoot, "meadow.config.yaml");
                if (File.Exists(path))
                {


                    using var reader = File.OpenText(
                        Path.Combine(FileSystem.FileSystemRoot, "meadow.config.yaml"));

                    // find the 'NtpServers:' line
                    string line;

                    while (true)
                    {
                        line = reader.ReadLine();
                        if (line.Contains("NtpServers:")) break;

                        if (reader.EndOfStream)
                        {
                            return Array.Empty<string>();
                        }
                    }

                    while (true)
                    {
                        line = reader.ReadLine().Trim();
                        if (line.Length > 0 && line[0] == '-')
                        {
                            list.Add(line[2..]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                _ntpServers = list.ToArray();
            }

            return _ntpServers;
        }
    }

    /// <summary>
    /// Gets the specified platform configuration value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public T GetConfigurationValue<T>(ConfigurationValues item) where T : struct
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the specified platform configuration value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="value"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void SetConfigurationValue<T>(ConfigurationValues item, T value) where T : struct
    {
        throw new NotImplementedException();
    }

    //==== public config properties
    /// <summary>
    /// Gets the OS version.
    /// </summary>
    /// <returns>OS version.</returns>
    public string OSVersion => GetString(ConfigurationValues.OsVersion);

    /// <summary>
    /// Gets the OS build date.
    /// </summary>
    /// <returns>OS build date.</returns>
    //TODO: parse as datetime
    public string OSBuildDate => GetString(ConfigurationValues.BuildDate);

    /// <summary>
    /// Get the current .NET runtime version being used to execute the application.
    /// </summary>
    /// <returns>Mono version.</returns>
    public string RuntimeVersion => GetString(ConfigurationValues.MonoVersion);

    /// <summary>
    /// Should the system reboot if an unhandled exception is encountered in the user application?
    /// </summary>
    public bool RebootOnUnhandledException => GetBoolean(ConfigurationValues.RebootOnUnhandledException);

    /// <summary>
    /// Number of seconds the initialization method in the user application is allowed to run before
    /// it is assumed to have crashed.
    /// </summary>
    /// <remarks>A value of 0 indicates an infinite period.</remarks>
    public uint InitializationTimeout => GetUInt(ConfigurationValues.InitializationTimeout);

    /// <summary>
    /// Should a WiFi connection be made on startup.
    /// </summary>
    /// <remarks>This assumes that the default access point is configured through wifi.config.yaml.</remarks>
    public bool AutomaticallyStartNetwork => GetBoolean(ConfigurationValues.AutomaticallyStartNetwork);

    /// <summary>
    /// Which network is selected in meadow.config.yaml.
    /// </summary>
    public NetworkConnectionType SelectedNetwork => (NetworkConnectionType)GetByte(ConfigurationValues.SelectedNetwork);

    /// <summary>
    /// Should SD storage be enabled on this device?
    /// </summary>
    public bool SdStorageSupported => GetBoolean(ConfigurationValues.SdStorageSupported);

    /// <summary>
    /// Names of any pins that should be reserved for OS use.
    /// </summary>
    /// <remarks>This should be a semicolon list of pin names that will be reserved for OS use.</remarks>
    public string ReservedPins => GetString(ConfigurationValues.ReservedPins);


    //==== Configuration internals

    /// <summary>
    /// Indicate if a read or write operation is to be executed.
    /// </summary>
    internal enum Direction { Get = 0, Set = 1 };

    /// <summary>
    /// Get or Set the specified value in the OS configuration.
    /// </summary>
    /// <param name="item">Item to retrieve.</param>
    /// <param name="direction">Indicate if a get or a set operation should be performed.</param>
    /// <param name="buffer">Byte buffer holding the value.</param>
    /// <returns>True if successful, false if there was a problem.</returns>
    internal static (bool Result, int Length) GetSetValue(ConfigurationValues item, Direction direction, byte[] buffer)
    {
        bool result = true;
        int length = 0;
        var bufferHandle = default(GCHandle);

        try
        {
            bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var request = new Interop.Nuttx.UpdConfigurationValue()
            {
                Item = (int)item,
                Direction = (byte)direction,
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
                Resolver.Log.Error($"Configuration ioctl failed, result code: {updResult}");
                result = false;
            }
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Configuration ioctl failed: {ex.Message}");
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
    /// <param name="item">Configuration item to read.</param>
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

        return str;
    }

    /// <summary>
    /// Get an array of bytes from the board configuration
    /// </summary>
    /// <param name="item">Configuration item to read.</param>
    /// <param name="buffer">Byte buffer to hold the data.</param>
    /// TODO: why the byte buffer? do we really need that kind of optimization here?
    /// why not just return the array?
    public static void GetByteArray(ConfigurationValues item, byte[] buffer)
    {
        (bool result, int length) = GetSetValue(item, Direction.Get, buffer);

        return;
    }

    /// <summary>
    /// Get an unsigned integer configuration item.
    /// </summary>
    /// <param name="item">Configuration item to read.</param>
    /// <returns>Configuration value if present, 0 if it could not be found.</returns>
    public static uint GetUInt(ConfigurationValues item)
    {
        byte[] buffer = new byte[4];
        uint ui = 0;

        (bool result, int length) = GetSetValue(item, Direction.Get, buffer);
        if (result && (length == 4))
        {
            ui = Encoders.ExtractUInt32(buffer, 0);
        }

        return ui;
    }

    /// <summary>
    /// Get a byte configuration item.
    /// </summary>
    /// <param name="item">Configuration item to read.</param>
    /// <returns>Configuration value if present, 0 if the item count not be found.</returns>
    public static byte GetByte(ConfigurationValues item)
    {
        byte[] buffer = new byte[1];
        byte b = 0;

        (bool result, int length) = GetSetValue(item, Direction.Get, buffer);
        if (result && (length == 1))
        {
            b = buffer[0];
        }

        return b;
    }

    /// <summary>
    /// Get a boolean configuration item.
    /// </summary>
    /// <param name="item">Configuration item to read.</param>
    /// <returns>Configuration value if present, false if the item could not be found.</returns>
    public static bool GetBoolean(ConfigurationValues item)
    {
        return GetByte(item) == 1 ? true : false;
    }

    /// <summary>
    /// Set the specified configuration item to the string value.
    /// </summary>
    /// <param name="item">Item to set.</param>
    /// <param name="value">Value to be used.></param>
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

        return result;
    }

    /// <summary>
    /// Set the specified configuration item to the given array of bytes.
    /// </summary>
    /// <param name="item">Item to set.</param>
    /// <param name="buffer">Byte buffer to hold the data.</param>
    /// <returns>True if the configuration value was set, false if there is a problem.</returns>
    /// TODO: why the byte buffer? do we really need that kind of optimization here?
    /// why not just return the array?
    public static bool SetByteArray(ConfigurationValues item, byte[] buffer)
    {
        (bool result, int length) = GetSetValue(item, Direction.Set, buffer);

        return result;
    }

    /// <summary>
    /// Set an unsigned integer configuration item.
    /// </summary>
    /// <param name="item">Item to set.</param>
    /// <param name="value">Value to be used.></param>
    /// <returns>True if the configuration value was set, false if there is a problem.</returns>
    public static bool SetUInt(ConfigurationValues item, uint value)
    {
        byte[] buffer = BitConverter.GetBytes(value);

        (bool result, int length) = GetSetValue(item, Direction.Set, buffer);

        return result;
    }

    /// <summary>
    /// Set a configuration to the specified byte.
    /// </summary>
    /// <param name="item">Item to set.</param>
    /// <param name="value">Value to be used.></param>
    /// <returns>True if the configuration value was set, false if there is a problem.</returns>
    public static bool SetByte(ConfigurationValues item, byte value)
    {
        byte[] buffer = new byte[1];
        buffer[0] = value;

        (bool result, int length) = GetSetValue(item, Direction.Set, buffer);

        return result;
    }

    /// <summary>
    /// Set a boolean configuration item.
    /// </summary>
    /// <param name="item">Item to set.</param>
    /// <param name="value">Value to be used.></param>
    /// <returns>True if the configuration value was set, false if there is a problem.</returns>
    public static bool SetBoolean(ConfigurationValues item, bool value)
    {
        byte b = (byte)((value ? 1 : 0) & 0xff);
        return SetByte(item, b);
    }
}
