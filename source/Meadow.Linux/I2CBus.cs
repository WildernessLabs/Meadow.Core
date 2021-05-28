using Meadow.Hardware;
using System;
using System.Collections.Generic;

namespace Meadow
{
    public class I2CBus : II2cBus
    {
        public int Frequency { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int BusNumber { get; } = 0;
        internal int Handle { get; set; }

        internal I2CBus(IPin clock, IPin data, int frequencyHz)
        {
            // TODO: determine bus number based on pins?

            // open the i2c block driver
            var driver = $"/dev/i2c-{BusNumber}";
            var handle = Interop.open(driver, Interop.DriverFlags.ReadWrite);
            if(handle < 0)
            {
                // TODO: maybe try modprobe here?
                throw new Exception($"Unable to open driver {driver}");
            }
            Handle = handle;
        }

        public byte[] ReadData(byte peripheralAddress, int numberOfBytes)
        {
            throw new NotImplementedException();
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

        public void WriteData(byte peripheralAddress, Span<byte> data)
        {
            throw new NotImplementedException();
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
