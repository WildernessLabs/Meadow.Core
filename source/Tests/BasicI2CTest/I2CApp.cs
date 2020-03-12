using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace BasicI2CTest
{
    public class I2CApp : App<F7Micro, I2CApp>
    {
        public I2CApp()
        {
            Console.WriteLine("+I2CApp");

            SpeedChangeTest();
        }

        private void SpeedChangeTest()
        {
            Console.WriteLine("+GY521 Speed Change Test");

            var i2c = Device.CreateI2cBus();

            var gyro = new GY521(i2c);

            Console.WriteLine(" Wake");
            gyro.Wake();

            var count = 0;

            while (true)
            {
                try
                {
                    Console.WriteLine($" Reading @{i2c.Frequency / 1000} kHz...");
                    gyro.Refresh();

                    Console.WriteLine($" ({gyro.AccelerationX:X4},{gyro.AccelerationY:X4},{gyro.AccelerationZ:X4}) ({gyro.GyroX:X4},{gyro.GyroY:X4},{gyro.GyroZ:X4}) {gyro.Temperature}");

                    switch (count++ % 4)
                    {
                        case 0:
                            i2c.Frequency = 100000;
                            break;
                        case 1:
                            i2c.Frequency = 400000;
                            break;
                        case 2:
                            i2c.Frequency = 1000000;
                            break;
                        case 3:
                            i2c.Frequency = 3400000;
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($" Error: {ex.Message}");
                }

                Thread.Sleep(2000);
            }
        }

        private void GY521Test()
        {
            var i2c = Device.CreateI2cBus();

            Console.WriteLine("+GY521 Test");

            var gyro = new GY521(i2c);

            Console.WriteLine(" Wake");
            gyro.Wake();

            while (true)
            {
                Console.WriteLine(" Reading...");
                gyro.Refresh();

                Console.WriteLine($" ({gyro.AccelerationX:X4},{gyro.AccelerationY:X4},{gyro.AccelerationZ:X4}) ({gyro.GyroX:X4},{gyro.GyroY:X4},{gyro.GyroZ:X4}) {gyro.Temperature}");

                Thread.Sleep(2000);
            }
        }

        private void BusScan(II2cBus i2c)
        {
            byte addr = 0;
            while (true)
            {
                if (++addr >= 127) addr = 1;

                Console.WriteLine($" Address: {addr}");

                i2c.WriteData(addr, new byte[] { 0 });
                Thread.Sleep(2000);
            }
        }
    }

    public class GY521
    {
        private enum Registers : byte
        {
            PowerManagement = 0x6b,
            AccelerometerX = 0x3b,
            AccelerometerY = 0x3d,
            AccelerometerZ = 0x3f,
            Temperature = 0x41,
            GyroX = 0x43,
            GyroY = 0x45,
            GyroZ = 0x47
        }

        private II2cBus _bus;

        public byte Address { get; }

        public GY521(II2cBus bus, byte address = 0x68)
        {
            Address = address;
            _bus = bus;
        }

        public void Wake()
        {
            _bus.WriteData(Address, (byte)Registers.PowerManagement, 0);
        }

        int c = 0;

        public void Refresh()
        {
            // tell it to send us 14 bytes (each value is 2-bytes), starting at 0x3b
            byte address = c++ % 10 == 0 ? (byte)(Address + 1) : Address;

            // cause occasional errors
            var data = _bus.WriteReadData(address, 14, (byte)Registers.AccelerometerX);

//            Console.WriteLine($" Got {data.Length} bytes");
//            Console.WriteLine($" {BitConverter.ToString(data)}");

            AccelerationX = data[0] << 8 | data[1];
            AccelerationY = data[2] << 8 | data[3];
            AccelerationZ = data[4] << 8 | data[5];
            Temperature = data[6] << 8 | data[7];
            GyroX = data[8] << 8 | data[9];
            GyroY = data[10] << 8 | data[11];
            GyroZ = data[12] << 8 | data[13];
        }

        public int AccelerationX { get; private set; }
        public int AccelerationY { get; private set; }
        public int AccelerationZ { get; private set; }
        public int Temperature { get; private set; }
        public int GyroX { get; private set; }
        public int GyroY { get; private set; }
        public int GyroZ { get; private set; }
    }
}
