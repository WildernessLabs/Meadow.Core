using Meadow.Hardware;
using System;
using System.Collections.Generic;

namespace Meadow
{
    public class KrtklI2CBus : II2cBus, IDisposable
    {
        public I2cBusSpeed BusSpeed { get; set; }


        public KrtklI2CBus(I2cBusSpeed busSpeed)
        {
            BusSpeed = busSpeed;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Exchange(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
        {
            throw new NotImplementedException();
        }

        public void Read(byte peripheralAddress, Span<byte> readBuffer)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadData(byte peripheralAddress, int numberOfBytes)
        {
            throw new NotImplementedException();
        }

        public void Write(byte peripheralAddress, Span<byte> writeBuffer)
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

        public byte[] WriteReadData(byte peripheralAddress, int byteCountToRead, params byte[] dataToWrite)
        {
            throw new NotImplementedException();
        }
    }
}
