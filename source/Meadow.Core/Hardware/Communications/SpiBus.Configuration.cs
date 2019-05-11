using System;
namespace Meadow.Hardware
{
	public partial class SpiBus
	{
        // TODO: @Tacke & @Mark, this class needs to be reviewed
        // we may not need all of this stuff, i'm not sure. 
        public class ConfigurationOptions
        {
            public readonly IDigitalOutputPort Busy;
            public readonly bool BusyPinActiveState;
            public readonly bool ChipSelectActiveState;
            public readonly uint ChipSelectHoldTime;
            public readonly IDigitalOutputPort ChipSelect;
            public readonly uint ChipSelectSetupTime;
            public readonly bool ClockEdge;
            public readonly bool ClockIdleState;
            public readonly uint ClockRateKHz;

            public ConfigurationOptions(
                IDigitalOutputPort chipSelect,
                bool chipSelectActiveState,
                uint chipSelectSetupTime,
                uint chipSelectHoldTime,
                bool clockIdleState,
                bool clockEdge,
                uint clockRateKHz)
            {
                this.ChipSelect = chipSelect;
                this.ChipSelectActiveState = chipSelectActiveState;
                this.ChipSelectSetupTime = chipSelectSetupTime;
                this.ChipSelectHoldTime = chipSelectHoldTime;
                this.ClockIdleState = clockIdleState;
                this.ClockEdge = clockEdge;
                this.ClockRateKHz = clockRateKHz;
            }

            public ConfigurationOptions(
                IDigitalOutputPort chipSelect,
                bool chipSelectActiveState,
                uint chipSelectSetupTime,
                uint chipSelectHoldTime,
                bool clockIdleState,
                bool clockEdge,
                uint clockRateKHz,
                IDigitalOutputPort busy,
                bool busyPinActiveState)
                 : this(chipSelect, chipSelectActiveState, chipSelectSetupTime, chipSelectHoldTime,
                       clockIdleState, clockEdge, clockRateKHz)
            {
                this.Busy = busy;
                this.BusyPinActiveState = busyPinActiveState;
            }
        }
    }
}
