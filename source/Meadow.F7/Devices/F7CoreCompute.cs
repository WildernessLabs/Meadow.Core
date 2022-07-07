using Meadow.Hardware;

namespace Meadow.Devices
{
    public partial class F7CoreComputeV1 : F7CoreComputeBase
    {
        public SerialPortNameDefinitions SerialPortNames => new SerialPortNameDefinitions();

        public F7CoreComputeV1()
            : base(new Pinout(),
                  new F7CoreComputeGpioManager(),
                  new AnalogCapabilities(true, DefaultA2DResolution),
                  new NetworkCapabilities(true, false))
        {
            if (this.Information.Platform != Hardware.MeadowPlatform.F7CoreComputeV1)
            {
                var message = $"Application is defined as F7CoreComputeV1, but running hardware is {this.Information.Platform}";
                Resolver.Log.Error(message);
                throw new UnsupportedPlatformException(this.Information.Platform, message);
            }
        }

        protected override int GetI2CBusNumberForPins(IPin clock, IPin data)
        {
            if (clock.Name == (Pins as F7CoreComputeV1.Pinout)?.I2C1_SCL.Name)
            {
                return 1;
            }

            if (clock.Name == (Pins as F7CoreComputeV1.Pinout)?.I2C3_SCL.Name)
            {
                return 3;
            }

            // this is an unsupported bus, but will get caught elsewhere
            return -1;
        }
    }
}