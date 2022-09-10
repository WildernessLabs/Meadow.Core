using Meadow.Update;

namespace Meadow
{
    public static class AppSettings
    {
        public static ILoggingSettings DefaultLoggingSettings => new DefaultLoggingSettings();
        public static ILifecycleSettings DefaultLifecycleSettings => new DefaultLifecycleSettings();
        public static IUpdateSettings DefaultUpdateSettings => new DefaultUpdateSettings();
    }
}
