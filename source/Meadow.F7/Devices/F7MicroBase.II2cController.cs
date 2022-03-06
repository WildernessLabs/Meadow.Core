using System;
using Meadow.Hardware;
using Meadow.Units;

namespace Meadow.Devices
{
    public abstract partial class F7MicroBase
    {
        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(int busNumber = 0)
        {
            return CreateI2cBus(busNumber, IMeadowDevice.DefaultI2cBusSpeed);
        }

        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            I2cBusSpeed busSpeed,
            int busNumber = 0
        )
        {
            return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, new Frequency((int)busSpeed, Frequency.UnitType.Hertz));
        }

        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <param name="frequency">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            int busNumber,
            Frequency frequency
        )
        {
            return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, frequency);
        }

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequency">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            IPin[] pins,
            Frequency frequency
        )
        {
            return CreateI2cBus(pins[0], pins[1], frequency);
        }

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequency">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            IPin clock,
            IPin data,
            Frequency frequency
        )
        {
            var bus = I2cBus.From(this.IoController, clock, data, frequency);
            bus.BusNumber = GetI2CBusNumberForPins(clock, data);
            Console.WriteLine($"I2C Bus number:{bus.BusNumber}");
            return bus;
        }

        protected int GetI2CBusNumberForPins(IPin clock, IPin data)
        {
            if (RuntimeInformation.IsPlatform(MeadowPlatform.F7CoreCompute))
            {
                if (clock.Name == (Pins as F7CoreCompute.Pinout)?.I2C3_SCL.Name)
                {
                    return 3;
                }
                if (clock.Name == (Pins as F7CoreCompute.Pinout)?.I2C1_SCL.Name)
                {
                    return 1;
                }
            }
            else if (RuntimeInformation.IsPlatform(MeadowPlatform.F7v1))
            {
                if (clock.Name == (Pins as F7Micro.Pinout)?.I2C_SCL.Name)
                {
                    return 1;
                }
            }
            else if (RuntimeInformation.IsPlatform(MeadowPlatform.F7v2))
            {
                if (clock.Name == (Pins as F7MicroV2.Pinout)?.I2C_SCL.Name)
                {
                    return 1;
                }
            }

            // this is an unsupported bus, but will get caught elsewhere
            return -1;
        }
    }
}
