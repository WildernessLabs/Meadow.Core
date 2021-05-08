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
        {
            this.Capabilities = new DeviceCapabilities(
                new AnalogCapabilities(true, DefaultA2DResolution),
                new NetworkCapabilities(true, true)
                );

            base.IoController = new F7GPIOManager();
            base.IoController.Initialize();

            this.Pins = new Pinout();

            // TODO: it would be nice to block on this initialization, but
            // because of the app architecture, this ctor runs asynchronously
            // with app startup, so right now we're raising an event.
            //this.InitEsp32CoProc();
            this.InitCoprocessor();
        }
    }
}