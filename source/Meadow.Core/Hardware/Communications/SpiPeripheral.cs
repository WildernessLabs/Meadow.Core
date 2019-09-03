using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.Hardware
{
    public class SpiPeripheral : ISpiPeripheral
    {
        public IDigitalOutputPort ChipSelect { get; }
        public ISpiBus Bus { get; }

        public SpiPeripheral(ISpiBus bus, IDigitalOutputPort chipSelect)
        {
            this.Bus = bus;
            this.ChipSelect = chipSelect;
        }

        public void WriteByte(byte value)
        {
            Bus.SendData(ChipSelect, value);
        }

        public void WriteBytes(byte[] values)
        {
            Bus.SendData(ChipSelect, values);
        }

        public void WriteUShort(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
        {
            WriteUShorts(address, new ushort[] { value }, order);
        }

        public void WriteUShorts(byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = new List<byte>();
            data.Add(address);

            foreach(var v in values)
            {
                if (order == ByteOrder.LittleEndian)
                {
                    data.Add((byte)(v >> 8));
                    data.Add((byte)(v & 0xff));
                }
                else
                {
                    data.Add((byte)(v & 0xff));
                    data.Add((byte)(v >> 8));
                }
            }

            WriteBytes(data.ToArray());
        }

        public void WriteRegister(byte address, byte value)
        {
            Bus.SendData(ChipSelect, address, value);
        }

        public void WriteRegisters(byte address, byte[] data)
        {
            var buffer = new byte[data.Length + 1];
            buffer[0] = address;
            Array.ConstrainedCopy(data, 0, buffer, 1, data.Length);
            Bus.SendData(ChipSelect, buffer);
        }

        public byte[] WriteRead(byte[] write, ushort length)
        {
            // assume we write all bytes and then read data;
            var totalExchange = new byte[write.Length + length];
            Array.Copy(write, totalExchange, write.Length);

            var rx = Bus.ExchangeData(ChipSelect, totalExchange);

            var result = new byte[length];
            Array.ConstrainedCopy(rx, write.Length - 1, result, 0, result.Length);

            return result;
        }

        public byte[] ReadBytes(ushort numberOfBytes)
        {
            return Bus.ReceiveData(ChipSelect, numberOfBytes);
        }

        public byte ReadRegister(byte address)
        {
            return ReadRegisters(address, 1).First();
        }

        public byte[] ReadRegisters(byte address, ushort length)
        {
            // the buffer needs to be big enough for the output and response
            var buffer = new byte[length + 1];
            buffer[0] = address;

            var rx = Bus.ExchangeData(ChipSelect, buffer);

            // skip past the byte where we clocked out the register address
            var registerData = rx.Skip(1).Take(length).ToArray();

            return registerData;
        }

        public ushort ReadUShort(byte address, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = ReadRegisters(address, 2);
            if (order == ByteOrder.LittleEndian)
            {
                return (ushort)((data[1] << 8) | data[0]);
            }
            return (ushort)((data[0] << 8) | data[1]);
        }

        public ushort[] ReadUShorts(byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = ReadRegisters(address, (ushort)(2 * number));

            var result = new ushort[number];

            for (int i = 0; i < number; i++)
            {
                if (order == ByteOrder.LittleEndian)
                {
                    result[i] = (ushort)((data[(i * 2) + 1] << 8) | data[i * 2]);
                }
                else
                {
                    result[i] = (ushort)((data[(i * 2)] << 8) | data[(i * 2) + 1]);
                }
            }

            return result;

        }
    }
}
