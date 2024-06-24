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
            [DllImport("nuttx")]
            public static extern HardwareVersion meadow_os_hardware_version();

            [DllImport("nuttx")]
            public static extern uint meadow_os_reset_reason();

            [DllImport("nuttx")]
            public static extern uint meadow_os_reset_cycle_count();

            [DllImport("nuttx")]
            public static extern uint meadow_os_power_cycle_count();
        }
    }
}
