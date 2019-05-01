using System;
using Meadow.Hardware.Communications;

namespace Meadow.Hardware
{
    /// <summary>
    /// Defines a contract for a peripheral that communicates via the IIC/I2C 
    /// protocol.
    /// </summary>
    public class I2cPeripheral : II2cPeripheral
    {
        public byte Address { get; protected set; }

        public I2cPeripheral(byte address)
        {
            Address = address;
        }


        //public Configuration Config;
        //protected bool m_disposed;

        //public I2cPeripheral(Configuration config) { throw new NotImplementedException(); }

        //~I2cPeripheral() { throw new NotImplementedException(); }

        //public static I2CReadTransaction CreateReadTransaction(byte[] buffer) { throw new NotImplementedException(); }
        //public static I2CWriteTransaction CreateWriteTransaction(byte[] buffer) { throw new NotImplementedException(); }
        //public void Dispose() { throw new NotImplementedException(); }
        //public int Execute(I2CTransaction[] xActions, int timeout) { throw new NotImplementedException(); }

        //public class Configuration
        //{
        //    public readonly ushort Address;
        //    public readonly int ClockRateKhz;

        //    public Configuration(ushort address, int clockRateKhz) { throw new NotImplementedException(); }
        //}
        //public sealed class I2CReadTransaction : I2CTransaction
        //{
        //}
        //public class I2CTransaction
        //{
        //    public readonly byte[] Buffer;

        //  //  protected I2CTransaction(byte[] buffer) { }
        //}
        //public sealed class I2CWriteTransaction : I2CTransaction
        //{
        //}

        public byte[] ReadBytes(ushort numberOfBytes)
        {
            throw new NotImplementedException();
        }

        public byte ReadRegister(byte address)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadRegisters(byte address, ushort length)
        {
            throw new NotImplementedException();
        }

        public ushort ReadUShort(byte address, ByteOrder order)
        {
            throw new NotImplementedException();
        }

        public ushort[] ReadUShorts(byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian)
        {
            throw new NotImplementedException();
        }

        public void WriteByte(byte value)
        {
            throw new NotImplementedException();
        }

        public void WriteBytes(byte[] values)
        {
            throw new NotImplementedException();
        }

        public byte[] WriteRead(byte[] write, ushort length)
        {
            throw new NotImplementedException();
        }

        public void WriteRegister(byte address, byte value)
        {
            throw new NotImplementedException();
        }

        public void WriteRegisters(byte address, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void WriteUShort(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
        {
            throw new NotImplementedException();
        }

        public void WriteUShorts(byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian)
        {
            throw new NotImplementedException();
        }
    }
}
