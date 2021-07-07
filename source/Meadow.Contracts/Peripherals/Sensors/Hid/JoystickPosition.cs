using System;

namespace Meadow.Peripherals.Sensors.Hid
{
    /// <summary>
    /// Represents a position that an analog joystick can havin, including both
    /// the `Horizontal` and `Vertical` components.
    /// </summary>
    public struct JoystickPosition
    {
        /// <summary>
        /// 
        /// </summary>
        public float? Horizontal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float? Vertical { get; set; }

        public JoystickPosition(float? horizontal, float? vertical) 
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        public static JoystickPosition From(JoystickPosition position) 
        {
            return new JoystickPosition(position.Horizontal, position.Vertical );
        }
    }
}