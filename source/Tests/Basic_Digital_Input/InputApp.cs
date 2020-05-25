using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            double debounceDuration = 500;
            var d4 = Device.CreateDigitalInputPort(Device.Pins.D04, InterruptMode.EdgeBoth, ResistorMode.Disabled);
            d4.DebounceDuration = debounceDuration;
            d4.Changed += OnStateChangedHandler;
            _inputs.Add(d4);
            // since we're looking for falling, pull it up
            var d3 = Device.CreateDigitalInputPort(Device.Pins.D03, InterruptMode.EdgeFalling, ResistorMode.PullUp);
            d3.DebounceDuration = debounceDuration;
            d3.Changed += OnStateChangedHandler;
            _inputs.Add(d3);
            // since we're looking for risinging, pull it down
            var d2 = Device.CreateDigitalInputPort(Device.Pins.D02, InterruptMode.EdgeRising, ResistorMode.PullDown);
            d2.DebounceDuration = debounceDuration;
            d2.Changed += OnStateChangedHandler;
            _inputs.Add(d2);
        }

        private void OnStateChangedHandler(object sender, DigitalInputPortEventArgs e)
        {
            var port = sender as IDigitalInputPort;

            if (port == null)
            {
                Console.WriteLine($"sender is a {port.GetType().Name}");
            }
            else
            {
                Console.WriteLine($"{port.Pin.Name} state changed to {e.Value}");
            }
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
                var line1 = string.Join(" ", _inputs.Select(i => i.Pin.Name).ToArray());
                var line2 = string.Join(" ", _inputs.Select(i => $" {(i.State ? 1 : 0)} ").ToArray());

                Console.WriteLine(line1);
                Console.WriteLine(line2 + "\n");

                Thread.Sleep(2000);
            }
        }
    }
}
