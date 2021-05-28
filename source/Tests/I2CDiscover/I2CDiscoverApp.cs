using System;
using System.Collections.Generic;
using System.IO;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Benchmarks
{
    public class I2CDiscoverApp : App<MeadowOnLinux<JetsonNanoPinout>, I2CDiscoverApp>
    {
        public I2CDiscoverApp()
        {
            Console.WriteLine("Meadow I2C Discovery on NVIDIA Jetson Nano");

            var bus = Device.CreateI2cBus();

            Console.WriteLine("Testing complete");
        }
    }
}
