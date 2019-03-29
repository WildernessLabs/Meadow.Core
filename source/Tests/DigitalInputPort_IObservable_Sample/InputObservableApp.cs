using System;
using Meadow;
using Meadow.Hardware;
using Meadow.Devices;

namespace DigitalInputPort_IObservable_Sample
{
    public class InputObservableApp : AppBase<F7Micro, InputObservableApp>
    {
        IDigitalInputPort _input;

        public InputObservableApp()
        {
            _input = Device.CreateDigitalInputPort(Device.Pins.D02);

            _input.Changed += (object sender, DigitalInputPortEventArgs e) => {
                Console.WriteLine($"Old school event raised; Time: {e.Time.ToShortTimeString()}, Value: {e.Value}");
            };

            _input.Subscribe(new FilterableObserver<DigitalInputPortEventArgs>(
                e => {
                    Console.WriteLine($"Time: {e.Time.ToShortTimeString()}, Value: {e.Value}");
                }));           
        }
    }
}
