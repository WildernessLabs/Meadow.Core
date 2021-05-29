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
        private const byte READ_ID_PART1 = 0xfa;
        private const byte READ_ID_PART2 = 0x0f;
        private const byte READ_2ND_ID_PART1 = 0xfc;
        private const byte READ_2ND_ID_PART2 = 0xc9;

        public I2CDiscoverApp()
        {
            Console.WriteLine("Meadow I2C Discovery on NVIDIA Jetson Nano");

            using(var bus = Device.CreateI2cBus(1))
            {

                // reset
                bus.WriteData(0x40, 0xFE);
                // read the serial number

                Span<byte> tx = stackalloc byte[2];
                Span<byte> rx = stackalloc byte[8];

                //
                //  Get the device ID.
                var SerialNumber = 0;

                // this device is...interesting.  Most registers are 1-bye addressing, but a few are 2-bytes?
                tx[0] = READ_ID_PART1;
                tx[1] = READ_ID_PART2;
                bus.WriteReadData(0x40, tx, 2, rx, 8);
                for (var index = 0; index < 4; index++)
                {
                    SerialNumber <<= 8;
                    SerialNumber += rx[index * 2];
                }

                tx[0] = READ_2ND_ID_PART1;
                tx[1] = READ_2ND_ID_PART2;
                bus.WriteReadData(0x40, tx, 2, rx, 8);

                SerialNumber <<= 8;
                SerialNumber += rx[0];
                SerialNumber <<= 8;
                SerialNumber += rx[1];
                SerialNumber <<= 8;
                SerialNumber += rx[3];
                SerialNumber <<= 8;
                SerialNumber += rx[4];
                if ((rx[0] == 0) || (rx[0] == 0xff))
                {
    //                SensorType = DeviceType.EngineeringSample;
                }
                else
                {
    //                SensorType = (DeviceType)rx[0];
                }
            }
            Console.WriteLine("Testing complete");
        }
    }
}
