using Meadow.Gateways;
using Meadow.Hardware;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Meadow F7 micro v2 device. Includes device-specific IO mapping,
    /// capabilities and provides access to the various device-specific features.
    /// </summary>
    public partial class F7MicroV2 : F7MicroBase
    {
        public SerialPortNameDefinitions SerialPortNames => new SerialPortNameDefinitions();

        public F7MicroV2()
            : base(new Pinout(),
                  new F7GPIOManager(),
                  new AnalogCapabilities(true, DefaultA2DResolution),
                  new NetworkCapabilities(true, true))
        {
            this.InitCoprocessor();
        }
    }
}