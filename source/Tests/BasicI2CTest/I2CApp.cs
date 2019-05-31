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
            byte addr = 0;
            while (true)
            {
                if (count++ % 5 == 0)
                {
//                    i2c.Reset();
//                    Thread.Sleep(2000);
                }
                if (++addr >= 128) addr = 1;

//                i2c.WriteBytes(0x42, new byte[] { 1, 2, 3, 4, 5 });
                i2c.WriteBytes(addr, new byte[] { });
                Thread.Sleep(2000);
            }
        }
    }
}
