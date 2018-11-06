using System;

namespace Meadow.Hardware
{
    public class I2CDevice : IDisposable
    {
        public Configuration Config;
        protected bool m_disposed;

        public I2CDevice(Configuration config) { throw new NotImplementedException(); }

        ~I2CDevice() { throw new NotImplementedException(); }

        public static I2CReadTransaction CreateReadTransaction(byte[] buffer) { throw new NotImplementedException(); }
        public static I2CWriteTransaction CreateWriteTransaction(byte[] buffer) { throw new NotImplementedException(); }
        public void Dispose() { throw new NotImplementedException(); }
        public int Execute(I2CTransaction[] xActions, int timeout) { throw new NotImplementedException(); }

        public class Configuration
        {
            public readonly ushort Address;
            public readonly int ClockRateKhz;

            public Configuration(ushort address, int clockRateKhz) { throw new NotImplementedException(); }
        }
        public sealed class I2CReadTransaction : I2CTransaction
        {
        }
        public class I2CTransaction
        {
            public readonly byte[] Buffer;

          //  protected I2CTransaction(byte[] buffer) { }
        }
        public sealed class I2CWriteTransaction : I2CTransaction
        {
        }
    }
}
