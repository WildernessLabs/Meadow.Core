using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Meadow.Peripherals.Sensors
{
    /// <summary>
    /// Humidity sensor interface requirements.
    /// </summary>
    public interface IHumiditySensor : ISensor
    {
        /// <summary>
        /// Last value read from the humidity sensor.
        /// </summary>
        RelativeHumidity? Humidity { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<IChangeResult<RelativeHumidity>> HumidityUpdated;
    }
}