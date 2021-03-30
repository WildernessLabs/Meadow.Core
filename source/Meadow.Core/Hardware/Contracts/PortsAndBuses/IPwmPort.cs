namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a Port that has is capable of 
    /// </summary>
    public interface IPwmPort : IDigitalPort
    {
        new IPwmChannelInfo Channel { get; }

        void Start();
        void Stop();

        float Duration { get; set; }
        float Period { get; set; }

        float DutyCycle { get; set; }
        float Frequency { get; set; }

        bool Inverted { get; set; }

        // Whether or not it's running
        bool State { get; }

        TimeScale TimeScale { get; set; }
    }

    public enum TimeScale
    {
        Seconds = 1,
        Milliseconds = 1000,
        Microseconds = 1000000
    }
}
