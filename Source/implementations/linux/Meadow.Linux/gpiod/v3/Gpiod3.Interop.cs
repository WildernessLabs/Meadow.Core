using System;
using System.Runtime.InteropServices;

namespace Meadow;

internal partial class Gpiod3
{
    internal static partial class Interop
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

        [LibraryImport(LIB_GPIOD, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
        public static partial IntPtr gpiod_chip_open(string path);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial void gpiod_chip_close(IntPtr chip);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial IntPtr gpiod_chip_get_info(IntPtr chip);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial IntPtr gpiod_chip_request_lines(IntPtr chip, IntPtr req_cfg, IntPtr line_cfg);

        // ===== Chip Info Operations =====

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial void gpiod_chip_info_free(IntPtr info);

        [LibraryImport(LIB_GPIOD, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
        public static partial IntPtr gpiod_chip_info_get_name(IntPtr info);

        [LibraryImport(LIB_GPIOD, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
        public static partial IntPtr gpiod_chip_info_get_label(IntPtr info);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial uint gpiod_chip_info_get_num_lines(IntPtr info);

        // ===== Line Info Operations =====

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial IntPtr gpiod_chip_get_line_info(IntPtr chip, uint offset);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial void gpiod_line_info_free(IntPtr info);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial uint gpiod_line_info_get_offset(IntPtr info);

        [LibraryImport(LIB_GPIOD, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
        public static partial IntPtr gpiod_line_info_get_name(IntPtr info);

        [LibraryImport(LIB_GPIOD, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
        public static partial IntPtr gpiod_line_info_get_consumer(IntPtr info);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial gpiod_line_direction gpiod_line_info_get_direction(IntPtr info);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool gpiod_line_info_is_used(IntPtr info);

        // ===== Request Config Operations =====

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial IntPtr gpiod_request_config_new();

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial void gpiod_request_config_free(IntPtr config);

        [LibraryImport(LIB_GPIOD, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
        public static partial void gpiod_request_config_set_consumer(IntPtr config, string consumer);

        // ===== Line Config Operations =====

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial IntPtr gpiod_line_config_new();

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial void gpiod_line_config_free(IntPtr config);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial int gpiod_line_config_add_line_settings(IntPtr config, uint[] offsets, UIntPtr num_offsets, IntPtr settings);

        // ===== Line Settings Operations =====

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial IntPtr gpiod_line_settings_new();

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial void gpiod_line_settings_free(IntPtr settings);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial int gpiod_line_settings_set_direction(IntPtr settings, gpiod_line_direction direction);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial int gpiod_line_settings_set_edge_detection(IntPtr settings, gpiod_line_edge edge);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial int gpiod_line_settings_set_bias(IntPtr settings, gpiod_line_bias bias);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial int gpiod_line_settings_set_output_value(IntPtr settings, gpiod_line_value value);

        // ===== Line Request Operations =====

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial void gpiod_line_request_release(IntPtr request);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial gpiod_line_value gpiod_line_request_get_value(IntPtr request, uint offset);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial int gpiod_line_request_set_value(IntPtr request, uint offset, gpiod_line_value value);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial int gpiod_line_request_wait_edge_events(IntPtr request, long timeout_ns);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial int gpiod_line_request_read_edge_events(IntPtr request, IntPtr buffer, UIntPtr max_events);

        // ===== Edge Event Operations =====

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial IntPtr gpiod_edge_event_buffer_new(UIntPtr capacity);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial void gpiod_edge_event_buffer_free(IntPtr buffer);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial IntPtr gpiod_edge_event_buffer_get_event(IntPtr buffer, ulong index);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial UIntPtr gpiod_edge_event_buffer_get_num_events(IntPtr buffer);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial uint gpiod_edge_event_get_line_offset(IntPtr evt);

        [LibraryImport(LIB_GPIOD, SetLastError = true)]
        public static partial ulong gpiod_edge_event_get_timestamp_ns(IntPtr evt);
    }
}
