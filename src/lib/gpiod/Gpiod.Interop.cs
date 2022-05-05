using System;
using System.Runtime.InteropServices;

namespace Meadow
{
    internal partial class Gpiod
    {
        internal static class Interop
        {
            /*
            struct gpiod_chip {
                struct gpiod_line **lines;
                unsigned int num_lines;

                int fd;

                char name[32];
                char label[32];
            };

            struct line_fd_handle {
                int fd;
                int refcount;
            };

            struct gpiod_line {
                unsigned int offset;

                /* The direction of the GPIO line. 
                int direction;

                /* The active-state configuration. 
                int active_state;

                /* The logical value last written to the line. 
                int output_value;

                /* The GPIOLINE_FLAGs returned by GPIO_GET_LINEINFO_IOCTL. 
                __u32 info_flags;

                /* The GPIOD_LINE_REQUEST_FLAGs provided to request the line.
                __u32 req_flags;

                 * Indicator of LINE_FREE, LINE_REQUESTED_VALUES or
                 * LINE_REQUESTED_EVENTS.
                int state;

                struct gpiod_chip *chip;
            struct line_fd_handle *fd_handle;

            char name[32];
                char consumer[32];
            };
        */
            [StructLayout(LayoutKind.Sequential)]
            internal struct line_fd_handle
            {
                public int fd;
                public int refcount;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct gpiod_line
            {
                public uint offset;

                // The direction of the GPIO line. 
                public int direction;

                // The active-state configuration. 
                public int active_state;

                // The logical value last written to the line. 
                public int output_value;

                // The GPIOLINE_FLAGs returned by GPIO_GET_LINEINFO_IOCTL. 
                public uint info_flags;

                // The GPIOD_LINE_REQUEST_FLAGs provided to request the line.
                public uint req_flags;

                // Indicator of LINE_FREE, LINE_REQUESTED_VALUES or
                // LINE_REQUESTED_EVENTS.
                public int state;

                public IntPtr chip; // gpiod_chip *chip;
                public IntPtr fd_handle; // line_fd_handle *fd_handle;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                char[] name;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                char[] consumer;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal unsafe struct gpiod_chip
            {
                public IntPtr lines; // gpiod_line **lines;
                public uint num_lines;

                public int fd;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                public char[] name;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                public char[] label;
            }

            private const string LIB_GPIOD = "libgpiod.so.2";

            // struct gpiod_chip *gpiod_chip_open_by_name(const char *name)
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern IntPtr gpiod_chip_open_by_name([MarshalAs(UnmanagedType.LPStr)] string name);

            // void gpiod_chip_close(struct gpiod_chip *chip)
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern void gpiod_chip_close(IntPtr chip);
        }
    }
}
