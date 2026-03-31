using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public enum HardwareVersion
        {
            //#define MEADOW_F7_HW_VERSION_NUMB_F7V1 (1)
            //#define MEADOW_F7_HW_VERSION_NUMB_F7V2 (2)
            //#define MEADOW_F7_HW_VERSION_NUMB_CCMV2 (3)
            F7FeatherV1 = 1,
            F7FeatherV2 = 2,
            F7CoreComputeV2 = 3,
        }

        public static partial class Nuttx
        {
            [LibraryImport("nuttx")]
            public static partial HardwareVersion meadow_os_hardware_version();

            [LibraryImport("nuttx")]
            public static partial uint meadow_os_reset_reason();

            [LibraryImport("nuttx")]
            public static partial uint meadow_os_reset_cycle_count();

            [LibraryImport("nuttx")]
            public static partial uint meadow_os_power_cycle_count();
        }
    }
}
