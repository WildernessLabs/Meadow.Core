namespace Meadow
{
    public class DefaultLoggingSettings : ILoggingSettings
    {
        public DefaultLoggingSettings()
        {
        }

        public ILogLevelSettings LogLevel => new DefaultLogLevelSettings();
    }
}
