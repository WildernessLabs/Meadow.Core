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
        public F7SerialPortNameDefinitions SerialPortNames => new F7SerialPortNameDefinitions();

        public F7FeatherV1()
            : base(
                  new Pinout(),
                  new F7FeatherGpioManager(),
                  new AnalogCapabilities(true, DefaultA2DResolution),
                  new NetworkCapabilities(true, false))
        {
        }
    }
}
