using System;

namespace Meadow.Devices
{
    [Obsolete("Use the F7FeatherV1 class instead.")]
    public class F7Micro : F7FeatherV1
    {
    }

    /// <summary>
    /// Represents a Meadow F7 micro device. Includes device-specific IO mapping,
    /// capabilities and provides access to the various device-specific features.
    /// </summary>
    public partial class F7FeatherV1 : F7FeatherBase
    {
        public F7FeatherV1()
            : base(
                  new Pinout(),
                  new F7FeatherGpioManager(),
                  new AnalogCapabilities(true, DefaultA2DResolution),
                  new NetworkCapabilities(true, false))
        {
            if (this.Information.Platform != Hardware.MeadowPlatform.F7FeatherV1)
            {
                var message = $"Application is defined as F7FeatherV1, but running hardware is {this.Information.Platform}";
                Console.WriteLine(message);
                throw new UnsupportedPlatformException(this.Information.Platform, message);
            }
        }
    }
}
