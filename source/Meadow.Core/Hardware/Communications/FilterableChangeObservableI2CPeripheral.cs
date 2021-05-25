using System;
using Meadow.Units;

namespace Meadow.Hardware
{
    public abstract class FilterableChangeObservableI2CPeripheral<UNIT> : FilterableChangeObservableBase<UNIT>, IDisposable
        where UNIT : struct
    {
        /// <summary>
        /// The peripheral's address on the I2C Bus
        /// </summary>
        public byte Address { get => Bus.Address; }

        protected I2CBusAccessor Bus { get; private set; }

        protected FilterableChangeObservableI2CPeripheral(II2cBus i2cBus, byte address, int rxBufferSize = 8, int txBufferSize = 8)
        {
            Bus = new I2CBusAccessor(i2cBus, address, rxBufferSize, txBufferSize);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Dispose managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }

    public class I2CBusAccessor
    {
        private object SyncRoot { get; } = new object();
        private byte[] _txBuffer;
        private byte[] _rxBuffer;

        public II2cBus Bus { get; private set; }
        public byte Address { get; private set; }

        public I2CBusAccessor(II2cBus i2cBus, byte address, int rxBufferSize = 8, int txBufferSize = 8)
        {
            Bus = i2cBus;
            Address = address;

            _rxBuffer = new byte[rxBufferSize];
            _txBuffer = new byte[txBufferSize];
        }

        public byte ReadRegisterByte(byte register)
        {
            lock (SyncRoot)
            {
                _txBuffer[0] = (byte)register;
                Bus.WriteReadData(Address, _txBuffer, 1, _rxBuffer, 1);
                return _rxBuffer[0];
            }
        }

        public ushort ReadRegisterShort(byte register)
        {
            lock (SyncRoot)
            {
                _txBuffer[0] = (byte)register;
                Bus.WriteReadData(Address, _txBuffer, 1, _rxBuffer, 2);
                return (ushort)(_rxBuffer[0] << 8 | _rxBuffer[1]);
            }
        }

        public uint ReadRegisterInt(byte register)
        {
            lock (SyncRoot)
            {
                _txBuffer[0] = (byte)register;
                Bus.WriteReadData(Address, _txBuffer, 1, _rxBuffer, 4);
                return (uint)(_rxBuffer[0] << 24 | _rxBuffer[1] << 16 | _rxBuffer[2] << 8 | _rxBuffer[3]);
            }
        }

        public void ReadRegisterBytes(byte register, byte[] readBuffer)
        {
            ReadRegisterBytes(register, readBuffer, readBuffer.Length);
        }

        public void ReadRegisterBytes(byte register, byte[] readBuffer, int bytesToRead)
        {
            lock (SyncRoot)
            {
                _txBuffer[0] = register;
                Bus.WriteReadData(Address, _txBuffer, 1, readBuffer, bytesToRead);
            }
        }

        public void WriteRegister(byte register, byte value)
        {
            lock (SyncRoot)
            {
                _txBuffer[0] = register;
                _txBuffer[1] = value;
                Bus.WriteData(Address, _txBuffer, 2);
            }
        }

        public void WriteRegister(byte register, ushort value, bool littleEndian = true)
        {
            _txBuffer[0] = register;
            if (littleEndian)
            {
                _txBuffer[1] = (byte)(value & 0xff);
                _txBuffer[2] = (byte)((value >> 8) & 0xff);
            }
            else
            {
                _txBuffer[1] = (byte)((value >> 8) & 0xff);
                _txBuffer[2] = (byte)(value & 0xff);
            }
            lock (SyncRoot)
            {
                Bus.WriteData(Address, _txBuffer, 3);
            }
        }

        public void WriteBytes(params byte[] data)
        {
            lock (SyncRoot)
            {
                Bus.WriteData(Address, data, data.Length);
            }
        }

        public void ReadBytes(byte[] buffer)
        {
            lock (SyncRoot)
            {
                var c = Bus.ReadData(Address, buffer.Length);
                Array.Copy(c, buffer, buffer.Length);
            }
        }

        public void ReadBytes(byte[] buffer, int readCount)
        {
            lock (SyncRoot)
            {
                var c = Bus.ReadData(Address, readCount);
                Array.Copy(c, buffer, readCount);
            }
        }

        public void WriteReadData(Span<byte> writeBuffer, int writeCount, Span<byte> readBuffer, int readCount)
        {
            lock (SyncRoot)
            {
                Bus.WriteReadData(Address, writeBuffer, writeCount, readBuffer, readCount);
            }
        }

    }
}
