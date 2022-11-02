using System;

namespace Meadow
{
    [Flags]
    internal enum LineConfiguration
    {
        /*
            #define GPIOHANDLE_REQUEST_INPUT	(1UL << 0)
            #define GPIOHANDLE_REQUEST_OUTPUT	(1UL << 1)
            #define GPIOHANDLE_REQUEST_ACTIVE_LOW	(1UL << 2)
            #define GPIOHANDLE_REQUEST_OPEN_DRAIN	(1UL << 3)
            #define GPIOHANDLE_REQUEST_OPEN_SOURCE	(1UL << 4)
        */
        Input = 1 << 0,
        Output = 1 << 1,
        ActiveLow = 1 << 2,
        OpenDrain = 1 << 3,
        OpenSource = 1 << 4
    }
}
