using Meadow.Gateways;
using Meadow.Hardware;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Meadow F7 micro device. Includes device-specific IO mapping,
    /// capabilities and provides access to the various device-specific features.
    /// </summary>
    public partial class F7Micro : F7MicroBase
    {
        public SerialPortNameDefinitions SerialPortNames => new SerialPortNameDefinitions();

        public F7Micro()
            : base(
                  new Pinout(),
                  new F7GPIOManager(),
                  new AnalogCapabilities(true, DefaultA2DResolution),
                  new NetworkCapabilities(true, true))
        {
            // TODO: it would be nice to block on this initialization, but
            // because of the app architecture, this ctor runs asynchronously
            // with app startup, so right now we're raising an event.
            //this.InitEsp32CoProc();

            this.esp32 = new Esp32Coprocessor();
            BluetoothAdapter = esp32;
            WiFiAdapter = esp32;
            Coprocessor = esp32;
        }
    }
}
