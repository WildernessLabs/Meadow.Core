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

        /// <summary>
        /// Internal write buffer. Used in methods in which the buffers aren't
        /// passed in.
        /// </summary>
        protected Memory<byte> WriteBuffer { get; }
        /// <summary>
        /// Internal read buffer. Used in methods in which the buffers aren't
        /// passed in.
        /// </summary>
        protected Memory<byte> ReadBuffer { get; }

        public I2cPeripheral(II2cBus bus, byte peripheralAddress, int readBufferSize = 8, int writeBufferSize = 8)
        {
            Bus = bus;
            Address = peripheralAddress;
            WriteBuffer = new byte[writeBufferSize];
            ReadBuffer = new byte[readBufferSize];
        }

        //==== NEW HOTNESS

        /// <summary>
        /// Reads data from the peripheral.
        /// </summary>
        /// <param name="readBuffer">The buffer to read from the peripheral into.</param>
        /// <remarks>
        /// The number of bytes to be read is determined by the length of the
        /// `readBuffer`.
        /// </remarks>
        public void Read(Span<byte> readBuffer)
        {
            Bus.Read(this.Address, readBuffer);
        }

        /// <summary>
        /// Reads data from the peripheral starting at the specified address.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        public void ReadRegister(byte address, Span<byte> readBuffer)
        {
            WriteBuffer.Span[0] = address;
            Bus.Exchange(this.Address, WriteBuffer.Span[0..1], readBuffer);
        }

        /// <summary>
        /// Read a register from the peripheral.
        /// </summary>
        /// <param name="address">Address of the register to read.</param>
        public byte ReadRegister(byte address)
        {
            WriteBuffer.Span[0] = address;
            Bus.Exchange(this.Address, WriteBuffer.Span[0..1], ReadBuffer.Span[0..1]);
            return ReadBuffer.Span[0];
        }

        /// <summary>
        /// Read an usingned short from a register.
        /// </summary>
        /// <param name="address">Register address of the low byte (the high byte will follow).</param>
        /// <param name="order">Order of the bytes in the register (little endian is the default).</param>
        /// <returns>Value read from the register.</returns>
        public ushort ReadRegisterAsUShort(byte address, ByteOrder order = ByteOrder.LittleEndian)
        {
            WriteBuffer.Span[0] = address;
            Bus.Exchange(this.Address, WriteBuffer[0..1].Span, ReadBuffer[0..2].Span);
            if (order == ByteOrder.LittleEndian) {
                return (ushort)(ReadBuffer.Span[0] | (ReadBuffer.Span[1] << 8));
            } else {
                return (ushort)(ReadBuffer.Span[0] << 8 | ReadBuffer.Span[1]);
            }
        }

        /// <summary>
        /// Write a single byte to the peripheral.
        /// </summary>
        /// <param name="value">Value to be written (8-bits).</param>
        public void Write(byte value)
        {
            // stuff the value into the write buffer
            WriteBuffer.Span[0] = value;
            this.Bus.Write(this.Address, WriteBuffer.Span[0..1]);
        }

        /// <summary>
        /// Write an array of bytes to the peripheral.
        /// </summary>
        /// <param name="data">Values to be written.</param>
        public void Write(Span<byte> data)
        {
            this.Bus.Write(this.Address, data);
        }

        /// <summary>
        /// Write data a register in the peripheral.
        /// </summary>
        /// <param name="address">Address of the register to write to.</param>
        /// <param name="value">Data to write into the register.</param>
        public void WriteRegister(byte address, byte value)
        {
            // stuff the address and value into the write buffer
            WriteBuffer.Span[0] = address;
            WriteBuffer.Span[1] = value;
            this.Bus.Write(this.Address, WriteBuffer.Span[0..2]);
        }

        /// <summary>
        /// Write an unsigned short to the peripheral.
        /// </summary>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="value">Value to be written (16-bits).</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        public void WriteRegister(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
        {
            // split the 16 bit ushort into two bytes
            var bytes = BitConverter.GetBytes(value);
            // call the helper method
            WriteRegister(address, bytes, order);
        }

        /// <summary>
        /// Write an unsigned integer to the peripheral.
        /// </summary>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="value">Value to be written.</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        public void WriteRegister(byte address, uint value, ByteOrder order = ByteOrder.LittleEndian)
        {
            // split the 32 bit uint into four bytes
            var bytes = BitConverter.GetBytes(value);
            // call the helper method
            WriteRegister(address, bytes, order);
        }

        /// <summary>
        /// Write an unsigned long to the peripheral.
        /// </summary>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="value">Value to be written.</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        public void WriteRegister(byte address, ulong value, ByteOrder order = ByteOrder.LittleEndian)
        {
            // split the 64 bit ulong into 8 bytes
            var bytes = BitConverter.GetBytes(value);
            // call the helper method
            WriteRegister(address, bytes, order);
        }

        /// <summary>
        /// Write data to a register in the peripheral.
        /// </summary>
        /// <param name="address">Address of the register to write to.</param>
        /// <param name="writeBuffer">A buffer of byte values to be written.</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        public void WriteRegister(byte address, Span<byte> writeBuffer, ByteOrder order = ByteOrder.LittleEndian)
        {
            if (WriteBuffer.Length < writeBuffer.Length + 1) {
                throw new ArgumentException("Data to write is too large for the write buffer. " +
                    "Must be less than WriteBuffer.Length + 1 (to allow for address). " +
                    "Instantiate this class with a larger WriteBuffer, or send a smaller" +
                    "amount of data to fix.");
            }

            // stuff the register address into the write buffer
            WriteBuffer.Span[0] = address;

            // stuff the bytes into the write buffer (starting at `1` index,
            // because `0` is the register address.
            switch (order) {
                case ByteOrder.LittleEndian:
                    for (int i = 0; i < writeBuffer.Length; i++) {
                        WriteBuffer.Span[i + 1] = writeBuffer[i];
                    }
                    break;
                case ByteOrder.BigEndian:
                    for (int i = 0; i < writeBuffer.Length; i++) {
                        // stuff them backwards
                        WriteBuffer.Span[i + 1] = writeBuffer[writeBuffer.Length - (i + 1)];
                    }
                    break;
            }
            // write it
            this.Bus.Write(this.Address, WriteBuffer.Span[0..(writeBuffer.Length + 1)]);
        }

        public void Exchange(Span<byte> writeBuffer, Span<byte> readBuffer, DuplexType duplex = DuplexType.Half)
        {
            if (duplex == DuplexType.Full) { throw new ArgumentException("I2C doesn't support full-duplex communications. Only half-duplex is available because it only has a single data line."); }

            Bus.Exchange(this.Address, writeBuffer, readBuffer);
        }

        //==== OLD AND BUSTED //TODO: Delete after M.Foundation update

        public byte[] ReadBytes(ushort numberOfBytes)
        {
            return Bus.ReadData(this.Address, numberOfBytes);
        }

        public byte[] ReadRegisters(byte address, ushort length)
        {
            if (length > ReadBuffer.Length) {
                throw new ArgumentException("Read length exceeds RxBuffer size. " +
                    "Please construct with a larger buffer to use this read length.");
            }

            this.WriteBuffer.Span[0] = address;
            Bus.Exchange(this.Address, WriteBuffer.Span[0..1], ReadBuffer.Span[0..length]);
            return ReadBuffer.Slice(0, length).ToArray();
        }

        public ushort[] ReadUShorts(byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian)
        {
            //TODO to @CTACKE from BC; i'm not sure what this method does. i'm also concerned
            // about the allocation of `number * 2` being larger than the buffer.
            // please convert

            // write the register address, then read
            Span<byte> tx = stackalloc byte[1];
            tx[0] = address;
            Span<byte> rx = stackalloc byte[number * 2];

            Bus.Exchange(this.Address, tx, rx);

            var result = new ushort[number];
            for (int i = 0; i < number; i++)
            {
                if (order == ByteOrder.LittleEndian)
                {
                    result[i] = (ushort)(rx[i * 2] | (rx[i * 2 + 1] << 8));
                }
                result[i] = (ushort)((rx[i * 2] << 8) | rx[i * 2 + 1]);
            }

            return result;
        }

        public void WriteBytes(byte[] values)
        {
            this.Bus.WriteData(this.Address, values);
        }


        [Obsolete("This overload of WriteReadData is obsolete for performance reasons and will be removed in a future release.  Migrate to another overload.", false)]
        public byte[] WriteRead(byte[] write, ushort length)
        {
            return this.Bus.WriteReadData(this.Address, length, write);
        }

        public void WriteRegisters(byte address, byte[] data)
        {
            var temp = new List<byte>();
            temp.Add(address);
            temp.AddRange(data);

            this.Bus.WriteData(this.Address, temp);
        }

        public void WriteRegisterUShort(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
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
