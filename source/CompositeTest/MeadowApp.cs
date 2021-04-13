using Meadow;
using Meadow.Bases;
using Meadow.Devices;
using Meadow.Units;
using System;

namespace CompositeTest
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        //  BME999 compositeSensor;

        Bmp180 bmp180;

        public MeadowApp()
        {
            Initialize();
        }

void Initialize()
{
    Console.WriteLine("Initialize hardware...");

    bmp180 = new Bmp180(Device.CreateI2cBus());

    Console.WriteLine("Event subscribe...");

    //  bmp180.Updated += Bmp180_Updated;

    Console.WriteLine("Start updating...");

    var observer = new FilterableChangeObserver<CompositeChangeResult<Pressure, Temperature>,
        (Pressure Pressure, Temperature Temperature)?>
    (
        h => { Console.WriteLine($"Update: {h.New.Value.unit1.StandardAtmosphere}, {h.New.Value.unit2.Fahrenheit}"); },
        e => { return true; }
    );
            (

    bmp180.Subscribe(observer);

    bmp180.StartUpdating();



            /*
            compositeSensor = new BME999();

            var observer = compositeSensor.GetObserver(
                h => { },
                e => { return true; }
            );

            compositeSensor.Subscribe(observer); */

            /*
            var observer = new FilterableChangeObserver<CompositeChangeResult<Mass, Pressure, Temperature>,
                (Mass, Pressure, Temperature)>
            (
                h => { },
                e => { return true; }
            );*/

            //compositeSensor.Subscribe(observer);
        }

        private void Bmp180_Updated(object sender, CompositeChangeResult<Pressure, Temperature> e)
        {
            Console.WriteLine($"Update: {e.New.Value.unit1.Psi}, {e.New.Value.unit2.Celsius}");
        }
    }
}