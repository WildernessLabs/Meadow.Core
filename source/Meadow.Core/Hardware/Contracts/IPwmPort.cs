namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a Port that has is capable of 
    /// </summary>
    public interface IPwmPort : IDigitalPort
    {
        IPwmChannelInfo ChannelInfo { get; }

        // TODO: these should return Task or Task<void> or whatever
        void Start();
        void Stop();

        // TODO: correct type? should be UInt?
        float Duration { get; set; }
        float Period { get; set; }

        float DutyCycle { get; set; }
        // TODO: correct type?
        float Frequency { get; set; }

        bool Inverted { get; set; }

        TimeScaleFactor Scale { get; set; }

    }

    // TODO: Maybe factor out?
    public enum TimeScaleFactor : uint
    {
        Milliseconds = 1000,
        Microseconds = 1000000,
        Nanoseconds = 1000000000
    }
}
