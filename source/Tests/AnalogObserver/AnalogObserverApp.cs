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
            // TODO: @BRIANK: Can we automatically create this somehow?
            // then the API would be _analog01.Subscribe(x, y, z);
            var observer = new MeadowObserver<FloatChangeResult>();

            // simple average
            observer.Subscribe(_analog01,
                filter: result => { return true; }, //( true ),
                handler: result => {
                Debug.WriteLine("Previous Value: " + result.Old);
                    Debug.WriteLine("New Value: " + result.New);
                });

            // absolute: notify me when the temperature hits 75º
            observer.Subscribe(_analog01,
                filter: result => (result.New > 75),
                handler: avgValue => {
                    Debug.WriteLine("New value: " + avgValue.New);
                    // TODO: unsubscribe - how? @BRIANK
                });

            // relative, static comparison; e.g if change is > 1º
            float oneDegreeC = 3.3f / 100f; // TMP35DZ: 0º = 0V, 100º = 3.3V
            observer.Subscribe(_analog01,
                filter: result => (result.Delta > oneDegreeC || result.Delta < oneDegreeC),
                handler: result => {
                    Debug.WriteLine("Changed value: " + result.Delta);
                });

            // relative percentage change
            observer.Subscribe(_analog01,
                filter: result => (result.DeltaPercent > 10 || result.DeltaPercent < 10),
                handler: result => {
                    Debug.WriteLine("Percentage changed: " + result.Delta);
                });

            _analog01.StartSampling();

            // TODO: @BRIANK - here? why? leftover?
            observer.Unsubscribe();

        }
    }
}
