using Meadow.Hardware;
using System;

namespace Meadow.Devices
{
    [Obsolete("Use the F7FeatherV2 class instead.")]
    public class F7Micro2 : F7FeatherV2
    {
    }

    /// <summary>
    /// Represents a Meadow F7 micro v2 device. Includes device-specific IO mapping,
    /// capabilities and provides access to the various device-specific features.
    /// </summary>
    public partial class F7FeatherV2 : F7FeatherBase
    {
        public SerialPortNameDefinitions SerialPortNames => new SerialPortNameDefinitions();

        public F7FeatherV2()
            : base(new Pinout(),
                  new F7FeatherGpioManager(),
                  new AnalogCapabilities(true, DefaultA2DResolution),
                  new NetworkCapabilities(true, false))
        {
            if (this.Information.Platform != Hardware.MeadowPlatform.F7FeatherV2)
            {
                var message = $"Application is defined as F7FeatherV2, but running hardware is {this.Information.Platform}";
                Resolver.Log.Error(message);
                throw new UnsupportedPlatformException(this.Information.Platform, message);
            }
        }

        protected override int GetI2CBusNumberForPins(IPin clock, IPin data)
        {
            if (clock.Name == (Pins as F7FeatherV2.Pinout)?.I2C_SCL.Name)
            {
                return 1;
            }

            // this is an unsupported bus, but will get caught elsewhere
            return -1;
        }
    }
}