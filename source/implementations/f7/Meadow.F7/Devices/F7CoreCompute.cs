using Meadow.Hardware;

namespace Meadow.Devices
{
    public partial class F7CoreComputeV2 : F7CoreComputeBase
    {
        public F7CoreComputeV2()
            : base(new Pinout(),
                  new F7CoreComputeGpioManager(),
                  new AnalogCapabilities(true, DefaultA2DResolution),
                  new NetworkCapabilities(true, false),
                  new StorageCapabilities(true))
        {
            if (this.Information.Platform != Hardware.MeadowPlatform.F7CoreComputeV2)
            {
                var message = $"Application is defined as F7CoreComputeV2, but running hardware is {this.Information.Platform}";
                Resolver.Log.Error(message);
                throw new UnsupportedPlatformException(this.Information.Platform, message);
            }

            // because we cant use new Pinout(this) in the cass to the base ('this' doesn't exist at that point and the compiler denies usage)
            Pins.Controller = this;
        }

        protected override int GetI2CBusNumberForPins(IPin clock, IPin data)
        {
            if (clock.Name == (Pins as F7CoreComputeV2.Pinout)?.I2C1_SCL.Name)
            {
                return 1;
            }

            if (clock.Name == (Pins as F7CoreComputeV2.Pinout)?.I2C3_SCL.Name)
            {
                return 3;
            }

            // this is an unsupported bus, but will get caught elsewhere
            return -1;
        }
    }
}