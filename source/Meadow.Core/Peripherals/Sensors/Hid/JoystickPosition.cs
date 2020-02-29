using System;

namespace Meadow.Peripherals.Sensors.Hid
{
    public class JoystickPosition
    {
        /// <summary>
        /// 
        /// </summary>
        public float HorizontalValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float VerticalValue { get; set; }

        public JoystickPosition() { }

        public JoystickPosition(float horizontalValue, float verticalValue) 
        {
            HorizontalValue = horizontalValue;
            VerticalValue = verticalValue;
        }

        public static JoystickPosition From(JoystickPosition conditions) 
        {
            return new JoystickPosition(
                conditions.HorizontalValue,
                conditions.VerticalValue
                );
        }
    }

    public class JoystickPositionChangeResult : IChangeResult<JoystickPosition>
    {
        public JoystickPosition New 
        { 
            get => _newValue; 
            set { _newValue = value;  } 
        } protected JoystickPosition _newValue = new JoystickPosition();
        
        public JoystickPosition Old 
        { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException();
        } protected JoystickPosition _oldValue = new JoystickPosition();

        public JoystickPosition Delta { get; protected set; } = new JoystickPosition();

        public JoystickPositionChangeResult(JoystickPosition newValue, JoystickPosition oldValue) 
        {
            New = newValue;
            Old = oldValue;
        }

        protected void RecalcDelta() 
        {
            JoystickPosition delta = new JoystickPosition();
            delta.HorizontalValue = New.HorizontalValue - Old.HorizontalValue;
            delta.VerticalValue = New.VerticalValue - Old.VerticalValue;
            Delta = delta;
        }
    }
}