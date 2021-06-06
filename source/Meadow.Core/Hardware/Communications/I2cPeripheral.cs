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

        protected readonly Memory<byte> TxBuffer;
        protected readonly Memory<byte> RxBuffer;

        public I2cPeripheral(II2cBus bus, byte peripheralAddress, int rxBufferSize = 8, int txBufferSize = 8)
        {
            Bus = bus;
            Address = peripheralAddress;
            TxBuffer = new byte[txBufferSize];
            RxBuffer = new byte[rxBufferSize];
        }

        // NEW
        public void Read(Span<byte> readBuffer)
        {
            Bus.ReadData(this.Address, readBuffer);
        }

        public byte[] ReadBytes(ushort numberOfBytes)
        {
            return Bus.ReadData(this.Address, numberOfBytes);
        }

        // NEW
        public void ReadRegister(byte address, Span<byte> readBuffer)
        {
            TxBuffer.Span[0] = address;
            Bus.WriteReadData(this.Address, TxBuffer.Span, 1, readBuffer, 1);
        }

        public byte ReadRegister(byte address)
        {
            // write the register address, then read
            //Span<byte> tx = stackalloc byte[1];
            //tx[0] = address;
            //Span<byte> rx = stackalloc byte[1];

            //Bus.WriteReadData(this.Address, tx, rx);
            //return rx[0];

            this.TxBuffer.Span[0] = address;
            Bus.WriteReadData(this.Address, TxBuffer.Span, 1, RxBuffer.Span, 1);
            return RxBuffer.Span[0];
        }

        public byte[] ReadRegisters(byte address, ushort length)
        {
            //// write the register address, then read
            //Span<byte> tx = stackalloc byte[1];
            //tx[0] = address;
            //Span<byte> rx = stackalloc byte[length];

            //Bus.WriteReadData(this.Address, tx, rx);
            //return rx.ToArray();

            if (length > RxBuffer.Length) {
                throw new ArgumentException("Read length exceeds RxBuffer size. " +
                    "Please construct with a larger buffer to use this read length.");
            }

            this.TxBuffer.Span[0] = address;
            Bus.WriteReadData(this.Address, TxBuffer.Span, 1, RxBuffer.Span, length);
            return RxBuffer.Slice(0, length).ToArray();
        }

        public ushort ReadUShort(byte address, ByteOrder order = ByteOrder.LittleEndian)
        {
            //// write the register address, then read
            //Span<byte> tx = stackalloc byte[1];
            //tx[0] = address;
            //Span<byte> rx = stackalloc byte[2];

            //Bus.WriteReadData(this.Address, tx, rx);
            //if (order == ByteOrder.LittleEndian)
            //{
            //    return (ushort)(rx[0] | (rx[1] << 8));
            //}
            //return (ushort)((rx[0] << 8) | rx[1]);

            // TODO to @CTACKE from BC: please confirm this code does the above
            TxBuffer.Span[0] = address;
            Bus.WriteReadData(this.Address, TxBuffer.Span, RxBuffer.Slice(0, 2).Span);
            if (order == ByteOrder.LittleEndian) {
                return (ushort)(RxBuffer.Span[0] | (RxBuffer.Span[1] << 8));
            } else {
                return (ushort)(RxBuffer.Span[0] << 8 | RxBuffer.Span[1]);
            }

            // ALSO: for comparison, this was the code in your other class, not
            // sure which is more efficient.
    //        _txBuffer[0] = register;
    //        if (littleEndian)
    //        {
    //            _txBuffer[1] = (byte)(value & 0xff);
    //            _txBuffer[2] = (byte)((value >> 8) & 0xff);
    //        }
    //        else
    //        {
    //            _txBuffer[1] = (byte)((value >> 8) & 0xff);
    //            _txBuffer[2] = (byte)(value & 0xff);
    //        }
    //        lock (SyncRoot)
    //        {
    //            Bus.WriteData(Address, _txBuffer, 3);
    //        }
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

            Bus.WriteReadData(this.Address, tx, rx);

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

        public void WriteByte(byte value)
        {
            this.Bus.WriteData(this.Address, value);
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

        public void WriteRead(Span<byte> writeBuffer, Span<byte> readBuffer)
        {
            Bus.WriteReadData(this.Address, writeBuffer, readBuffer);
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
