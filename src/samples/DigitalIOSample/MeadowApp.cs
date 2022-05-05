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
            RunLoopback();
        }

        void RunPulse()
        {
            // this is pin 40 on a Pi4, so last outer pin (easy to clip with a scope)
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

        void RunLoopback()
        {
            // this is pin 40 on a Pi4, so last outer pin (easy to clip with a scope)
            var output = Device.CreateDigitalOutputPort(Device.Pins.GPIO21);
            var state = false;

            var input = Device.CreateDigitalInputPort(Device.Pins.GPIO20, Meadow.Hardware.InterruptMode.EdgeBoth, Meadow.Hardware.ResistorMode.Disabled);

            while (true)
            {
                output.State = state;
                Console.WriteLine($"{(state ? 'H' : 'L')} -> {(input.State ? 'H' : 'L')}");

                state = !state;
                Thread.Sleep(1000);
            }
        }
    }
}
