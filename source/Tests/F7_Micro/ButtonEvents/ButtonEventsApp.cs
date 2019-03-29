using System;
using Meadow;
using Meadow.Hardware;
using Meadow.Devices;

namespace ButtonEvents
{
    /// <summary>
    /// This sample illustrates simple input events using a PushButton. To wire up, add
    /// a PushButton connected to D02, with the circuit terminating on the 3.3V rail, so that
    /// when the button is pressed, the input is raised high. Because internal pull-downs are
    /// not currently working, add a 10k pull-down resistor to the input side as illustrated in
    /// http://developer.wildernesslabs.co/Hardware/Tutorials/Electronics/Part4/PullUp_PullDown_Resistors/
    /// </summary>
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
