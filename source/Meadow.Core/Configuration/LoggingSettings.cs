namespace Meadow
{
    internal class LoggingSettings : ConfigurableObject
    {
        public LoggingSettings() : base()
        {
            LogLevel = new LogLevelSettings(this, null);
        }

        public LogLevelSettings LogLevel { get; set; }

        public class LogLevelSettings : ConfigurableObject
        {
            public LogLevelSettings()
            {
            }

            public LogLevelSettings(object parent, string? configRootPath)
                : base(parent, configRootPath)
            {
            }

            public string Default => this.GetConfiguredValue() ?? "Info";
        }
    }
}
