using System;
namespace Meadow.Hardware
{
	public partial class SpiBus
	{
        public enum ClockPolarity
        {
            CPOL_0 = 0,
            CPOL_1 = 1
        }

        public enum ClockPhase
        {
            CPHA_0 = 0,
            CPHA_1 = 1
        }

        public enum Mode
        {
            Mode0,
            Mode1,
            Mode2,
            Mode3
        }

        public class ClockConfiguration
        {
            public ClockPolarity Polarity { get; internal set; }
            public ClockPhase Phase { get; internal set; }
            /// <summary>
            /// The desired bus clock speed.
            /// </summary>
            /// <remarks>
            /// The set of supported speeds is programmatically available from the bus in the <b>SupportedSpeeds</b> property.
            /// </remarks>
            public long SpeedKHz { get; internal set; }

            internal ClockConfiguration()
            {
            }

            public ClockConfiguration(
                long speedKHz,
                ClockPolarity polarity = ClockPolarity.CPOL_0,
                ClockPhase phase = ClockPhase.CPHA_0
            )
            {
                this.SpeedKHz = speedKHz;
                this.Polarity = polarity;
                this.Phase = phase;
            }

            public ClockConfiguration(
                long speedKHz,
                Mode mode
            )
            {
                this.SpeedKHz = speedKHz;
                switch (mode)
                {
                    case Mode.Mode0:
                        this.Polarity = ClockPolarity.CPOL_0;
                        this.Phase = ClockPhase.CPHA_0;
                        break;
                    case Mode.Mode1:
                        this.Polarity = ClockPolarity.CPOL_0;
                        this.Phase = ClockPhase.CPHA_1;
                        break;
                    case Mode.Mode2:
                        this.Polarity = ClockPolarity.CPOL_1;
                        this.Phase = ClockPhase.CPHA_0;
                        break;
                    case Mode.Mode3:
                        this.Polarity = ClockPolarity.CPOL_1;
                        this.Phase = ClockPhase.CPHA_1;
                        break;
                }
            }
        }
    }
}
