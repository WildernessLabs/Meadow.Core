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

        public AnalogObserverApp()
        {
            this.InitializeIO();
            this.WireUpObservers();
        }

        public void InitializeIO()
        {
            _analog01 = new AnalogInputPort(Device.Pins.A01);
        }

        public void WireUpObservers()
        {
            var disposer = _analog01.Subscribe(
                filter: result => { return true; },
                handler: result => {
                    Debug.WriteLine("Previous Value: " + result.Old);
                    Debug.WriteLine("New Value: " + result.New);
                });

            disposer.Dispose();

            // simple average
            var simple = _analog01.Subscribe(
                filter: result => { return true; }, //( true ),
                handler: result => {
                Debug.WriteLine("Previous Value: " + result.Old);
                    Debug.WriteLine("New Value: " + result.New);
                });

            simple.Dispose();

            // absolute: notify me when the temperature hits 75º
            var absolute = _analog01.Subscribe(
                filter: result => (result.New > 75),
                handler: avgValue => {
                    Debug.WriteLine("New value: " + avgValue.New);
                    // TODO: unsubscribe - how? @BRIANK
                });
            absolute.Dispose();

            // relative, static comparison; e.g if change is > 1º
            float oneDegreeC = 3.3f / 100f; // TMP35DZ: 0º = 0V, 100º = 3.3V
            var relative = _analog01.Subscribe(
                filter: result => (result.Delta > oneDegreeC || result.Delta < oneDegreeC),
                handler: result => {
                    Debug.WriteLine("Changed value: " + result.Delta);
                });

            relative.Dispose();

            // relative percentage change
            _analog01.Subscribe(
                filter: result => (result.DeltaPercent > 10 || result.DeltaPercent < 10),
                handler: result => {
                    Debug.WriteLine("Percentage changed: " + result.Delta);
                });

            _analog01.StartSampling();

        }
    }
}
