using Meadow.Hardware;
using Meadow.Pinouts;
using System;
using System.IO;
using System.Linq;

namespace Meadow
{
    public class RaspberryPi : Linux
    {
        private readonly DeviceCapabilities _capabilities;
        private bool? _isPi5;

        public RaspberryPiPinout Pins { get; }

        /// <inheritdoc/>
        public override DeviceCapabilities Capabilities => _capabilities;

        public bool IsRaspberryPi5 => _isPi5 ??= CheckIfPi5();

        public RaspberryPi()
        {
            // are we a Pi5?  if so, the GPIOs have moved
            if (IsRaspberryPi5)
            {
                Console.WriteLine("We're on a Raspberry Pi 5");
            }

            Pins = new RaspberryPiPinout(
                IsRaspberryPi5 ?
                    RaspberryPiPinout.GpiodChipPi5 :
                    RaspberryPiPinout.GpiodChipPi4,
                IsRaspberryPi5 ? 53 : 0
                );

            Pins.Controller = this;

            _capabilities = new DeviceCapabilities(
                new AnalogCapabilities(false, null),
                new NetworkCapabilities(false, true),
                new StorageCapabilities(true)
                );
        }

        private bool CheckIfPi5()
        {
            // TODO: is there a better way to find this?
            try
            {
                var model = File.ReadAllText("/proc/device-tree/model");
                return model.Contains("Raspberry Pi 5");
            }
            catch
            {
                return false;
            }
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
