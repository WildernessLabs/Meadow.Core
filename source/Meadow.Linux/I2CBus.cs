using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow
{
    internal class I2CPeripheralInfo
    {
        public int BusNumber { get; set; }
        public byte BusAddress { get; set; }
        public int DriverHandle { get; set; }
        public bool IsOpen { get; set; }
    }

    public class I2CBus : II2cBus
    {
        public int Frequency { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//        public int BusNumber { get; } = 0;
//        internal int Handle { get; set; }

        // the I2C block driver is...interesting.  You open it multiple times, once per peripheral address per bus
        private Dictionary<byte, I2CPeripheralInfo> InfoMap0 { get; }

        internal I2CBus(IPin clock, IPin data, int frequencyHz)
        {
            // TODO: determine bus number based on pins?
            // TODO: how do we affect frequency on these platforms?

            InfoMap0 = new(); // this is just for bus 0
        }

        private int GetPeripheralHandle(int busNumber, byte busAddress)
        {
            Dictionary<byte, I2CPeripheralInfo> map;

            switch(busNumber)
            {
                case 0:
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
                var handle = Interop.open(driver, Interop.DriverFlags.ReadWrite);
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
            }

            return info.DriverHandle;
        }

        public byte[] ReadData(byte peripheralAddress, int numberOfBytes)
        {
            throw new NotImplementedException();
        }

        public void WriteData(byte peripheralAddress, params byte[] data)
        {
            WriteData(peripheralAddress, data, data.Length);
        }

        public void WriteData(byte peripheralAddress, byte[] data, int length)
        {
            var handle = GetPeripheralHandle(0, peripheralAddress);
            Interop.write(handle, data, length);
        }

        public void WriteData(byte peripheralAddress, IEnumerable<byte> data)
        {
            WriteData(peripheralAddress, data.ToArray(), data.Count());
        }

        public void WriteData(byte peripheralAddress, Span<byte> data)
        {
            WriteData(peripheralAddress, data.ToArray(), data.Length);
        }

        public byte[] WriteReadData(byte peripheralAddress, int byteCountToRead, params byte[] dataToWrite)
        {
            throw new NotImplementedException();
        }

        public void WriteReadData(byte peripheralAddress, Span<byte> writeBuffer, int writeCount, Span<byte> readBuffer, int readCount)
        {
            throw new NotImplementedException();
        }

        public void WriteReadData(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
        {
            throw new NotImplementedException();
        }
    }
}
