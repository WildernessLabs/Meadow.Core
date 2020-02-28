using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Encapsulates properties of a SPI Bus Clock
    /// </summary>
    public class SpiClockConfiguration
    {
        private long _speed;
        private ClockPhase _phase;
        private ClockPolarity _polarity;

        internal event EventHandler Changed;

        /// <summary>
        /// SPI Bus Clock Polarity (CPOL)
        /// </summary>
        public enum ClockPolarity
        {
            Normal = 0,
            Inverted = 1
        }

        /// <summary>
        /// SPI Bus Clock Phase (CPHA)
        /// </summary>
        public enum ClockPhase
        {
            Zero = 0,
            One = 1
        }

        /// <summary>
        /// Mode (combination of Phase and Polarity) of a SPI bus clock
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Normal (0) Polarity, and Phase 0
            /// </summary>
            Mode0,
            /// <summary>
            /// Normal (0) Polarity, and Phase 1
            /// </summary>
            Mode1,
            /// <summary>
            /// Inverted (1) Polarity, and Phase 0
            /// </summary>
            Mode2,
            /// <summary>
            /// Inverted (1) Polarity, and Phase 1
            /// </summary>
            Mode3
        }

        /// <summary>
        /// Gets or sets the current Polarity of the SPI bus clock
        /// </summary>
        public ClockPolarity Polarity 
        {
            get => _polarity;
            set
            {
                if (value == Polarity) return;
                _polarity = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the current Phase of the SPI bus clock
        /// </summary>
        public ClockPhase Phase 
        {
            get => _phase;
            set
            {
                if (value == Phase) return;
                _phase = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the desired speed or gets the actual speed of the SPI bus clock.
        /// </summary>
        /// <remarks>
        /// The set of supported speeds is programmatically available from the bus in the <b>SupportedSpeeds</b> property.
        /// </remarks>
        public long SpeedKHz 
        {
            get => _speed;
            set
            {
                if (value == SpeedKHz) return;
                _speed = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Provided to allow setting speed value without raising a Changed event.  This method is used internally.
        /// </summary>
        /// <param name="speed"></param>
        internal void SetActualSpeedKHz(long speed)
        {
            _speed = speed;
        }

        internal SpiClockConfiguration()
        {
        }

        /// <summary>
        /// Creates a SpiClockConfiguration instance
        /// </summary>
        /// <param name="speedKHz">Bus clock speed, in kHz</param>
        /// <param name="polarity">Bus clock polarity</param>
        /// <param name="phase">Bus clock phase</param>
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

        /// <summary>
        /// Creates a SpiClockConfiguration instance
        /// </summary>
        /// <param name="speedKHz">Bus clock speed, in kHz</param>
        /// <param name="mode">Bus Mode (phase and polarity)</param>
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
