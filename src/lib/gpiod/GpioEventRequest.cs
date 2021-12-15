using System.Runtime.InteropServices;

namespace Meadow
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GpioEventRequest
    {
        /*
        struct gpioevent_request {
	        __u32 lineoffset;
	        __u32 handleflags;
	        __u32 eventflags;
	        char consumer_label[32];
	        int fd;
        };
        */
        public int LineOffset;
        public int HandleFlags;
        public int EventFlags;
        public fixed char ConsumerLabel[32];
        public int FileDescriptor;
    }
}
