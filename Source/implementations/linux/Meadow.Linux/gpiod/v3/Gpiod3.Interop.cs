using System;
using System.Runtime.InteropServices;

namespace Meadow;

internal partial class Gpiod3
{
    internal static class Interop
    {
        private const string LIB_GPIOD = "libgpiod.so.3";

        // ===== Enums =====

        public enum gpiod_line_value
        {
            GPIOD_LINE_VALUE_ERROR = -1,
            GPIOD_LINE_VALUE_INACTIVE = 0,
            GPIOD_LINE_VALUE_ACTIVE = 1,
        }

        public enum gpiod_line_direction
        {
            GPIOD_LINE_DIRECTION_AS_IS = 1,
            GPIOD_LINE_DIRECTION_INPUT,
            GPIOD_LINE_DIRECTION_OUTPUT,
        }

        public enum gpiod_line_edge
        {
            GPIOD_LINE_EDGE_NONE = 1,
            GPIOD_LINE_EDGE_RISING,
            GPIOD_LINE_EDGE_FALLING,
            GPIOD_LINE_EDGE_BOTH,
        }

        public enum gpiod_line_bias
        {
            GPIOD_LINE_BIAS_AS_IS = 1,
            GPIOD_LINE_BIAS_UNKNOWN,
            GPIOD_LINE_BIAS_DISABLED,
            GPIOD_LINE_BIAS_PULL_UP,
            GPIOD_LINE_BIAS_PULL_DOWN,
        }

        public enum gpiod_line_drive
        {
            GPIOD_LINE_DRIVE_PUSH_PULL = 1,
            GPIOD_LINE_DRIVE_OPEN_DRAIN,
            GPIOD_LINE_DRIVE_OPEN_SOURCE,
        }

        // ===== Chip Operations =====

        [DllImport(LIB_GPIOD, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr gpiod_chip_open([MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern void gpiod_chip_close(IntPtr chip);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern IntPtr gpiod_chip_get_info(IntPtr chip);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern IntPtr gpiod_chip_request_lines(IntPtr chip, IntPtr req_cfg, IntPtr line_cfg);

        // ===== Chip Info Operations =====

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern void gpiod_chip_info_free(IntPtr info);

        [DllImport(LIB_GPIOD, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr gpiod_chip_info_get_name(IntPtr info);

        [DllImport(LIB_GPIOD, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr gpiod_chip_info_get_label(IntPtr info);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern uint gpiod_chip_info_get_num_lines(IntPtr info);

        // ===== Line Info Operations =====

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern IntPtr gpiod_chip_get_line_info(IntPtr chip, uint offset);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern void gpiod_line_info_free(IntPtr info);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern uint gpiod_line_info_get_offset(IntPtr info);

        [DllImport(LIB_GPIOD, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr gpiod_line_info_get_name(IntPtr info);

        [DllImport(LIB_GPIOD, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr gpiod_line_info_get_consumer(IntPtr info);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern gpiod_line_direction gpiod_line_info_get_direction(IntPtr info);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern bool gpiod_line_info_is_used(IntPtr info);

        // ===== Request Config Operations =====

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern IntPtr gpiod_request_config_new();

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern void gpiod_request_config_free(IntPtr config);

        [DllImport(LIB_GPIOD, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern void gpiod_request_config_set_consumer(IntPtr config, [MarshalAs(UnmanagedType.LPStr)] string consumer);

        // ===== Line Config Operations =====

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern IntPtr gpiod_line_config_new();

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern void gpiod_line_config_free(IntPtr config);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern int gpiod_line_config_add_line_settings(IntPtr config, uint[] offsets, UIntPtr num_offsets, IntPtr settings);

        // ===== Line Settings Operations =====

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern IntPtr gpiod_line_settings_new();

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern void gpiod_line_settings_free(IntPtr settings);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern int gpiod_line_settings_set_direction(IntPtr settings, gpiod_line_direction direction);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern int gpiod_line_settings_set_edge_detection(IntPtr settings, gpiod_line_edge edge);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern int gpiod_line_settings_set_bias(IntPtr settings, gpiod_line_bias bias);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern int gpiod_line_settings_set_output_value(IntPtr settings, gpiod_line_value value);

        // ===== Line Request Operations =====

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern void gpiod_line_request_release(IntPtr request);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern gpiod_line_value gpiod_line_request_get_value(IntPtr request, uint offset);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern int gpiod_line_request_set_value(IntPtr request, uint offset, gpiod_line_value value);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern int gpiod_line_request_wait_edge_events(IntPtr request, long timeout_ns);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern int gpiod_line_request_read_edge_events(IntPtr request, IntPtr buffer, UIntPtr max_events);

        // ===== Edge Event Operations =====

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern IntPtr gpiod_edge_event_buffer_new(UIntPtr capacity);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern void gpiod_edge_event_buffer_free(IntPtr buffer);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern IntPtr gpiod_edge_event_buffer_get_event(IntPtr buffer, ulong index);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern UIntPtr gpiod_edge_event_buffer_get_num_events(IntPtr buffer);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern uint gpiod_edge_event_get_line_offset(IntPtr evt);

        [DllImport(LIB_GPIOD, SetLastError = true)]
        public static extern ulong gpiod_edge_event_get_timestamp_ns(IntPtr evt);
    }
}
