using Meadow;
using Meadow.Pinouts;
using System;
using System.Threading;

namespace DigitalIOSample
{
    public class MeadowApp : App<MeadowForLinux<SnickerdoodleBlack>>
    {
        public MeadowApp()
        {
            RunPulse();
            //RunLoopback();
            //CheckResistor();
        }

        void CheckResistor()
        {
            //            var input20 = Device.CreateDigitalInputPort(Device.Pins.GPIO20, Meadow.Hardware.InterruptMode.None, Meadow.Hardware.ResistorMode.InternalPullUp);
            //            var input21 = Device.CreateDigitalInputPort(Device.Pins.GPIO21, Meadow.Hardware.InterruptMode.None, Meadow.Hardware.ResistorMode.InternalPullDown);

            //            Console.WriteLine($"20 is {input20.State}");
            //            Console.WriteLine($"21 is {input21.State}");
        }

        void RunPulse()
        {
            //            var output0 = Device.CreateDigitalOutputPort(Device.Pins.GPIO0);
            var output1 = Device.CreateDigitalOutputPort(Device.Pins.GPIO1);
            var output2 = Device.CreateDigitalOutputPort(Device.Pins.GPIO2);
            var output3 = Device.CreateDigitalOutputPort(Device.Pins.GPIO3);
            var output4 = Device.CreateDigitalOutputPort(Device.Pins.GPIO4);
            var output5 = Device.CreateDigitalOutputPort(Device.Pins.GPIO5);
            var output6 = Device.CreateDigitalOutputPort(Device.Pins.GPIO6);
            var output7 = Device.CreateDigitalOutputPort(Device.Pins.GPIO7);
            var output8 = Device.CreateDigitalOutputPort(Device.Pins.GPIO8);
            //            var output9 = Device.CreateDigitalOutputPort(Device.Pins.GPIO9);
            var state = false;

            while (true)
            {
                Console.Write(".");
                //                output0.State = state;
                output1.State = state;
                output2.State = state;
                output3.State = state;
                output4.State = state;
                output5.State = state;
                output6.State = state;
                output7.State = state;
                output8.State = state;
                //                output9.State = state;

                state = !state;
                Thread.Sleep(1000);
            }
        }

        void RunLoopback()
        {
            /*
            // this is pin 40 on a Pi4, so last outer pin (easy to clip with a scope)
            var output = Device.CreateDigitalOutputPort(Device.Pins.GPIO21);
            var state = false;

            var input = Device.CreateDigitalInputPort(Device.Pins.GPIO20, Meadow.Hardware.InterruptMode.EdgeRising, Meadow.Hardware.ResistorMode.InternalPullUp);
            input.Changed += (s, e) =>
            {
                Console.WriteLine($"Interrupt: {e.New.State}");
            };

            while (true)
            {
                output.State = state;
                Console.WriteLine($"{(state ? 'H' : 'L')} -> {(input.State ? 'H' : 'L')}");

                state = !state;
                Thread.Sleep(1000);
            }
            */
        }
    }
}
