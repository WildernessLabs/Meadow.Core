using Meadow.Units;

namespace Meadow.Hardware
{

    public class FilterableChangeObservableI2CPeripheral<T, U1> : FilterableChangeObservable<T, U1>
        where T : CompositeChangeResult<U1>
        where U1 : IUnitType
    {
        protected I2CBusAccessor Bus { get; private set; }

        protected FilterableChangeObservableI2CPeripheral(II2cBus i2cBus, byte address, int rxBufferSize = 8, int txBufferSize = 8)
        {
            Bus = new I2CBusAccessor(i2cBus, address, rxBufferSize, txBufferSize);
        }
    }

    public class FilterableChangeObservableI2CPeripheral<T, U1, U2> : FilterableChangeObservable<T, U1, U2>
        where T : CompositeChangeResult<U1, U2>
        where U1 : IUnitType
        where U2 : IUnitType
    {
        protected I2CBusAccessor Bus { get; private set; }

        protected FilterableChangeObservableI2CPeripheral(II2cBus i2cBus, byte address, int rxBufferSize = 8, int txBufferSize = 8)
        {
            Bus = new I2CBusAccessor(i2cBus, address, rxBufferSize, txBufferSize);
        }
    }

    public class I2CBusAccessor
    { 
        private byte[] _txBuffer;
        private byte[] _rxBuffer;

        protected II2cBus Bus { get; private set; }
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
            _txBuffer[0] = (byte)register;
            Bus.WriteReadData(Address, _txBuffer, 1, _rxBuffer, 1);
            return _rxBuffer[0];
        }

        public ushort ReadRegisterShort(byte register)
        {
            _txBuffer[0] = (byte)register;
            Bus.WriteReadData(Address, _txBuffer, 1, _rxBuffer, 2);
            return (ushort)(_rxBuffer[0] << 8 | _rxBuffer[1]);
        }

        public uint ReadRegisterInt(byte register)
        {
            _txBuffer[0] = (byte)register;
            Bus.WriteReadData(Address, _txBuffer, 1, _rxBuffer, 4);
            return (uint)(_rxBuffer[0] << 24 | _rxBuffer[1] << 16 | _rxBuffer[2] << 8 | _rxBuffer[3]);
        }

        public void ReadRegisterBytes(byte register, byte[] outputBuffer)
        {
            ReadRegisterBytes(register, outputBuffer, outputBuffer.Length);
        }

        public void ReadRegisterBytes(byte register, byte[] outputBuffer, int bytesToRead)
        {
            _txBuffer[0] = register;
            Bus.WriteReadData(Address, _txBuffer, 1, outputBuffer, bytesToRead);
        }

        public void WriteRegister(byte register, byte value)
        {
            _txBuffer[0] = register;
            _txBuffer[1] = value;
            Bus.WriteReadData(Address, _txBuffer, 2, null, 0);
        }

        public void WriteData(byte[] data)
        {
            Bus.WriteReadData(Address, data, data.Length, null, 0);
        }
    }
}
