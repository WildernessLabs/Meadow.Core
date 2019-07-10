using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace GPIO_PertTest
{
    class MainClass
    {
        static IApp _app;

        public static void Main(string[] args)
        {
            _app = new PerfApp();

            while(true)
            {
                Thread.Sleep(10000);
            }
        }
    }

    class PerfApp : AppBase<F7Micro, PerfApp>
    {
        IDigitalOutputPort _d03;
        IDigitalInputPort _d02;

        public PerfApp()
        {
            Console.WriteLine("Starting perf test");

            _d03 = Device.CreateDigitalOutputPort(Device.Pins.D03);
            _d02 = Device.CreateDigitalInputPort(Device.Pins.D02, InterruptMode.EdgeRising);

            _d02.Changed += (s, e) =>
            {
                _d03.State = true;
                _d03.State = false;
            };
        }
    }
}
