using Meadow.Contracts;

namespace Meadow.Peripherals.Sensors.Atmospheric
{
    public class SensorConditions<T1>
    {
        public T1 Value1 { get; set; }

        public SensorConditions()
        {

        }

        public SensorConditions(
            T1 value1)
        {
            Value1 = value1;
        }
    }

    public class SensorConditions<T1, T2>
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }

          public SensorConditions(
            T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }

    public class SensorConditions<T1, T2, T3>
    {
        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
        public T3 Value3 { get; set; }

        public SensorConditions(
          T1 value1, T2 value2, T3 value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }
    }

    //public class AtmosphericConditionChangeResult : IChangeResult<AtmosphericConditions>
    public class SensorConditionsChangeResult<T1> : ISensorChangeResult<SensorConditions<T1>>
    {

        public SensorConditions<T1> New
        {
            get => newValue;
            set
            {
                newValue = value;
                RecalcDelta();
            }
        }
        protected SensorConditions<T1> newValue = new SensorConditions<T1>();

        public SensorConditions<T1> Old
        {
            get => oldValue;
            set
            {
                oldValue = value;
                RecalcDelta();
            }
        }
        protected SensorConditions<T1> oldValue;

        public SensorConditions<T1> Delta { get; protected set; } = new SensorConditions<T1>();
        public SensorConditions<T1> Result1 { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public SensorConditionsChangeResult(
            SensorConditions<T1> newValue, SensorConditions<T1> oldValue)
        {
            New = newValue;
            Old = oldValue;
        }

        protected void RecalcDelta()
        {
        /*    SensorConditions<T1> delta = new SensorConditions<T1>();
            delta.Temperature = New.Temperature - Old.Temperature;
            delta.Pressure = New.Pressure - Old.Pressure;
            delta.Humidity = New.Humidity - Old.Humidity;
            Delta = delta;*/
        }
    }
}