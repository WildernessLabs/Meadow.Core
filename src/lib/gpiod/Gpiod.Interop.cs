using System;
using System.Runtime.InteropServices;

namespace Meadow
{
    internal partial class Gpiod
    {
        // https://github.com/brgl/libgpiod/blob/master/include/gpiod.h

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

            public enum line_request
            {
                GPIOD_LINE_REQUEST_DIRECTION_AS_IS = 1,
                /**< Request the line(s), but don't change current direction. */
                GPIOD_LINE_REQUEST_DIRECTION_INPUT,
                /**< Request the line(s) for reading the GPIO line state. */
                GPIOD_LINE_REQUEST_DIRECTION_OUTPUT,
                /**< Request the line(s) for setting the GPIO line state. */
                GPIOD_LINE_REQUEST_EVENT_FALLING_EDGE,
                /**< Only watch falling edge events. */
                GPIOD_LINE_REQUEST_EVENT_RISING_EDGE,
                /**< Only watch rising edge events. */
                GPIOD_LINE_REQUEST_EVENT_BOTH_EDGES,
                /**< Monitor both types of events. */
            }

            [Flags]
            public enum line_request_flags
            {
                None = 0,
                GPIOD_LINE_REQUEST_FLAG_OPEN_DRAIN = 1 << 0,
                /**< The line is an open-drain port. */
                GPIOD_LINE_REQUEST_FLAG_OPEN_SOURCE = 1 << 1,
                /**< The line is an open-source port. */
                GPIOD_LINE_REQUEST_FLAG_ACTIVE_LOW = 1 << 2,
                /**< The active state of the line is low (high is the default). */
                GPIOD_LINE_REQUEST_FLAG_BIAS_DISABLE = 1 << 3,
                /**< The line has neither either pull-up nor pull-down resistor. */
                GPIOD_LINE_REQUEST_FLAG_BIAS_PULL_DOWN = 1 << 4,
                /**< The line has pull-down resistor enabled. */
                GPIOD_LINE_REQUEST_FLAG_BIAS_PULL_UP = 1 << 5,
                /**< The line has pull-up resistor enabled. */
            }

            public enum line_direction
            {
                GPIOD_LINE_DIRECTION_INPUT = 1,
                /**< Direction is input - we're reading the state of a GPIO line. */
                GPIOD_LINE_DIRECTION_OUTPUT,
                /**< Direction is output - we're driving the GPIO line. */
            };

