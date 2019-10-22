using System;
namespace Meadow.Hardware
{
    public class SpiClockConfiguration
    {
        public enum ClockPolarity
        {
            Normal = 0,
            Inverted = 1
        }

        public enum ClockPhase
        {
            Zero = 0,
            One = 1
        }

        public enum Mode
        {
            Mode0,
            Mode1,
            Mode2,
            Mode3
        }

        public ClockPolarity Polarity { get; internal set; }
        public ClockPhase Phase { get; internal set; }
        /// <summary>
        /// The desired bus clock speed.
        /// </summary>
        /// <remarks>
        /// The set of supported speeds is programmatically available from the bus in the <b>SupportedSpeeds</b> property.
        /// </remarks>
        public long SpeedKHz { get; internal set; }

        internal SpiClockConfiguration()
        {
        }

        public SpiClockConfiguration(
            long speedKHz,
            ClockPolarity polarity = ClockPolarity.Normal,
            ClockPhase phase = ClockPhase.Zero
        )
        {
            this.SpeedKHz = speedKHz;
            this.Polarity = polarity;
            this.Phase = phase;
        }

        public SpiClockConfiguration(
            long speedKHz,
            Mode mode
        )
        {
            this.SpeedKHz = speedKHz;
            switch (mode) {
                case Mode.Mode0:
                    this.Polarity = ClockPolarity.Normal;
                    this.Phase = ClockPhase.Zero;
                    break;
                case Mode.Mode1:
                    this.Polarity = ClockPolarity.Normal;
                    this.Phase = ClockPhase.One;
                    break;
                case Mode.Mode2:
                    this.Polarity = ClockPolarity.Inverted;
                    this.Phase = ClockPhase.Zero;
                    break;
                case Mode.Mode3:
                    this.Polarity = ClockPolarity.Inverted;
                    this.Phase = ClockPhase.One;
                    break;
            }
        }
    }
}
