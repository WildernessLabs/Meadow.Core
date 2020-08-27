using System;

namespace Meadow.Peripherals.Sensors.Distance
{
    /// <summary>
    /// Interface for distance sensors classes.
    /// </summary>
    public interface IRangeFinder : ISensor, IObservable<DistanceConditionChangeResult>
    {
        /// <summary>
        /// Last value read from the Temperature sensor.
        /// </summary>
        DistanceConditions Conditions { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<DistanceConditionChangeResult> Updated;
    }
}
