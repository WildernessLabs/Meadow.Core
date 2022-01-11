namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a Port that has is capable of 
    /// </summary>
    public interface IPwmPort : IDigitalPort
    {
        /// <summary>
        /// PWM channel value
        /// </summary>
        new IPwmChannelInfo Channel { get; }

        void Start();
        void Stop();

        /// <summary>
        /// Duration of pulse
        /// </summary>
        float Duration { get; set; }

        /// <summary>
        /// Period of pulse
        /// </summary>
        float Period { get; set; }

        /// <summary>
        /// Duty cycle 
        /// </summary>
        float DutyCycle { get; set; }

        /// <summary>
        /// Frequency 
        /// </summary>
        float Frequency { get; set; }

        /// <summary>
        /// Is PWM signal inverted
        /// </summary>
        bool Inverted { get; set; }

        /// <summary>
        /// Is running
        /// </summary>
        bool State { get; }

        /// <summary>
        /// Timescale for time calcutions (will be removed in future revisions)
        /// </summary>
        TimeScale TimeScale { get; set; }
    }

    /// <summary>
    /// Timescale enum
    /// </summary>
    public enum TimeScale
    {
        /// <summary>
        /// Seconds
        /// </summary>
        Seconds = 1,
        /// <summary>
        /// Milliseconds
        /// </summary>
        Milliseconds = 1000,
        /// <summary>
        /// Microseconds
        /// </summary>
        Microseconds = 1000000
    }
}
