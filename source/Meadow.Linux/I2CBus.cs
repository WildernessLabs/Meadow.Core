using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Meadow
{
    internal class I2CPeripheralInfo
    {
        public int BusNumber { get; set; }
        public byte BusAddress { get; set; }
        public int DriverHandle { get; set; }
        public bool IsOpen { get; set; }
    }

    public class I2CBus : II2cBus, IDisposable
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

        // the I2C block driver is...interesting.  You open it multiple times, once per peripheral address per bus
        private Dictionary<byte, I2CPeripheralInfo> InfoMap0 { get; }

        internal I2CBus(int busNumber, Frequency frequency)
        {
            BusNumber = busNumber;

            // TODO: how do we affect frequency on these platforms?
            Frequency = frequency;

            InfoMap0 = new(); // this is just for bus 0
        }

        public void Dispose()
        {
            Interop.close(InfoMap0.First().Value.DriverHandle);
        }

        private int GetPeripheralHandle(int busNumber, byte busAddress)
        {
            Dictionary<byte, I2CPeripheralInfo> map;

            switch(busNumber)
            {
                case 0:
                case 1:
                    map = InfoMap0;
                    break;
                default:
                    throw new Exception($"Unsupported bus number: {busNumber}");
            }

            I2CPeripheralInfo info;

            if (map.ContainsKey(busAddress))
            {
                info = map[busAddress];
            }
            else
            {
                // open the i2c block driver
                var driver = $"/dev/i2c-{busNumber}";
                var handle = Interop.open(driver, Interop.DriverFlags.O_RDWR);
                if (handle < 0)
                {
                    // TODO: maybe try modprobe here?
                    throw new Exception($"Unable to open driver {driver}");
                }
                info = new I2CPeripheralInfo
                {
                    DriverHandle = handle,
                    BusAddress = busAddress,
                    BusNumber = busNumber
                };
                map.Add(busAddress, info);
            }

            if(!info.IsOpen)
            {
                // call the ioctl to set the address for this handle
                var result = Interop.ioctl(info.DriverHandle, (int)I2CIoctl.SLAVE, info.BusAddress);
                if(result < 0)
                {
                    Console.WriteLine($"ERROR: {Marshal.GetLastWin32Error()}");
                }
            }

            return info.DriverHandle;
        }

        public byte[] ReadData(byte peripheralAddress, int numberOfBytes)
        {
            var data = new byte[numberOfBytes];
            var handle = GetPeripheralHandle(BusNumber, peripheralAddress);
            var result = Interop.read(handle, data, numberOfBytes);
            if(result < 0)
            {
                Console.WriteLine($"ERROR: {Marshal.GetLastWin32Error()}");
            }
            return data;
        }

        public void WriteData(byte peripheralAddress, params byte[] data)
        {
            WriteData(peripheralAddress, data, data.Length);
        }

        public void WriteData(byte peripheralAddress, byte[] data, int length)
        {
            var handle = GetPeripheralHandle(BusNumber, peripheralAddress);
            var result = Interop.write(handle, data, length);
            if(result < 0)
            {
                Console.WriteLine($"ERROR: {Marshal.GetLastWin32Error()}");
            }
        }

        public void WriteData(byte peripheralAddress, IEnumerable<byte> data)
        {
            WriteData(peripheralAddress, data.ToArray(), data.Count());
        }

        public void WriteData(byte peripheralAddress, Span<byte> data)
        {
            var handle = GetPeripheralHandle(BusNumber, peripheralAddress);
            unsafe
            {
                fixed (byte* writePtr = &MemoryMarshal.GetReference(data))
                {
                    var result = Interop.write(handle, writePtr, data.Length);
            if(result < 0)
            {
                Console.WriteLine($"ERROR: {Marshal.GetLastWin32Error()}");
            }
                }
            }
        }

        public byte[] WriteReadData(byte peripheralAddress, int byteCountToRead, params byte[] dataToWrite)
        {
            var handle = GetPeripheralHandle(BusNumber, peripheralAddress);
            var readBuffer = new byte[byteCountToRead];
            var result = Interop.write(handle, dataToWrite, dataToWrite.Length);
            result = Interop.read(handle, readBuffer, readBuffer.Length);
            if(result < 0)
            {
                Console.WriteLine($"ERROR: {Marshal.GetLastWin32Error()}");
            }
            return readBuffer;
        }

        public void WriteReadData(byte peripheralAddress, Span<byte> writeBuffer, int writeCount, Span<byte> readBuffer, int readCount)
        {
            var handle = GetPeripheralHandle(BusNumber, peripheralAddress);
            unsafe
            {
                fixed (byte* writePtr = &MemoryMarshal.GetReference(writeBuffer))
                fixed (byte* readPtr = &MemoryMarshal.GetReference(readBuffer))
                {
                    var result = Interop.write(handle, writePtr, writeCount);
                    if(result < 0)
                    {
                        Console.WriteLine($"ERROR: {Marshal.GetLastWin32Error()}");
                    }
                    result = Interop.read(handle, readPtr, readCount);
                    if(result < 0)
                    {
                        Console.WriteLine($"ERROR: {Marshal.GetLastWin32Error()}");
                    }
                }
            }
        }

        public void WriteReadData(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
        {
            var handle = GetPeripheralHandle(BusNumber, peripheralAddress);
            unsafe
            {
                fixed (byte* writePtr = &MemoryMarshal.GetReference(writeBuffer))
                fixed (byte* readPtr = &MemoryMarshal.GetReference(readBuffer))
                {
                    var result = Interop.write(handle, writePtr, writeBuffer.Length);
                    if(result < 0)
                    {
                        Console.WriteLine($"ERROR: {Marshal.GetLastWin32Error()}");
                    }
                    result = Interop.read(handle, readPtr, readBuffer.Length);
                    if(result < 0)
                    {
                        Console.WriteLine($"ERROR: {Marshal.GetLastWin32Error()}");
                    }
                }
            }
        }
    }
}
