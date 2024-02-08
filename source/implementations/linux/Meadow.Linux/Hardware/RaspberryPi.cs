using Meadow.Hardware;
using Meadow.Pinouts;
using System;
using System.Linq;

namespace Meadow
{
    public class RaspberryPi : Linux
    {
        private readonly DeviceCapabilities _capabilities;

        public RaspberryPiPinout Pins { get; }

        /// <inheritdoc/>
        public override DeviceCapabilities Capabilities => _capabilities;

        public RaspberryPi()
        {
            Pins = new RaspberryPiPinout();

            Pins.Controller = this;

            _capabilities = new DeviceCapabilities(
                new AnalogCapabilities(false, null),
                new NetworkCapabilities(false, true),
                new StorageCapabilities(true)
                );
        }

        /// <inheritdoc/>
        public override IPin GetPin(string pinName)
        {
            return Pins.AllPins.First(p => string.Compare(p.Name, pinName) == 0);
        }

        /// <inheritdoc/>
        public override II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
        {
            if (clock == Pins["PIN05"] && data == Pins["PIN03"])
            {
                return new I2CBus(1, busSpeed);
            }

            throw new ArgumentOutOfRangeException("Requested pins are not I2C bus pins");
        }
    }
}
