using Meadow.Logging;

namespace Meadow
{
    public class DefaultLogLevelSettings : ILogLevelSettings
    {
        public DefaultLogLevelSettings()
        {
        }

        public string Default => LogLevel.Information.ToString();
    }
}
