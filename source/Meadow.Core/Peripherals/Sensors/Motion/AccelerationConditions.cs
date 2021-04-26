﻿using System;
namespace Meadow.Peripherals.Sensors.Motion
{
    [Obsolete]
    public class AccelerationConditions
    {
        public float? XAcceleration { get; set; }
        public float? YAcceleration { get; set; }
        public float? ZAcceleration { get; set; }

        public float? XGyroscopicAcceleration { get; set; }
        public float? YGyroscopicAcceleration { get; set; }
        public float? ZGyroscopicAcceleration { get; set; }

        public AccelerationConditions()
        {
        }

        public AccelerationConditions(float? xAcceleration, float? yAcceleration, float? zAcceleration,
            float? xGyroAcceleration, float? yGyroAcceleration, float? zGyroAcceleration)
        {
            this.XAcceleration = xAcceleration;
            this.YAcceleration = yAcceleration;
            this.ZAcceleration = zAcceleration;

            this.XGyroscopicAcceleration = xGyroAcceleration;
            this.YGyroscopicAcceleration = yGyroAcceleration;
            this.ZGyroscopicAcceleration = zGyroAcceleration;
        }

        public static AccelerationConditions From(AccelerationConditions conditions)
        {
            return new AccelerationConditions(
                conditions.XAcceleration,
                conditions.YAcceleration,
                conditions.ZAcceleration,
                conditions.XGyroscopicAcceleration,
                conditions.YGyroscopicAcceleration,
                conditions.ZGyroscopicAcceleration
                );
        }
    }

    public class AccelerationConditionChangeResult : IChangeResult<AccelerationConditions>
    {
        public AccelerationConditions New
        {
            get => newValue;
            set
            {
                newValue = value;
                RecalcDelta();
            }
        }
        protected AccelerationConditions newValue = new AccelerationConditions();

        public AccelerationConditions Old
        {
            get => oldValue; 
            set
            {
                oldValue = value;
                RecalcDelta();
            }
        }
        protected AccelerationConditions oldValue = new AccelerationConditions();

        public AccelerationConditions Delta { get; protected set; } = new AccelerationConditions();

        public AccelerationConditionChangeResult(
            AccelerationConditions newValue, AccelerationConditions oldValue)
        {
            New = newValue;
            Old = oldValue;
        }

        protected void RecalcDelta()
        {
            var delta = new AccelerationConditions();

            delta.XAcceleration = New.XAcceleration - Old.XAcceleration;
            delta.YAcceleration = New.YAcceleration - Old.YAcceleration;
            delta.ZAcceleration = New.ZAcceleration - Old.ZAcceleration;

            delta.XGyroscopicAcceleration = New.XGyroscopicAcceleration - Old.XGyroscopicAcceleration;
            delta.YGyroscopicAcceleration = New.YGyroscopicAcceleration - Old.YGyroscopicAcceleration;
            delta.ZGyroscopicAcceleration = New.ZGyroscopicAcceleration - Old.ZGyroscopicAcceleration;

            Delta = delta;
        }
    }
}