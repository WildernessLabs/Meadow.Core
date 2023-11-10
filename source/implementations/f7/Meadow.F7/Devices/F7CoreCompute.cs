using Meadow.Hardware;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents the Meadow F7CoreComputeV2 device
    /// </summary>
    public partial class F7CoreComputeV2 : F7CoreComputeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="F7CoreComputeV2"/> class
        /// </summary>
        public F7CoreComputeV2()
            : base(new Pinout(),
                  new F7CoreComputeGpioManager(),
                  new AnalogCapabilities(true, DefaultA2DResolution),
                  new NetworkCapabilities(true, false),
                  new StorageCapabilities(true))
        {
            if (this.Information.Platform != Hardware.MeadowPlatform.F7CoreComputeV2)
            {
                var message = $"Application is defined as {nameof(F7CoreComputeV2)}, but running hardware is {this.Information.Platform}";
                Resolver.Log.Error(message);
                throw new UnsupportedPlatformException(this.Information.Platform, message);
            }

            // because we can't use new Pinout(this) in the cast to the base ('this' doesn't exist at that point and the compiler denies usage)
            Pins.Controller = this;
        }

        /// <summary>
        /// Gets the I2C bus number for the specified clock and data pins
        /// </summary>
        /// <param name="clock">The clock pin</param>
        /// <param name="data">The data pin</param>
        /// <returns>The I2C bus number, or -1 if the bus is unsupported</returns>
        protected override int GetI2cBusNumberForPins(IPin clock, IPin data)
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