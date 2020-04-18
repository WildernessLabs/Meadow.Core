﻿using System;
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
                Device.Pins.D15, false);

            WireUpInterrupt(Device.Pins.D00, output); // PI9
            WireUpInterrupt(Device.Pins.D01, output); // PH13
            WireUpInterrupt(Device.Pins.D02, output); // PC6
            WireUpInterrupt(Device.Pins.D03, output); // PB8
        }

        private void WireUpInterrupt(IPin pin, IDigitalOutputPort output)
        {
            var input = Device.CreateDigitalInputPort(
                pin,
                InterruptMode.EdgeFalling,
                ResistorMode.PullDown,
                20,   // debounce duration
                2);   // glitch filter

            input.Changed += async (s, o) =>
            {
                output.State = true;
                await Task.Delay(1000);
                output.State = false;
                Console.WriteLine($"InterruptApp:{++_count:D4} Mono:{(s as DigitalInputPort).Channel.Name} interrupt");
            };

        }
    }
}
