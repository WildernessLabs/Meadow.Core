namespace Meadow
{
    public interface ILoggingSettings
    {
        bool ShowTicks { get; }
        ILogLevelSettings LogLevel { get; }
    }
}
