using System;
namespace Meadow.Hardware
{
    public class SpiPeripheral : ISpiPeripheral
    {
        public IDigitalOutputPort ChipSelect { get; protected set; }
        public ISpiBus Bus { get; protected set; }

        public SpiPeripheral(ISpiBus bus, IDigitalOutputPort chipSelect)
        {
            this.Bus = bus;
            this.ChipSelect = chipSelect;
        }

        public byte[] ReadBytes(ushort numberOfBytes)
        {
            return this.Bus.ReadBytes(this.ChipSelect, numberOfBytes);
        }

        public byte ReadRegister(byte address)
        {
            return this.Bus.ReadRegister(this.ChipSelect, address);
        }

        public byte[] ReadRegisters(byte address, ushort length)
        {
            return this.Bus.ReadRegisters(this.ChipSelect, address, length);
        }

        public ushort ReadUShort(byte address, ByteOrder order)
        {
            return this.Bus.ReadUShort(this.ChipSelect, address, order);
        }

        public ushort[] ReadUShorts(byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian)
        {
            return this.Bus.ReadUShorts(this.ChipSelect, address, number, order);
        }

        public void WriteByte(byte value)
        {
            this.Bus.WriteByte(this.ChipSelect, value);
        }

        public void WriteBytes(byte[] values)
        {
            this.Bus.WriteBytes(this.ChipSelect, values);
        }

        public byte[] WriteRead(byte[] write, ushort length)
        {
            return this.Bus.WriteRead(this.ChipSelect, write, length);
        }

        public void WriteRegister(byte address, byte value)
        {
            this.Bus.WriteRegister(this.ChipSelect, address, value);
        }

        public void WriteRegisters(byte address, byte[] data)
        {
            this.Bus.WriteRegisters(this.ChipSelect, address, data);
        }

        public void WriteUShort(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
        {
            this.Bus.WriteUShort(this.ChipSelect, address, value, order);
        }

        public void WriteUShorts(byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian)
        {
            this.Bus.WriteUShorts(this.ChipSelect, address, values, order);
        }
    }
}
