using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace InterruptTest
{
    class InterruptApp : App<F7Micro, InterruptApp>
    {
        private List<IDigitalInputPort> _inputs = new List<IDigitalInputPort>();
        private int _count = 0;

        public InterruptApp()
        {
            Console.WriteLine("+InterruptApp");

            var output = Device.CreateDigitalOutputPort(
                Device.Pins.D05, false);

            //             Monitored inputs
            WireUpInterrupt(Device.Pins.D00, output); // PI9
            WireUpInterrupt(Device.Pins.D01, output); // PH13
            WireUpInterrupt(Device.Pins.D02, output); // PC6
            WireUpInterrupt(Device.Pins.D03, output); // PB8
        }

        private void WireUpInterrupt(IPin pin, IDigitalOutputPort output)
        {
            var input = Device.CreateDigitalInputPort(
                pin,
                InterruptMode.EdgeRising,
                ResistorMode.PullUp,
                0,          // debounce duration
                20);        // glitch filter

            Console.WriteLine($"InterruptApp: Input config complete for:{pin.Name}");
            input.Changed += async (s, o) =>
            {
                // This is for testing that the DebounceDuration and GlitchDuration
                // properties can be changed by the app
                //if ((_count % 5) == 0)
                //{
                //    bool _changed = false;
                //    if (input.DebounceDuration == 21)
                //    {
                //        input.DebounceDuration = 41;
                //        _changed = true;
                //    }
                //    else if (input.DebounceDuration == 41)
                //    {
                //        input.DebounceDuration = 21;
                //        _changed = true;
                //    }

                //    if (input.GlitchDuration == 21)
                //    {
                //        input.GlitchDuration = 41;
                //        _changed = true;
                //    }
                //    else if (input.GlitchDuration == 41)
                //    {
                //        input.GlitchDuration = 21;
                //        _changed = true;
                //    }

                //    if (_changed)
                //      Console.WriteLine($"{_count:D4} ***** Changed debounce and/or glitch durations *****");
                //}

                output.State = true;    // flash LED for Task.Delay
                await Task.Delay(1000);
                output.State = false;
                Console.WriteLine($"InterruptApp:{++_count:D4} Mono:{(s as DigitalInputPort).Channel.Name} interrupt");
            };
        }
    }
}
