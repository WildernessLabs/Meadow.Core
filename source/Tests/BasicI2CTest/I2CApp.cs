using System;
using System.Threading;
using Meadow;
using Meadow.Devices;

namespace BasicI2CTest
{
    public class I2CApp : App<F7Micro, I2CApp>
    {
        public I2CApp()
        {
            Console.WriteLine("+I2CApp");

            var i2c = Device.CreateI2cBus();
            byte addr = 0;
            while (true)
            {
                if (++addr >= 127) addr = 1;

                Console.WriteLine($" Address: {addr}");

                i2c.WriteBytes(addr, new byte[] { 0 });
                Thread.Sleep(2000);
            }
        }
    }
}
