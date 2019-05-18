using System.Threading;
using Meadow;
using Meadow.Devices;

namespace BasicI2CTest
{
    public class I2CApp : App<F7Micro, I2CApp>
    {
        public I2CApp()
        {
            var count = 0;

            var i2c = Device.CreateI2cBus();
            while (true)
            {
                if (count++ % 5 == 0)
                {
                    i2c.Reset();
                    Thread.Sleep(2000);
                }
//                i2c.WriteBytes(0x42, new byte[] { 1, 2, 3, 4, 5 });
                i2c.WriteBytes(0x42, new byte[] { 1 });
                Thread.Sleep(2000);
            }
        }
    }
}
