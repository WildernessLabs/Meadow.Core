using System;

namespace Meadow
{
    internal static partial class Interop
    {
        [Flags]
        public enum PollEvent : short
        {
            NONE = 0,
            POLLIN = 0x0001,
            POLLPRI = 0x0002,
            POLLOUT = 0x0004,
            POLLERR = 0x0008,
            POLLHUP = 0x0010,
            POLLNVAL = 0x0020
        }

        public struct pollfd
        {
            public int fd;
            public PollEvent events;
            public PollEvent revents;
        }
    }
}
