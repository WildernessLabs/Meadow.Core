using Meadow;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Motion;
using Meadow.Pinouts;
using System;
using System.Threading;

namespace XavierI2C_Sample
{
    class MeadowApp : App<MeadowForLinux<JetsonXavierAGX>>
    {
        private Bno055 _bno055;
        private Ccs811 _ccs811;
        private Si70xx _si7021;
        private Adxl345 _adxl345;

        public MeadowApp()
        {
            InitializeHardware();
        }

        private void InitializeHardware()
        {
            // Note the Xavier uses bus 8 for pins 3 & 5
            var bus = Device.CreateI2cBus(8);

            _bno055 = new Bno055(bus);
            _bno055.EulerOrientationUpdated += OnEulerOrientationUpdated;
            _bno055.StartUpdating(TimeSpan.FromSeconds(1));

            _ccs811 = new Ccs811(bus);
            _ccs811.Updated += AirConditionsUpdated;
            _ccs811.StartUpdating(TimeSpan.FromSeconds(5));

            _si7021 = new Si70xx(bus);
            _si7021.Updated += TempHumidityUpdated;
            _si7021.StartUpdating(TimeSpan.FromSeconds(5));
        }

        private void TempHumidityUpdated(object sender, IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity)> e)
        {
            Console.WriteLine($"Temp:{e.New.Temperature.Value.Fahrenheit}F Humidity:{e.New.Humidity.Value.Percent}%");
        }

        private void AirConditionsUpdated(object sender, IChangeResult<(Meadow.Units.Concentration? Co2, Meadow.Units.Concentration? Voc)> e)
        {
            Console.WriteLine($"CO2:{e.New.Co2.Value.PartsPerMillion}PPM VOC:{e.New.Voc.Value.PartsPerMillion}PPM");
        }

        private void OnEulerOrientationUpdated(object sender, IChangeResult<Meadow.Foundation.Spatial.EulerAngles> e)
        {
            Console.WriteLine($"H:{e.New.Heading} P:{e.New.Pitch} R:{e.New.Roll}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new MeadowApp();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
