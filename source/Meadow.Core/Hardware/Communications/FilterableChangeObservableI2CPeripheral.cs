using System;
using Meadow.Units;

namespace Meadow.Hardware
{
    public abstract class FilterableChangeObservableI2CPeripheral<UNIT> :
        FilterableChangeObservableBase<UNIT>, IDisposable where UNIT : struct
    {
        /// <summary>
        /// The peripheral's address on the I2C Bus
        /// </summary>
        public byte Address { get => I2cPeripheral.Address; }

        protected II2cPeripheral I2cPeripheral { get; private set; }

        protected FilterableChangeObservableI2CPeripheral(II2cBus i2cBus, byte address, int rxBufferSize = 8, int txBufferSize = 8)
        {
            I2cPeripheral = new I2cPeripheral(i2cBus, address);
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
}