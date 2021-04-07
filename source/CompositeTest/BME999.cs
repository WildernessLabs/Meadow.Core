using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meadow.Bases;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Temperature;
using Meadow.Units;

namespace CompositeTest
{
    //public class Bmp180 : FilterableChangeObservableBase<AtmosphericConditionChangeResult, AtmosphericConditions>,
    //    IAtmosphericSensor, IBarometricPressureSensor, ITemperatureSensor

    public class BME999 :
        FilterableChangeObservableBase<CompositeChangeResult<Mass, Pressure, Temperature>, Mass, Pressure, Temperature>
        , IBarometricPressureSensor, ITemperatureSensor
    {
        public float Pressure => 100;

        public float Temperature => 50;

        public float Mass => 0;

        public event EventHandler<AtmosphericConditionChangeResult> Updated;

        public IDisposable Subscribe(IObserver<AtmosphericConditionChangeResult> observer)
        {
            throw new NotImplementedException();
        }
    }
}
