using System;
using Meadow;
using Meadow.Hardware;
using Meadow.Devices;

namespace DigitalInputPort_IObservable_Sample
{
    /// <summary>
    /// This sample illustrates using the IFilterableObserver pattern. To wire up, add
    /// a PushButton connected to D02, with the circuit terminating on the 3.3V rail, so that
    /// when the button is pressed, the input is raised high. Because internal pull-downs are
    /// not currently working, add a 10k pull-down resistor to the input side as illustrated in
    /// http://developer.wildernesslabs.co/Hardware/Tutorials/Electronics/Part4/PullUp_PullDown_Resistors/
    /// </summary>
    public class InputObservableApp : AppBase<F7Micro, InputObservableApp>
    {
        IDigitalInputPort _input;

        public InputObservableApp()
        {
            // create an input port on D02. 
            _input = Device.CreateDigitalInputPort(Device.Pins.D02);

            // Traditional event
            _input.Changed += (object sender, DigitalInputPortEventArgs e) => {
                Console.WriteLine($"Old school event raised; Time: {e.Time.Millisecond}, Value: {e.Value}");
            };

            // this illustrates using a FilterableObserver. Note that the filter is an optional
            // parameter, if you're interested in all notifications, don't pass a filter/predicate.
            // in this case, we filter on events by time, and only notify if the new event is > 1 second from
            // the last event. 
            _input.Subscribe(new FilterableObserver<DigitalInputPortEventArgs>(
                e => {
                    Console.WriteLine($"Observer Observing the Observable, Observably speaking, Time: {e.Time.Millisecond}, Value: {e.Value}");
                },
                // Optional filter paramter, showing a 1 second filter, i.e., only notify
                // if the new event is > 1 second from last.
                f => {
                    return (f.Time - f.PreviousTime > new TimeSpan(0, 0, 0, 0, 1000));
                }));

            Console.WriteLine("Got here 3.");

        }
    }
}
