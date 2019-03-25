using System;
using Meadow;
using Meadow.Hardware;
using Meadow.Devices;

namespace ButtonEvents
{
    public class ButtonEventsApp : AppBase<F7Micro, ButtonEventsApp>
    {
        IDigitalInputPort _input;

        public ButtonEventsApp()
        {
            _input = Device.CreateDigitalInputPort(Device.Pins.D02, InterruptMode.EdgeBoth, debounceDuration: 20);
            _input.Changed += Input_Changed;
        }

        private void Input_Changed(object sender, DigitalInputPortEventArgs e)
        {
            Console.WriteLine("Changed: " + e.Value.ToString() + ", Time: " + e.Time.ToString());
        }
    }
}
