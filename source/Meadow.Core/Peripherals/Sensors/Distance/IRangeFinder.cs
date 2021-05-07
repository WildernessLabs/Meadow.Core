using Meadow.Units;
using System;

namespace Meadow.Peripherals.Sensors
{
    /// <summary>
    /// Interface for distance sensors classes.
    /// </summary>
    public interface IRangeFinder : ISensor
    {
        /// <summary>
        /// Last value read from the Temperature sensor.
        /// </summary>
        Length Distance { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<IChangeResult<Length>> DistanceUpdated;
    }
}