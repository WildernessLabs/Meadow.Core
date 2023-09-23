namespace Meadow;

public static partial class MeadowOS
{
    /// <summary>
    /// A static convenience accessor for retrieving system information
    /// </summary>
    public static class SystemInformation
    {
        /// <summary>
        /// Get the OS version.
        /// </summary>
        /// <returns>OS version.</returns>
        public static string OSVersion => CurrentDevice.PlatformOS.OSVersion;

        /// <summary>
        /// Get the OS build date.
        /// </summary>
        /// <returns>OS build date.</returns>
        //TODO: parse as datetime
        public static string OSBuildDate => CurrentDevice.PlatformOS.OSBuildDate;

        /// <summary>
        /// Get the mono version on the device.
        /// </summary>
        /// <returns>Mono version.</returns>
        public static string RuntimeVersion => CurrentDevice.PlatformOS.RuntimeVersion;

        /// <summary>
        /// Should the system reboot if an unhandled exception is encountered in the user application?
        /// </summary>
        public static bool RebootOnUnhandledException => CurrentDevice.PlatformOS.RebootOnUnhandledException;

        /// <summary>
        /// Number of seconds the initialization method in the user application is allowed to run before
        /// it is assumed to have crashed.
        /// </summary>
        /// <remarks>A value of 0 indicates an infinite period.</remarks>
        public static uint InitizationTimeout => CurrentDevice.PlatformOS.InitializationTimeout;
    }
}
