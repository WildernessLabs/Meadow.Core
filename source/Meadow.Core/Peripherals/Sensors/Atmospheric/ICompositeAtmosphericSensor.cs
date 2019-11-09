using System;
using System.Threading.Tasks;
using Meadow.Peripherals.Sensors;

namespace Meadow.Peripherals.Sensors.Atmospheric
{
    public interface ICompositeAtmosphericSensor : ISensor, IObservable<AtmosphericConditionChangeResult>
    {
        /// <summary>
        /// Last value read from the Temperature sensor.
        /// </summary>
        AtmosphericConditions Conditions { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<AtmosphericConditionChangeResult> Updated;
    }
}
