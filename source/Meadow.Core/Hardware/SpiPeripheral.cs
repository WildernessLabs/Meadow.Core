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

        //public void SendData(Span<byte> data)
        //{
        //    Bus.SendData(ChipSelect, data);
        //}

        /// <summary>
        /// Writes a single byte to the peripheral
        /// </summary>
        /// <param name="value">Value to write</param>
        public void WriteByte(byte value)
        {
            Bus.SendData(ChipSelect, value);
        }

        /// <summary>
        /// Writes an array of bytes to the peripheral
        /// </summary>
        /// <param name="values">The data to write</param>
        public void WriteBytes(byte[] values)
        {
            Bus.SendData(ChipSelect, values);
        }

        /// <summary>
        /// Writes a single ushort value to a target register address on the peripheral (i.e. [address][ushort])
        /// </summary>
        /// <param name="address">The target write register address</param>
        /// <param name="value">Value to write</param>
        /// <param name="order">Endianness of the value to be written</param>
        public void WriteUShort(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
        {
            WriteUShorts(address, new ushort[] { value }, order);
        }

        /// <summary>
        /// Writes an array of ushort values to a target register address on the peripheral (i.e. [address][ushort])
        /// </summary>
        /// <param name="address">The target write register address</param>
        /// <param name="values">Values to write</param>
        /// <param name="order">Endianness of the values to be written</param>
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

        /// <summary>
        /// Writes a single byte to the specified address of the peripheral
        /// </summary>
        /// <param name="address">The target write register address</param>
        /// <param name="value">Value to write</param>
        public void WriteRegister(byte address, byte value)
        {
            Bus.SendData(ChipSelect, address, value);
        }

        /// <summary>
        /// Writes an array of bytes to the specified address of the peripheral
        /// </summary>
        /// <param name="address">The target write register address</param>
        /// <param name="values">Values to write</param>
        public void WriteRegisters(byte address, byte[] values)
        {
            var buffer = new byte[values.Length + 1];
            buffer[0] = address;
            Array.ConstrainedCopy(values, 0, buffer, 1, values.Length);
            Bus.SendData(ChipSelect, buffer);
        }

        /// <summary>
        /// Writes a series of byes to the peripheral then reads the specified number of bytes back from the peripheral.
        /// </summary>
        /// <param name="write"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <remarks>The data read happens <b>after</b> all outbound data has been clocked out (i.e. there is no overlap).  If you need overlap, use the Exchange method of the bus.</remarks>
        public byte[] WriteRead(byte[] write, ushort length)
        {
            // TODO: This is a terribly inefficient way to use a SPI bus

            // create a buffer for both directions
            var txBuffer = new byte[write.Length + length];
            //                       output^^        ^^input

            // copy the output to the beginning of the buffer
            Array.Copy(write, txBuffer, write.Length);

            // create an input buffer
            var rxBuffer = new byte[txBuffer.Length];

            // do a data exchange
            Bus.ExchangeData(ChipSelect, ChipSelectMode.ActiveLow, txBuffer, rxBuffer);

            // copy the result to "left align" it in an array
            var result = new byte[length];
            Array.ConstrainedCopy(rxBuffer, write.Length, result, 0, result.Length);

            // and return it
            return result;
        }

        public void ExchangeData(Span<byte> writeBuffer, Span<byte> readBuffer)
        {
            //// TODO: This is a terribly inefficient way to use a SPI bus - underlying bus needs Span support
            //var r = WriteRead(writeBuffer.ToArray(), (ushort)readBuffer.Length);
            //for (int i = 0; i < readBuffer.Length; i++)
            //{
            //    readBuffer[i] = r[i];
            //}
            Bus.ExchangeData(ChipSelect, ChipSelectMode.ActiveLow, writeBuffer, readBuffer);
        }

        // TODO:
        public void Read(Span<byte> readBuffer)
        {
            throw new NotImplementedException();
        }

        public void ReadRegister(byte address, Span<byte> buffer)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Reads the specified number of bytes from the bus
        /// </summary>
        /// <param name="numberOfBytes">The number of bytes to read</param>
        /// <returns>the data read</returns>
        public byte[] ReadBytes(ushort numberOfBytes)
        {
            return Bus.ReceiveData(ChipSelect, numberOfBytes);
        }

        /// <summary>
        /// Reads a single byte from the specified address of the peripheral
        /// </summary>
        /// <param name="address">Address to read</param>
        /// <returns>The byte read</returns>
        public byte ReadRegister(byte address)
        {
            return ReadRegisters(address, 1).First();
        }

        /// <summary>
        /// Reads an array of bytes from the specified address of the peripheral
        /// </summary>
        /// <param name="address">Address of the read</param>
        /// <param name="length">Number of bytes to read</param>
        /// <returns>Data read from the device</returns>
        public byte[] ReadRegisters(byte address, ushort length)
        {
            var buffer = new byte[length];
            buffer[0] = address;

            return WriteRead(buffer, length);
        }

        /// <summary>
        /// Reads a single ushort value from the specified address of the peripheral
        /// </summary>
        /// <param name="address">Address of the read</param>
        /// <param name="order">Endianness of the value read</param>
        /// <returns>The value read</returns>
        public ushort ReadUShort(byte address, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = ReadRegisters(address, 2);
            if (order == ByteOrder.LittleEndian)
            {
                return (ushort)((data[1] << 8) | data[0]);
            }
            return (ushort)((data[0] << 8) | data[1]);
        }

        /// <summary>
        /// Reads an array of ushort values from the specified address of the peripheral
        /// </summary>
        /// <param name="address">Address of the read</param>
        /// <param name="number">The number of ushort values to read</param>
        /// <param name="order">Endianness of the values read</param>
        /// <returns>The values read</returns>
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
