using System;
using Meadow;
using Meadow.Hardware;
using Meadow.Devices;
using System.Threading;

namespace Basic_AnalogReads
{
    public class AnalogReadApp : AppBase<F7Micro, AnalogReadApp>
    {
        IAnalogInputPort analogIn;

        public AnalogReadApp()
        {
            Console.WriteLine("Starting App");
            analogIn = Device.CreateAnalogInputPort(Device.Pins.A00);
            this.StartReading();
        }

        protected void StartReading()
        {
            float voltage;
            while (true) {
                voltage = analogIn.Read();
                Console.WriteLine("Voltage: " + voltage.ToString());
                Thread.Sleep(500);
            }
        }
    }
}
