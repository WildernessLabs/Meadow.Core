namespace Meadow
{
    public class DefaultLoggingSettings : ILoggingSettings
    {
        public DefaultLoggingSettings()
        {
        }

        public ILogLevelSettings LogLevel => new DefaultLogLevelSettings();
        public bool ShowTicks => false;
    }
}
