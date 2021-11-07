using System;
using System.Runtime.InteropServices;

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
            /// Get the OS build date
            /// </summary>
            /// <returns>OS build date.</returns>

            //TODO: parse as datetime
            public static string OSBuildDate => CurrentDevice.PlatformOS.OSBuildDate;
            /// <summary>
            /// Get the mono version on the device..
            /// </summary>
            /// <returns>Mono version.</returns>
            public static string MonoVersion => CurrentDevice.PlatformOS.MonoVersion;
        }



    }
}
