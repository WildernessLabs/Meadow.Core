using System;
using System.Collections.Generic;
using System.Linq;

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
            return this.Bus.ReadData(this.Address, numberOfBytes);
        }

        public byte ReadRegister(byte address)
        {
            return this.Bus.WriteReadData(this.Address, 1, address).First();
        }

        public byte[] ReadRegisters(byte address, ushort length)
        {
            return this.Bus.WriteReadData(this.Address, length, address);
        }

        public ushort ReadUShort(byte address, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = this.Bus.WriteReadData(this.Address, 2, address);
            if (order == ByteOrder.LittleEndian)
            {
                return (ushort)(data[0] | (data[1] << 8));
            }
            return (ushort)((data[0] << 8) | data[1]);
        }

        public ushort[] ReadUShorts(byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = this.Bus.WriteReadData(this.Address, number * 2, address);

            var result = new ushort[number];
            for (int i = 0; i < number; i++)
            {
                if (order == ByteOrder.LittleEndian)
                {
                    result[i] = (ushort)(data[i * 2] | (data[i * 2 + 1] << 8));
                }
                result[i] = (ushort)((data[i * 2] << 8) | data[i * 2 + 1]);
            }

            return result;
        }

        public void WriteByte(byte value)
        {
            this.Bus.WriteData(this.Address, value);
        }

        public void WriteBytes(byte[] values)
        {
            this.Bus.WriteData(this.Address, values);
        }

        public byte[] WriteRead(byte[] write, ushort length)
        {
            return this.Bus.WriteReadData(this.Address, length, write);
        }

        public void WriteRegister(byte address, byte value)
        {
            this.Bus.WriteData(this.Address, address, value);
        }

        public void WriteRegisters(byte address, byte[] data)
        {
            var temp = new List<byte>();
            temp.Add(address);
            temp.AddRange(data);

            this.Bus.WriteData(this.Address, temp);
        }

        public void WriteUShort(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
        {
            var temp = new List<byte>();
            temp.Add(address);

            var b = BitConverter.GetBytes(value);

            if (order == ByteOrder.LittleEndian)
            {
                temp.AddRange(b);
            }
            else
            {
                temp.Add(b[1]);
                temp.Add(b[0]);
            }

            this.Bus.WriteData(this.Address, temp);
        }

        public void WriteUShorts(byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian)
        {
            var temp = new List<byte>();
            temp.Add(address);

            for (int i = 0; i < values.Length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);

                if (order == ByteOrder.LittleEndian)
                {
                    temp.AddRange(b);
                }
                else
                {
                    temp.Add(b[1]);
                    temp.Add(b[0]);
                }
            }

            this.Bus.WriteData(this.Address, temp);
        }
    }
}
