using System;
using System.Collections.Generic;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Basic_Digital_Input
{
    class InputApp : App<F7Micro, InputApp>
    {
        private List<IDigitalInputPort> _inputs = new List<IDigitalInputPort>();

        public InputApp()
        {
            ConfigureInputs();
            ShowStates();
        }

        private void ConfigureInputs()
        {
            // we'll create 3 inputs, with each of the available resistor modes
            var d5 = Device.CreateDigitalInputPort(Device.Pins.D05, resistorMode: ResistorMode.Disabled);
            _inputs.Add(d5);
            var d6 = Device.CreateDigitalInputPort(Device.Pins.D06, resistorMode: ResistorMode.PullUp);
            _inputs.Add(d6);
            var d7 = Device.CreateDigitalInputPort(Device.Pins.D07, resistorMode: ResistorMode.PullDown);
            _inputs.Add(d7);
        }

        public void ShowStates()
        {
            // Display the current input states
            // The general idea here is that you have 1 floating, 1 pulled high, and 1 pulled low.
            // With nothing connected, you should have inputs of:
            //   - D05: undetermined
            //   - D06: high
            //   - D07: low
            // You can then drive the outputs with a jumper to either GND or VCC to change their states to high or low
            while (true)
            {
                foreach(var input in _inputs)
                {
                    Console.WriteLine($"{input.Pin.Name}: {(input.State ? "high" : "low")}");
                }

                Thread.Sleep(2000);
            }
        }
    }
}
