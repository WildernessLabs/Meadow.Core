using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using System;
using System.Threading;

namespace DigitalIOSample
{
    public class MeadowApp : App<MeadowForLinux<RaspberryPiPinout>, MeadowApp>
    {
        public MeadowApp()
        {
            Initialize();
            Run();
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");
        }

        void Run()
        {
            // this is pin 40 on a Pi4, so last inner pin (easy to clip with a scope)
            var output = Device.CreateDigitalOutputPort(Device.Pins.GPIO21);
            var state = false;

            while (true)
            {
                Console.Write(".");
                output.State = state;
                state = !state;
                Thread.Sleep(1000);
            }
        }
    }
}
