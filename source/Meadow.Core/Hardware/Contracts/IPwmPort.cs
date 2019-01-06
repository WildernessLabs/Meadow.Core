namespace Meadow.Hardware
{
    public interface IPwmPort : IPort
    {
        // TODO: these should return Task or Task<void> or whatever
        void Start();
        void Stop();

        // TODO: correct type? should be UInt?
        double Duration { get; set; }
        double Period { get; set; }

        // TODO: correct type?
        double DutyCycle { get; set; }
        double Frequency { get; set; }

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
