using System;

namespace Meadow.Peripherals.Sensors.Hid
{
    public class JoystickConditions
    {
        /// <summary>
        /// 
        /// </summary>
        public float HorizontalValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float VerticalValue { get; set; }

        public JoystickConditions() { }

        public JoystickConditions(float horizontalValue, float verticalValue) 
        {
            HorizontalValue = horizontalValue;
            VerticalValue = verticalValue;
        }

        public static JoystickConditions From(JoystickConditions conditions) 
        {
            return new JoystickConditions(
                conditions.HorizontalValue,
                conditions.VerticalValue
                );
        }
    }

    public class JoystickConditionChangeResult : IChangeResult<JoystickConditions>
    {
        public JoystickConditions New 
        { 
            get => _newValue; 
            set { _newValue = value;  } 
        } protected JoystickConditions _newValue = new JoystickConditions();
        
        public JoystickConditions Old 
        { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException();
        } protected JoystickConditions _oldValue = new JoystickConditions();

        public JoystickConditions Delta { get; protected set; } = new JoystickConditions();

        public JoystickConditionChangeResult(JoystickConditions newValue, JoystickConditions oldValue) 
        {
            New = newValue;
            Old = oldValue;
        }

        protected void RecalcDelta() 
        {
            JoystickConditions delta = new JoystickConditions();
            delta.HorizontalValue = New.HorizontalValue - Old.HorizontalValue;
            delta.VerticalValue = New.VerticalValue - Old.VerticalValue;
            Delta = delta;
        }
    }
}