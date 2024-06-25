using Meadow.Hardware;
using Meadow.Pinouts;
using System;
using System.Linq;

namespace Meadow
{
    public class JetsonNano : Linux
    {
        private readonly DeviceCapabilities _capabilities;
        private IPlatformOS _platformOS;

        public JetsonNanoPinout Pins { get; }

        public override DeviceCapabilities Capabilities => _capabilities;
        public override IPlatformOS PlatformOS => _platformOS;

        /// <summary>
        /// Creates the Meadow on Linux infrastructure instance
        /// </summary>
        public JetsonNano()
        {
            _platformOS = new JetsonPlatformOS();

            Pins = new JetsonNanoPinout();

            Pins.Controller = this;

            _capabilities = new DeviceCapabilities(
                new AnalogCapabilities(false, null),
                new NetworkCapabilities(false, true),
                new StorageCapabilities(true)
                );
        }

        public override IPin GetPin(string pinName)
        {
            return Pins.AllPins.First(p => string.Compare(p.Name, pinName) == 0);
        }

        public override II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
        {
            if (clock == Pins["PIN05"] && data == Pins["PIN03"])
            {
                return new I2CBus(1, busSpeed);
            }
            else if (clock == Pins["PIN28"] && data == Pins["PIN27"])
            {
                return new I2CBus(0, busSpeed);
            }

            throw new ArgumentOutOfRangeException("Requested pins are not I2C bus pins");
        }
    }
}
