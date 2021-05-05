using System;

namespace Meadow.Peripherals.Sensors.Hid
{
    public interface IJoystickSensor : ISensor, IObservable<CompositeChangeResult<JoystickPosition>>
    {
        /// <summary>
        /// Last horizontal value read from the Joystick.
        /// </summary>
        float HorizontalValue { get; }

        /// <summary>
        /// Last vertical value read from the Joystick.
        /// </summary>
        float VerticalValue { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        //event EventHandler<JoystickPositionChangeResult> Updated;
        event EventHandler<CompositeChangeResult<JoystickPosition>> Updated;
    }
}