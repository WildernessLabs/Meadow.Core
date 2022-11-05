using static Meadow.IPlatformOS;

namespace Meadow
{
    public static partial class MeadowOS
    {
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
            public static string MonoVersion => CurrentDevice.PlatformOS.MonoVersion;

            /// <summary>
            /// Should the system reboot if an unhandled exception is encounted in the user application?
            /// </summary>
            public static bool RebootOnUnhandledException => CurrentDevice.PlatformOS.RebootOnUnhandledException;

            /// <summary>
            /// Number of seconds the initialization method in the user application is allowed to run before
            /// it is assumed to have crashed.
            /// </summary>
            /// <remarks>A value of 0 indicates an infinite period.</remarks>
            public static uint InitizationTimeout => CurrentDevice.PlatformOS.InitializationTimeout;

            /// <summary>
            /// Is an SD Card connected to the module.
            /// </summary>
            /// <remarks>This is currently only relevant to the CCM.</remarks>
            /// <returns>True if an SD card is connected to the STM32, false otherwise.</returns>
            public static bool SdCardPresent => CurrentDevice.PlatformOS.SdCardPresent;

            /// <summary>
            /// Get the selected network.
            /// </summary>
            /// <remarks>The current network can be changed through the meadow.config.yaml file.</remarks>
            /// <returns>Currently selected network.</returns>
            public static NetworkConnectionType SelectedNetwork => CurrentDevice.PlatformOS.SelectedNetwork;
        }
    }
}
