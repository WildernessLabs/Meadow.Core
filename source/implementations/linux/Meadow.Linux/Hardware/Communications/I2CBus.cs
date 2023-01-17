using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Meadow
{
    public partial class I2CBus : II2cBus, IDisposable
    {
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

        public int BusNumber { get; set; } = 1;
        public Frequency Frequency { get; set; }

        private PeripheralMap InfoMap { get; }

        public I2CBus(int busNumber, Frequency frequency)
        {
            BusNumber = busNumber;

            // TODO: how do we affect frequency on these platforms?
            Frequency = frequency;
            InfoMap = new PeripheralMap(BusNumber);
        }

        public void Dispose()
        {
            InfoMap.Dispose();
        }

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

        public unsafe void Exchange(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
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

        public void WriteData(byte peripheralAddress, params byte[] data)
        {
            throw new NotImplementedException();
        }

        public void WriteData(byte peripheralAddress, byte[] data, int length)
        {
            throw new NotImplementedException();
        }

        public void WriteData(byte peripheralAddress, IEnumerable<byte> data)
        {
            throw new NotImplementedException();
        }

        public byte[] WriteReadData(byte peripheralAddress, int byteCountToRead, params byte[] dataToWrite)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadData(byte peripheralAddress, int numberOfBytes)
        {
            throw new NotImplementedException();
        }
    }
}
