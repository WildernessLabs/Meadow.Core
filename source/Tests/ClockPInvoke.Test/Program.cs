using System;
using System.IO;
using System.Threading;
using Meadow.Core.Interop;
using static Meadow.Core.Interop.Interop.Nuttx;

namespace ClockPInvoke.Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Program Started (console)!");

//            ClockTest();
            OnBoardLEDTest();
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

        static void OnBoardLEDTest()
        {
            var driverName = $"/dev/gpio";
            Console.WriteLine($"Opening void driver {driverName}...");
            var handle = Interop.Nuttx.open(driverName, DriverFlags.ReadOnly);
            Console.WriteLine($"Driver handle: {handle.ToString()}");

            var pinState = new GPIOPinState();

            while (true)
            {
                // invert the state
                pinState.State = !pinState.State;
                 
                // set each color (the on-board is a 3-color LED)

                // RED
                pinState.PinNumber = 1;
                Console.WriteLine($"Writing state {pinState.State} to pin {pinState.PinNumber}");
                Interop.Nuttx.ioctl(handle, GpioIoctlFn.WriteSingle, ref pinState);

                Thread.Sleep(500);

                // GREEN
                pinState.PinNumber = 2;
                Console.WriteLine($"Writing state {pinState.State} to pin {pinState.PinNumber}");
                Interop.Nuttx.ioctl(handle, GpioIoctlFn.WriteSingle, ref pinState);

                Thread.Sleep(500);

                // BLUE
                pinState.PinNumber = 3;
                Console.WriteLine($"Writing state {pinState.State} to pin {pinState.PinNumber}");
                Interop.Nuttx.ioctl(handle, GpioIoctlFn.WriteSingle, ref pinState);

                Thread.Sleep(1000);
            }
        }

    }
}
