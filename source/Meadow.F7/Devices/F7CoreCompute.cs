using System;
using static Meadow.Core.Interop;

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
                Console.WriteLine(message);
                throw new UnsupportedPlatformException(this.Information.Platform, message);
            }
        }
    }
}