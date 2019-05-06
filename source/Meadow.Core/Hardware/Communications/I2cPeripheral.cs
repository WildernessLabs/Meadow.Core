using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Defines a contract for a peripheral that communicates via the IIC/I2C 
    /// protocol.
    /// </summary>
    public class I2cPeripheral : II2cPeripheral
    {
        public byte Address { get; protected set; }
        public II2cBus Bus { get; protected set; }

        public I2cPeripheral(II2cBus bus, byte peripheralAddress)
        {
            Bus = bus;
            Address = peripheralAddress;
        }

        public byte[] ReadBytes(ushort numberOfBytes)
        {
            return this.Bus.ReadBytes(this.Address, numberOfBytes);
        }

        public byte ReadRegister(byte address)
        {
            return this.Bus.ReadRegister(this.Address, address);
        }

        public byte[] ReadRegisters(byte address, ushort length)
        {
            return this.Bus.ReadRegisters(this.Address, address, length);
        }

        public ushort ReadUShort(byte address, ByteOrder order = ByteOrder.LittleEndian)
        {
            return this.Bus.ReadUShort(this.Address, address, order);
        }

        public ushort[] ReadUShorts(byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian)
        {
            return this.Bus.ReadUShorts(this.Address, address, number, order);
        }

        public void WriteByte(byte value)
        {
            this.Bus.WriteByte(this.Address, value);
        }

        public void WriteBytes(byte[] values)
        {
            this.Bus.WriteBytes(this.Address, values);
        }

        public byte[] WriteRead(byte[] write, ushort length)
        {
            return this.Bus.WriteRead(this.Address, write, length);
        }

        public void WriteRegister(byte address, byte value)
        {
            this.Bus.WriteRegister(this.Address, address, value);
        }

        public void WriteRegisters(byte address, byte[] data)
        {
            this.Bus.WriteRegisters(this.Address, address, data);
        }

        public void WriteUShort(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
        {
            this.Bus.WriteUShort(this.Address, address, value, order);
        }

        public void WriteUShorts(byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian)
        {
            this.Bus.WriteUShorts(this.Address, address, values, order);
        }
    }
}
