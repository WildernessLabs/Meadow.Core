using Meadow;
using Meadow.Bases;
using Meadow.Devices;
using Meadow.Units;
using System;

namespace CompositeTest
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        BME999 compositeSensor;

        public MeadowApp()
        {
            Initialize();
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            compositeSensor = new BME999();

            var observer = compositeSensor.GetObserver(
                h => { },
                e => { return true; }
            );

            compositeSensor.Subscribe(observer);

            /*
            var observer = new FilterableChangeObserver<CompositeChangeResult<Mass, Pressure, Temperature>,
                (Mass, Pressure, Temperature)>
            (
                h => { },
                e => { return true; }
            );*/

            //compositeSensor.Subscribe(observer);
        }
    }
}