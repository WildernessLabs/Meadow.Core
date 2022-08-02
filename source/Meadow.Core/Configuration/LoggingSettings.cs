namespace Meadow
{
    internal class LoggingSettings : ConfigurableObject, ILoggingSettings
    {
        public LoggingSettings() : base()
        {
            LogLevel = new LogLevelSettings(this, null);
        }

        public ILogLevelSettings LogLevel { get; set; }

        public class LogLevelSettings : ConfigurableObject, ILogLevelSettings
        {
            public LogLevelSettings()
            {
            }

            public LogLevelSettings(object parent, string? configRootPath)
                : base(parent, configRootPath)
            {
            }

            public string Default => this.GetConfiguredValue() ?? Logging.LogLevel.Information.ToString();
        }
    }
}
