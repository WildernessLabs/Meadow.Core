using Meadow.Hardware;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Meadow;

/// <summary>
/// Represents an I2C bus with specified bus number and speed.
/// </summary>
public partial class I2CBus : II2cBus, IDisposable
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct I2CIoctlData
    {
        public ushort Address;
        public I2CMessageFlags Flags;
        public ushort Length;
        public byte* Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct I2CIoctlDataSet
    {
        public I2CIoctlData* msgs;
        public int nmsgs;
    }

    internal enum I2CMessageFlags : ushort
    {
        Write = 0x0000,
        Read = 0x0001
    }

    private class SMBusIoctlData
    {
        public SMBusIoctlData(int size)
        {
            Size = size;
            Data = new byte[Size];
        }

        public byte ReadWrite { get; set; }
        public byte Command { get; set; }
        public int Size { get; set; }
        public byte[] Data { get; set; }
    }

    private enum I2CIoctl
    {
        RETRIES = 0x0701,
        TIMEOUT = 0x0702,
        SLAVE = 0x0703,
        FUNCS = 0x0705,
        RDWR = 0x0707,
        SMBUS = 0x0720
    }

    /// <inheritdoc/>
    public int BusNumber { get; set; } = 1;
    /// <inheritdoc/>
    public I2cBusSpeed BusSpeed { get; set; }

    private PeripheralMap InfoMap { get; }

    internal I2CBus(int busNumber, I2cBusSpeed busSpeed)
    {
        BusNumber = busNumber;

        // TODO: how do we affect frequency on these platforms?
        BusSpeed = busSpeed;

        InfoMap = new PeripheralMap(BusNumber);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        InfoMap.Dispose();
    }

    /// <inheritdoc/>
    public unsafe void Read(byte peripheralAddress, Span<byte> readBuffer)
    {
        var handle = InfoMap.GetAddressHandle(BusNumber, peripheralAddress);
        fixed (byte* pData = readBuffer)
        {
            var result = Interop.read(handle, pData, readBuffer.Length);
            if (result < 0)
            {
                var le = (LinuxErrorCode)Marshal.GetLastWin32Error();
                switch (le)
                {
                    case LinuxErrorCode.IOError:
                        throw new NativeException("I/O Error.  Check your wiring");
                    default:
                        var msg = $"READ ERROR: {le}";
                        throw new NativeException(msg);
                }
            }
        }
    }

    /// <inheritdoc/>
    public unsafe void Write(byte peripheralAddress, Span<byte> writeBuffer)
    {
        var handle = InfoMap.GetAddressHandle(BusNumber, peripheralAddress);
        fixed (byte* pData = writeBuffer)
        {
            var result = Interop.write(handle, pData, writeBuffer.Length);
            if (result < 0)
            {
                var le = (LinuxErrorCode)Marshal.GetLastWin32Error();
                switch (le)
                {
                    case LinuxErrorCode.IOError:
                        throw new NativeException("I/O Error.  Check your wiring");
                    default:
                        var msg = $"WRITE ERROR: {le}";
                        throw new NativeException(msg);
                }
            }
        }
    }

    /// <inheritdoc/>
    public unsafe void Exchange(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
    {
        if (InfoMap.SupportsIoctlExchange)
        {
            ExchangeWithIoctl(peripheralAddress, writeBuffer, readBuffer);
        }
        else
        {
            ExchangeWithFileOps(peripheralAddress, writeBuffer, readBuffer);
        }
    }

    private unsafe void ExchangeWithIoctl(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
    {
        I2CIoctlData* pMessages = stackalloc I2CIoctlData[2];
        var index = 0;

        var handle = InfoMap.GetAddressHandle(BusNumber, peripheralAddress);
        fixed (byte* pWrite = writeBuffer)
        fixed (byte* pRead = readBuffer)
        {
            if (writeBuffer != null)
            {
                pMessages[index] = new I2CIoctlData()
                {
                    Address = peripheralAddress,
                    Flags = I2CMessageFlags.Write,
                    Length = (ushort)writeBuffer.Length,
                    Data = pWrite
                };
                index++;
            }

            if (readBuffer != null)
            {
                pMessages[index] = new I2CIoctlData()
                {
                    Address = peripheralAddress,
                    Flags = I2CMessageFlags.Read,
                    Length = (ushort)readBuffer.Length,
                    Data = pRead
                };
                index++;
            }

            var data = new I2CIoctlDataSet()
            {
                msgs = pMessages,
                nmsgs = index
            };

            int result = Interop.ioctl(handle, (int)I2CIoctl.RDWR, new IntPtr(&data));
            if (result < 0)
            {
                throw new IOException($"Error {Marshal.GetLastWin32Error()} performing I2C data transfer.");
            }
        }
    }

    private unsafe void ExchangeWithFileOps(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
    {
        var handle = InfoMap.GetAddressHandle(BusNumber, peripheralAddress);
        fixed (byte* pWrite = writeBuffer)
        fixed (byte* pRead = readBuffer)
        {
            var result = Interop.write(handle, pWrite, writeBuffer.Length);
            if (result < 0)
            {
                var le = (LinuxErrorCode)Marshal.GetLastWin32Error();
                switch (le)
                {
                    case LinuxErrorCode.IOError:
                        throw new NativeException("I/O Error.  Check your wiring");
                    default:
                        var msg = $"WRITE ERROR: {le}";
                        throw new NativeException(msg);
                }
            }
            result = Interop.read(handle, pRead, readBuffer.Length);
            if (result < 0)
            {
                var le = (LinuxErrorCode)Marshal.GetLastWin32Error();
                switch (le)
                {
                    case LinuxErrorCode.IOError:
                        throw new NativeException("I/O Error.  Check your wiring");
                    default:
                        var msg = $"READ ERROR: {le}";
                        throw new NativeException(msg);
                }
            }
        }
    }
}
