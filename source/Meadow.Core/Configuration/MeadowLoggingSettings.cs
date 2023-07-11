using Meadow.Logging;

namespace Meadow;

internal class MeadowLoggingSettings : ILoggingSettings
{
    public bool ShowTicks { get; set; } = false;
    public ILogLevelSettings LogLevel { get; } = new LogLevelSettings();

    public class LogLevelSettings : ILogLevelSettings
    {
        public LogLevel Default { get; set; } = Logging.LogLevel.Information;
    }
}
