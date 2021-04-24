namespace Meadow.Peripherals.Sensors.Distance
{
    // TODO: Remove this class after units conversion is done
    public class DistanceConditions
    {
        public float? Distance { get; set; }

        public DistanceConditions()
        {
        }

        public DistanceConditions(float? distance)
        {
            Distance = distance;
        }

        public static DistanceConditions From(DistanceConditions conditions)
        {
            return new DistanceConditions(conditions.Distance);
        }
    }

    public class DistanceConditionChangeResult : IChangeResult<DistanceConditions>
    {
        public DistanceConditions New
        {
            get => newValue; 
            set
            {
                newValue = value;
                RecalcDelta();
            }
        }
        protected DistanceConditions newValue = new DistanceConditions();

        public DistanceConditions Old
        {
            get { return oldValue; }
            set
            {
                oldValue = value;
                RecalcDelta();
            }
        }
        protected DistanceConditions oldValue = new DistanceConditions();

        public DistanceConditions Delta { get; protected set; } = new DistanceConditions();

        public DistanceConditionChangeResult(DistanceConditions newValue, DistanceConditions oldValue)
        {
            New = newValue;
            Old = oldValue;
        }

        protected void RecalcDelta()
        {
            DistanceConditions delta = new DistanceConditions();
            delta.Distance = New.Distance - Old.Distance;
 
            Delta = delta;
        }
    }
}