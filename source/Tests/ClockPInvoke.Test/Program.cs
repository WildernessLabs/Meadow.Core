using System;
using System.IO;
using Meadow.Core.Interop;

namespace ClockPInvoke.Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Program Started (console)!");

            ClockTest();
            GPIOTest();
        }

        static void ClockTest()
        {
//            while (true)
//            {
                Console.WriteLine("Before clock_gettime");

                Interop.Nuttx.timespec ts = new Interop.Nuttx.timespec();
                var time = Interop.Nuttx.clock_gettime(Interop.Nuttx.clockid_t.CLOCK_REALTIME, ref ts);

                Console.WriteLine("clock_gettime result: " + ts.tv_sec.ToString());

                System.Threading.Thread.Sleep(1000);
//            }
        }

        static void GPIOTest()
        {
            var names = new string[]
            {
//                "/dev/gpin",
                "/dev/gpout",
//                "/dev/gpint"
            };

            foreach(var n in names)
            {
                for (int i = 1; i < 2; i++)
                {
                    var driverName = $"{n}{i}";
                    Console.WriteLine($"Opening void driver {driverName}...");
                    var handle = Interop.Nuttx.open_void(driverName, 2);
                    Console.WriteLine($"Driver handle: {handle.ToString()}");
                    /*
                    if (handle != Interop.Nuttx.INVALID_HANDLE)
                    {
                        Console.WriteLine($"Reading pin type...");
                        var result = Interop.Nuttx.ioctl(handle, Interop.Nuttx.GPIOC_PINTYPE, out Interop.Nuttx.GPIOPinType pinType);
                        Console.WriteLine($"Ioctl result: {result}");
                        Console.WriteLine($"Pin type: {pinType.ToString()} [{pinType}]");

                        Console.WriteLine($"Closing driver...");
                        Interop.Nuttx.close(handle);
                    }
                    */
                }
            }
        }

        static void LEDTest()
        {
            var driverName = "/dev/gpout1";

            // open the driver
            // fd = open(devpath, O_RDWR);
            Console.WriteLine($"Opening void driver {driverName}...");
            var handle = Interop.Nuttx.open_void(driverName, 2);
            Console.WriteLine($"Driver handle: {handle.ToString()}");
            /*
            // read the pin type
            // et = ioctl(fd, GPIOC_PINTYPE, (unsigned long)((uintptr_t)&pintype));
            Console.WriteLine($"Reading pin type...");
            var result = Interop.Nuttx.ioctl(handle, Interop.Nuttx.GPIOC_PINTYPE, out Interop.Nuttx.GPIOPinType pinType);
            Console.WriteLine($"Ioctl result: {result}");
            Console.WriteLine($"Pin type: {pinType.ToString()} [{pinType}]");
            */
            // read the pin value
            // ret = ioctl(fd, GPIOC_READ, (unsigned long)((uintptr_t) & invalue));

            // write the output
            // ret = ioctl(fd, GPIOC_WRITE, (unsigned long)outvalue);

            // close the driver
            // close(fd);
            Console.WriteLine($"Closing driver...");
//            Interop.Nuttx.close(handle);
            Console.WriteLine($"Done.");

        }

    }
}
