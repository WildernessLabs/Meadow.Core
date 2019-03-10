//using System;
//using System.IO;
//using System.Threading;
//using Meadow.Core.Interop;
//using static Meadow.Core.Interop.Interop.Nuttx;

namespace ClockPInvoke.Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
//            Console.WriteLine("Program Started (console)!");

////            ClockTest();
//            OnBoardLEDTest();
        }

//        static void ClockTest()
//        {
//            for(int i = 0; i < 10; i++)
//            { 
//                Console.WriteLine("Before clock_gettime");

//                    Interop.Nuttx.timespec ts = new Interop.Nuttx.timespec();
//                    var time = Interop.Nuttx.clock_gettime(Interop.Nuttx.clockid_t.CLOCK_REALTIME, ref ts);

//                    Console.WriteLine("clock_gettime result: " + ts.tv_sec.ToString());

//                    System.Threading.Thread.Sleep(1000);
//            }
//        }

//        static void OnBoardLEDTest()
//        {
//            var driverName = $"/dev/gpio";
//            Console.WriteLine($"Opening void driver {driverName}...");
//            var handle = Interop.Nuttx.open(driverName, DriverFlags.ReadOnly);
//            Console.WriteLine($"Driver handle: {handle.ToString()}");

//            // initialize the GPIOs (this needs to be in some class initialization)
//            var ledBlueInit = GPIOConfigFlags.Pin0 | GPIOConfigFlags.PortA | GPIOConfigFlags.OutputInitialValueHigh | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
//            var ledGreenInit = GPIOConfigFlags.Pin1 | GPIOConfigFlags.PortA | GPIOConfigFlags.OutputInitialValueHigh | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
//            var ledRedInit = GPIOConfigFlags.Pin2 | GPIOConfigFlags.PortA | GPIOConfigFlags.OutputInitialValueHigh | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;

//            Console.WriteLine($"Doing pin initializations...");
//            Interop.Nuttx.ioctl(handle, GpioIoctlFn.SetConfig, ref ledBlueInit);
//            Interop.Nuttx.ioctl(handle, GpioIoctlFn.SetConfig, ref ledGreenInit);
//            Interop.Nuttx.ioctl(handle, GpioIoctlFn.SetConfig, ref ledRedInit);

//            var pinState = new GPIOPinState();

//            while (true)
//            {
//                // invert the state
//                pinState.State = !pinState.State;
                 
//                // set each color (the on-board is a 3-color LED)

//                pinState.PinDesignator = PinDesignator.PortA | PinDesignator.Pin0;
//                Console.WriteLine($"Writing state {pinState.State} to pin {pinState.PinDesignator.ToString()}");
//                Interop.Nuttx.ioctl(handle, GpioIoctlFn.Write, ref pinState);

//                Thread.Sleep(500);

//                pinState.PinDesignator = PinDesignator.PortA | PinDesignator.Pin1;
//                Console.WriteLine($"Writing state {pinState.State} to pin {pinState.PinDesignator.ToString()}");
//                Interop.Nuttx.ioctl(handle, GpioIoctlFn.Write, ref pinState);

//                Thread.Sleep(500);

//                pinState.PinDesignator = PinDesignator.PortA | PinDesignator.Pin2;
//                Console.WriteLine($"Writing state {pinState.State} to pin {pinState.PinDesignator.ToString()}");
//                Interop.Nuttx.ioctl(handle, GpioIoctlFn.Write, ref pinState);

//                Thread.Sleep(1000);
//            }
//        }

    }
}