            public enum line_active_state
            {
                GPIOD_LINE_ACTIVE_STATE_HIGH = 1,
                /**< The active state of a GPIO is active-high. */
                GPIOD_LINE_ACTIVE_STATE_LOW,
                /**< The active state of a GPIO is active-low. */
            };

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            internal unsafe struct gpiod_line
            {
                public uint offset;

                // The direction of the GPIO line. 
                public line_direction direction;

                // The active-state configuration. 
                public line_active_state active_state;

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

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string name;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string consumer;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            internal struct gpiod_chip
            {
                public IntPtr lines; // gpiod_line **lines;
                public uint num_lines;

                public int fd;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string name;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string label;
            }

            internal enum gpiod_event_type
            {
                GPIOD_LINE_EVENT_RISING_EDGE = 1,
                /**< Rising edge event. */
                GPIOD_LINE_EVENT_FALLING_EDGE,
                /**< Falling edge event. */
            }

            internal struct gpiod_line_request_config
            {
                public string consumer;
                /**< Name of the consumer. */
                public int request_type;
                /**< Request type. */
                public int flags;
                /**< Other configuration flags. */
            };

            internal struct gpiod_line_event
            {
                public timespec ts;
                /**< Best estimate of time of event occurrence. */
                public gpiod_event_type event_type;
                /**< Type of the event that occurred. */
            };

            private const string LIB_GPIOD = "libgpiod.so.2";

            // struct gpiod_chip *gpiod_chip_open_by_name(const char *name)
            [DllImport(LIB_GPIOD, SetLastError = true, CharSet=CharSet.Ansi)]
            public static extern IntPtr gpiod_chip_open_by_name([MarshalAs(UnmanagedType.LPStr)] string name);

            // struct gpiod_chip *gpiod_chip_open_by_number(unsigned int num) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern IntPtr gpiod_chip_open_by_number(uint num);

            // void gpiod_chip_close(struct gpiod_chip *chip)
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern void gpiod_chip_close(IntPtr chip);

            // struct gpiod_line *gpiod_chip_get_line(struct gpiod_chip *chip, unsigned int offset)
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern IntPtr gpiod_chip_get_line(IntPtr chip, int offset);

            // int gpiod_line_update(struct gpiod_line *line)
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_update(IntPtr line);

            /**
             * @brief Reserve a single line.
             * @param line GPIO line object.
             * @param config Request options.
             * @param default_val Initial line value - only relevant if we're setting
             *                    the direction to output.
             * @return 0 if the line was properly reserved. In case of an error this
             *         routine returns -1 and sets the last error number.
             *
             * If this routine succeeds, the caller takes ownership of the GPIO line until
             * it's released.
             */
            // int gpiod_line_request(struct gpiod_line *line, const struct gpiod_line_request_config *config, int default_val) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_request(IntPtr line, IntPtr config, int default_val);

            /**
             * @brief Reserve a single line, set the direction to input.
             * @param line GPIO line object.
             * @param consumer Name of the consumer.
             * @return 0 if the line was properly reserved, -1 on failure.
             */
            //int gpiod_line_request_input(struct gpiod_line *line, const char *consumer) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_request_input(IntPtr line, [MarshalAs(UnmanagedType.LPStr)] string consumer);

            /**
             * @brief Reserve a single line, set the direction to input.
             * @param line GPIO line object.
             * @param consumer Name of the consumer.
             * @param flags Additional request flags.
             * @return 0 if the line was properly reserved, -1 on failure.
             */
            // int gpiod_line_request_input_flags(struct gpiod_line *line, const char* consumer, int flags) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_request_input_flags(IntPtr line, [MarshalAs(UnmanagedType.LPStr)] string consumer, line_request_flags flags);

            /**
             * @brief Reserve a single line, set the direction to output.
             * @param line GPIO line object.
             * @param consumer Name of the consumer.
             * @param default_val Initial line value.
             * @return 0 if the line was properly reserved, -1 on failure.
             */
            //int gpiod_line_request_output(struct gpiod_line *line, const char *consumer, int default_val) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_request_output(IntPtr line, [MarshalAs(UnmanagedType.LPStr)] string consumer);


            /**
             * @brief Read current value of a single GPIO line.
             * @param line GPIO line object.
             * @return 0 or 1 if the operation succeeds. On error this routine returns -1
             *         and sets the last error number.
             */
            // int gpiod_line_get_value(struct gpiod_line *line) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_get_value(IntPtr line);

            /**
             * @brief Set the value of a single GPIO line.
             * @param line GPIO line object.
             * @param value New value.
             * @return 0 is the operation succeeds. In case of an error this routine
             *         returns -1 and sets the last error number.
             */
            // int gpiod_line_set_value(struct gpiod_line *line, int value) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_set_value(IntPtr line, int value);

            /**
             * @brief Release a previously reserved line.
             * @param line GPIO line object.
             */
            // void gpiod_line_release(struct gpiod_line *line) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern void gpiod_line_release(IntPtr line);

            // bool gpiod_line_is_requested(struct gpiod_line *line) GPIOD_API;
            // bool gpiod_line_is_free(struct gpiod_line *line) GPIOD_API;

            /**
             * @brief Update the configuration of a single GPIO line.
             * @param line GPIO line object.
             * @param direction Updated direction which may be one of
             *                  GPIOD_LINE_REQUEST_DIRECTION_AS_IS,
             *                  GPIOD_LINE_REQUEST_DIRECTION_INPUT, or
             *                  GPIOD_LINE_REQUEST_DIRECTION_OUTPUT.
             * @param flags Replacement flags.
             * @param value The new output value for the line when direction is
             *              GPIOD_LINE_REQUEST_DIRECTION_OUTPUT.
             * @return 0 is the operation succeeds. In case of an error this routine
             *         returns -1 and sets the last error number.
             */
            // int gpiod_line_set_config(struct gpiod_line *line, int direction, int flags, int value) GPIOD_API;

            /**
             * @brief Set the direction of a single GPIO line to output.
             * @param line GPIO line object.
             * @param value The logical value output on the line.
             * @return 0 is the operation succeeds. In case of an error this routine
             *         returns -1 and sets the last error number.
             */
            //int gpiod_line_set_direction_output(struct gpiod_line *line, int value) GPIOD_API;

            /**
             * @brief Set the direction of a single GPIO line to input.
             * @param line GPIO line object.
             * @return 0 is the operation succeeds. In case of an error this routine
             *         returns -1 and sets the last error number.
             */
            //int gpiod_line_set_direction_input(struct gpiod_line *line) GPIOD_API;

            /**
             * @brief Update the configuration flags of a single GPIO line.
             * @param line GPIO line object.
             * @param flags Replacement flags.
             * @return 0 is the operation succeeds. In case of an error this routine
             *         returns -1 and sets the last error number.
             */
            // int gpiod_line_set_flags(struct gpiod_line *line, int flags) GPIOD_API;

            internal struct timespec
            {
                public ulong tv_sec;      //   seconds 
                public ulong tv_nsec;       // nanoseconds 
            };

            /**
             * @brief Wait for an event on a single line.
             * @param line GPIO line object.
             * @param timeout Wait time limit.
             * @return 0 if wait timed out, -1 if an error occurred, 1 if an event
             *         occurred.
             */
            //int gpiod_line_event_wait(struct gpiod_line *line, const struct timespec *timeout) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_event_wait(IntPtr line, ref timespec timeout);


            /**
             * @brief Request rising edge event notifications on a single line.
             * @param line GPIO line object.
             * @param consumer Name of the consumer.
             * @return 0 if the operation succeeds, -1 on failure.
             */
            //int gpiod_line_request_rising_edge_events(struct gpiod_line *line, const char* consumer) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_request_rising_edge_events(IntPtr line, [MarshalAs(UnmanagedType.LPStr)] string consumer);

            /**
             * @brief Request rising edge event notifications on a single line.
             * @param line GPIO line object.
             * @param consumer Name of the consumer.
             * @param flags Additional request flags.
             * @return 0 if the operation succeeds, -1 on failure.
             */
            // int gpiod_line_request_rising_edge_events_flags(struct gpiod_line *line, const char* consumer, int flags) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_request_rising_edge_events_flags(IntPtr line, [MarshalAs(UnmanagedType.LPStr)]  string consumer, line_request_flags flags);

            /**
             * @brief Request falling edge event notifications on a single line.
             * @param line GPIO line object.
             * @param consumer Name of the consumer.
             * @return 0 if the operation succeeds, -1 on failure.
             */
            //int gpiod_line_request_falling_edge_events(struct gpiod_line *line, const char *consumer) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_request_falling_edge_events(IntPtr line, [MarshalAs(UnmanagedType.LPStr)] string consumer);

            /**
             * @brief Request all event type notifications on a single line.
             * @param line GPIO line object.
             * @param consumer Name of the consumer.
             * @return 0 if the operation succeeds, -1 on failure.
             */
            //int gpiod_line_request_both_edges_events(struct gpiod_line *line, const char *consumer) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_request_both_edges_events(IntPtr line, [MarshalAs(UnmanagedType.LPStr)] string consumer);

            /**
             * @brief Request falling edge event notifications on a single line.
             * @param line GPIO line object.
             * @param consumer Name of the consumer.
             * @param flags Additional request flags.
             * @return 0 if the operation succeeds, -1 on failure.
             */
            // int gpiod_line_request_falling_edge_events_flags(struct gpiod_line *line, const char* consumer, int flags) GPIOD_API;

            /**
             * @brief Request all event type notifications on a single line.
             * @param line GPIO line object.
             * @param consumer Name of the consumer.
             * @param flags Additional request flags.
             * @return 0 if the operation succeeds, -1 on failure.
             */
            //int gpiod_line_request_both_edges_events_flags(struct gpiod_line *line, const char* consumer, int flags) GPIOD_API;       

            /**
             * @brief Read next pending event from the GPIO line.
             * @param line GPIO line object.
             * @param event Buffer to which the event data will be copied.
             * @return 0 if the event was read correctly, -1 on error.
             * @note This function will block if no event was queued for this line.
             */
            //int gpiod_line_event_read(struct gpiod_line *line, struct gpiod_line_event *event) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern int gpiod_line_event_read(IntPtr line, ref gpiod_line_event evnt);

            // struct gpiod_chip_iter *gpiod_chip_iter_new(void) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern IntPtr gpiod_chip_iter_new();

            // void gpiod_chip_iter_free(struct gpiod_chip_iter *iter) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern void gpiod_chip_iter_free(IntPtr iter);

            // struct gpiod_chip *gpiod_chip_iter_next(struct gpiod_chip_iter *iter) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern IntPtr gpiod_chip_iter_next(IntPtr iter);

            // struct gpiod_chip *gpiod_chip_iter_next_noclose(struct gpiod_chip_iter *iter) GPIOD_API;
            [DllImport(LIB_GPIOD, SetLastError = true)]
            public static extern IntPtr gpiod_chip_iter_next_noclose(IntPtr iter);
        }
    }
}