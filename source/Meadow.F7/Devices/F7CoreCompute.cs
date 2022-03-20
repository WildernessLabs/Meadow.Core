using System;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    public partial class F7CoreCompute : F7CoreComputeBase
    {
        public SerialPortNameDefinitions SerialPortNames => new SerialPortNameDefinitions();

        public F7CoreCompute()
            : base(new Pinout(),
                  new F7CoreComputeGpioManager(),
                  new AnalogCapabilities(true, DefaultA2DResolution),
                  new NetworkCapabilities(true, false))
        {
            // TODO: verify the platform hardware
        }
    }
}