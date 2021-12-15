using System.Runtime.InteropServices;

namespace Meadow
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GpioHandleRequest
    {
        /*
            #define GPIOHANDLES_MAX 64
            struct gpiohandle_request {
                __u32 lineoffsets[GPIOHANDLES_MAX];
                __u32 flags;
                __u8 default_values[GPIOHANDLES_MAX];
                char consumer_label[32];
                __u32 lines;
                int fd;
            };
        */
        public fixed int LineOffsets[64];
        public int Flags;
        public fixed byte DefaultValues[64];
        public fixed char ConsumerLabel[32];
        public int Lines;
        public int FileDescriptor;

    }
}
