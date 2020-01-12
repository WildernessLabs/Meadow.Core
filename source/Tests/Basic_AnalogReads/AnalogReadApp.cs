using System;
using Meadow;
using Meadow.Hardware;
using Meadow.Devices;
using System.Threading;
using System.Threading.Tasks;

namespace Basic_AnalogReads
{
    public class AnalogReadApp : App<F7Micro, AnalogReadApp>
    {
        IAnalogInputPort analogIn;

        public AnalogReadApp()
        {
            Console.WriteLine("Starting App");
            analogIn = Device.CreateAnalogInputPort(Device.Pins.A00);
            Console.WriteLine("Analog port created");
            this.StartReading();
        }

        protected async void StartReading()
        {
            float voltage;

            for (int i = 0; i < 10; i++) {
                Console.WriteLine(i);
                voltage = await analogIn.Read();
                Console.WriteLine("Voltage: " + voltage.ToString());
                Thread.Sleep(500);
            }
        }
    }
}
