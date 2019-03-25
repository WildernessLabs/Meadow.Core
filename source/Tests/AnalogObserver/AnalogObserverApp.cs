using System;
using System.Diagnostics;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace AnalogObserver
{
    public class AnalogObserverApp : AppBase<F7Micro, AnalogObserverApp>
    {
        protected AnalogInputPort _analog01;

        protected IDisposable _absoluteObserver = null;

        public AnalogObserverApp()
        {
            this.InitializeIO();
            this.WireUpObservers();
        }

        public void InitializeIO()
        {
            // TODO: how to instantiate?
            //_analog01 = new AnalogInputPort();
        }

        public void WireUpObservers()
        {
            var firehoseSubscriber = _analog01.Subscribe(new FilteredObserver<FloatChangeResult>(
                handler: result =>
                {
                    Debug.WriteLine("Previous Value: " + result.Old);
                    Debug.WriteLine("New Value: " + result.New);
                }));

            //firehoseSubscriber.Dispose();

            // absolute: notify me when the temperature hits 75º
            float seventyFiveDegrees = (3.3f / 100f) * 75;
            _absoluteObserver = _analog01.Subscribe(new FilteredObserver<FloatChangeResult>(
                filter: result => (result.New > seventyFiveDegrees),
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
            var relative = _analog01.Subscribe(new FilteredObserver<FloatChangeResult>(
                filter: result => (result.Delta > oneDegreeC || result.Delta < oneDegreeC),
                handler: result =>
                {
                    Debug.WriteLine("Changed value: " + result.Delta);
                }));

            //relative.Dispose();

            // relative percentage change
            _analog01.Subscribe(new FilteredObserver<FloatChangeResult>(
                filter: result => (result.DeltaPercent > 10 || result.DeltaPercent < 10),
                handler: result =>
                {
                    Debug.WriteLine("Percentage changed: " + result.Delta);
                }));

            // spin up the ADC sampling engine
            // TODO
            _analog01.StartSampling();

        }
    }
}
