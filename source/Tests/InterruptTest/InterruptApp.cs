using System;
using System.Collections.Generic;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace InterruptTest
{
    class InterruptApp : App<F7Micro, InterruptApp>
    {
        private List<IDigitalInputPort> _inputs = new List<IDigitalInputPort>();

        public InterruptApp()
        {
            Console.WriteLine("+InterruptApp");
            ConfigureInterrupts();
        }

        private void ConfigureInterrupts()
        {
            Console.WriteLine("Looking for rising interrupts on D00");
            var d0 = Device.CreateDigitalInputPort(Device.Pins.D00, InterruptMode.EdgeRising);
            d0.DebounceDuration = 1000;
            d0.Resistor = ResistorMode.PullDown;
            d0.Changed += D0_Changed;
            _inputs.Add(d0);

            Console.WriteLine("Looking for falling interrupts on D01");
            var d1 = Device.CreateDigitalInputPort(Device.Pins.D01, InterruptMode.EdgeFalling);
            d1.DebounceDuration = 1000;
            d1.Resistor = ResistorMode.PullUp;
            d1.Changed += D1_Changed;
            _inputs.Add(d1);

            Console.WriteLine("Looking for rising or falling interrupts on D02");
            var d2 = Device.CreateDigitalInputPort(Device.Pins.D02, InterruptMode.EdgeBoth);
            d2.DebounceDuration = 1000;
            d2.Resistor = ResistorMode.PullDown;
            d2.Changed += D2_Changed;
            _inputs.Add(d2);
        }

        void D0_Changed(object sender, DigitalInputPortEventArgs e)
        {
            Console.WriteLine($"Rising Interrupt on D00 @ {e.Time.ToString("s.fff")}");
        }

        void D1_Changed(object sender, DigitalInputPortEventArgs e)
        {
            Console.WriteLine($"Falling Interrupt on D01 @ {e.Time.Ticks}");
        }

        void D2_Changed(object sender, DigitalInputPortEventArgs e)
        {
            Console.WriteLine($"{(e.Value ? "Rising" : "Falling")} Interrupt on D02 @ {e.Time.Ticks}");
        }
    }
}
