using System;

namespace Meadow.Peripherals.Sensors.Hid
{
    public interface IJoystickSensor : ISensor, IObservable<ChangeResult<JoystickPosition>>
    {
        /// <summary>
        /// Gets the position of the joystick.
        /// </summary>
        JoystickPosition? Position { get; }

        /// <summary>
        /// Raised when a new reading has been made. Events will only be raised
        /// while the driver is updating. To start, call the `StartUpdating()`
        /// method.
        /// </summary>
        event EventHandler<ChangeResult<JoystickPosition>> Updated;
    }
}