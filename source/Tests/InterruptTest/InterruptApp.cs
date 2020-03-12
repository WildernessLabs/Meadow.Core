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

//            WireInterrupt(Device.Pins.D02, output);
            WireInterrupt(Device.Pins.D03, output);
            WireInterrupt(Device.Pins.D04, output);
        }

        private void WireInterrupt(IPin pin, IDigitalOutputPort output)
        {
            var input = Device.CreateDigitalInputPort(
                pin,
                InterruptMode.EdgeRising,
                ResistorMode.PullDown,
                20);

            input.Changed += async (s, o) =>
            {
                output.State = true;
                await Task.Delay(1000);
                output.State = false;
                Console.WriteLine($"{++_count}: {(s as DigitalInputPort).Channel.Name} interrupt");
            };

        }
    }
}
