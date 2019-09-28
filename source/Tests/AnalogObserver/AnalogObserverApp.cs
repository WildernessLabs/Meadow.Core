using System;
using System.Diagnostics;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace AnalogObserver
{
    public class AnalogObserverApp : App<F7Micro, AnalogObserverApp>
    {
        IAnalogInputPort _analogIn;

        protected IDisposable _absoluteObserver = null;

        public AnalogObserverApp()
        {
            Console.WriteLine("Starting App");
            this.InitializeIO();
            this.WireUpObservers();
        }

        public void InitializeIO()
        {
            _analogIn = Device.CreateAnalogInputPort(Device.Pins.A00);
            Console.WriteLine("Analog port created");
        }

        public void WireUpObservers()
        {
            var firehoseSubscriber = _analogIn.Subscribe(new FilterableObserver<FloatChangeResult>(
                handler: result =>
                {
                    Debug.WriteLine("Previous Value: " + result.Old);
                    Debug.WriteLine("New Value: " + result.New);
                }));

            //firehoseSubscriber.Dispose();

            // absolute: notify me when the temperature hits 75º
            float seventyFiveDegreesC = (75f - 32f) * (5f / 9f); // convert to C
            float seventyFiveDegreesVoltage = (seventyFiveDegreesC / 100f) * 3.3f;
            _absoluteObserver = _analogIn.Subscribe(new FilterableObserver<FloatChangeResult>(
                filter: result => (result.New > seventyFiveDegreesVoltage),
                handler: avgValue =>
                {
                    Debug.WriteLine("We've hit 75º!");
                    // unsubscribe when we hit it
                    if (_absoluteObserver != null)
                    {
                        _absoluteObserver.Dispose();
                    }
                }));

            // relative, static comparison; e.g if change is > 1º
            float oneDegreeC = 3.3f / 100f; // TMP35DZ: 0º = 0V, 100º = 3.3V
            var relative = _analogIn.Subscribe(new FilterableObserver<FloatChangeResult>(
                filter: result => (result.Delta > oneDegreeC || result.Delta < oneDegreeC),
                handler: result =>
                {
                    Debug.WriteLine("Changed value: " + result.Delta);
                }));

            //relative.Dispose();

            // relative percentage change
            _analogIn.Subscribe(new FilterableObserver<FloatChangeResult>(
                filter: result => (result.DeltaPercent > 10 || result.DeltaPercent < 10),
                handler: result =>
                {
                    Debug.WriteLine("Percentage changed: " + result.Delta);
                }));

            // spin up the ADC sampling engine
            // TODO
            _analogIn.StartSampling();

        }
    }
}
