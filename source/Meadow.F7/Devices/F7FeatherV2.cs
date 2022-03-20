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
        }
    }
}